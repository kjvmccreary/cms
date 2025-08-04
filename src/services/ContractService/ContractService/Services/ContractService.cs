using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ContractService.Data;
using ContractService.Models;
using ContractService.DTOs;
using Shared.Infrastructure;
using Shared.DTOs;

namespace ContractService.Services
{
    public class ContractService : IContractService
    {
        private readonly ContractDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<ContractService> _logger;

        public ContractService(
            ContractDbContext context,
            IMapper mapper,
            ITenantContext tenantContext,
            ILogger<ContractService> logger)
        {
            _context = context;
            _mapper = mapper;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task<PagedResult<ContractDto>> GetContractsAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            try
            {
                var query = _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties.OrderBy(p => p.SortOrder))
                    .Where(c => c.TenantId == _tenantContext.TenantId);

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(c => 
                        c.Title.Contains(search) ||
                        c.ContractNumber.Contains(search) ||
                        (c.Description != null && c.Description.Contains(search)) ||
                        c.ContractType.Name.Contains(search));
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ContractStatus>(status, true, out var statusEnum))
                {
                    query = query.Where(c => c.Status == statusEnum);
                }

                var totalCount = await query.CountAsync();

                // Apply pagination and ordering
                var contracts = await query
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var contractDtos = _mapper.Map<List<ContractDto>>(contracts);

                // Populate user names (in a real implementation, you might want to do this via a user service)
                await PopulateUserNamesAsync(contractDtos);

                return new PagedResult<ContractDto>
                {
                    Items = contractDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractDto?> GetContractByIdAsync(Guid id)
        {
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties.OrderBy(p => p.SortOrder))
                    .Include(c => c.ParentContract)
                    .Include(c => c.ChildContracts)
                    .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);

                if (contract == null) return null;

                var contractDto = _mapper.Map<ContractDto>(contract);
                await PopulateUserNamesAsync(new List<ContractDto> { contractDto });

                return contractDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract {ContractId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractDto> CreateContractAsync(CreateContractDto createDto)
        {
            try
            {
                // Validate contract type exists
                var contractType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Id == createDto.ContractTypeId && ct.TenantId == _tenantContext.TenantId && ct.IsActive);

                if (contractType == null)
                {
                    throw new ArgumentException("Contract type not found or inactive.");
                }

                // Validate business rules
                if (createDto.EndDate.HasValue && createDto.EndDate <= createDto.StartDate)
                {
                    throw new ArgumentException("End date must be after start date.");
                }

                // Create new contract
                var contract = _mapper.Map<Contract>(createDto);
                contract.Id = Guid.NewGuid();
                contract.ContractNumber = await GenerateContractNumberAsync();
                contract.Status = ContractStatus.Draft;
                contract.TenantId = _tenantContext.TenantId;
                contract.CreatedAt = DateTime.UtcNow;
                contract.UpdatedAt = DateTime.UtcNow;
                contract.CreatedBy = _tenantContext.UserId;
                contract.UpdatedBy = _tenantContext.UserId;

                _context.Contracts.Add(contract);

                // Add parties if provided
                if (createDto.Parties?.Any() == true)
                {
                    foreach (var partyDto in createDto.Parties.OrderBy(p => p.SortOrder))
                    {
                        var party = _mapper.Map<ContractParty>(partyDto);
                        party.Id = Guid.NewGuid();
                        party.ContractId = contract.Id;
                        party.TenantId = _tenantContext.TenantId;
                        party.CreatedAt = DateTime.UtcNow;
                        party.UpdatedAt = DateTime.UtcNow;
                        party.CreatedBy = _tenantContext.UserId;
                        party.UpdatedBy = _tenantContext.UserId;

                        _context.ContractParties.Add(party);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} created successfully for tenant {TenantId}", contract.Id, _tenantContext.TenantId);

                return await GetContractByIdAsync(contract.Id) ?? throw new InvalidOperationException("Failed to retrieve created contract");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractDto?> UpdateContractAsync(Guid id, UpdateContractDto updateDto)
        {
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);

                if (contract == null)
                {
                    return null;
                }

                // Check if contract is in an editable state
                if (!contract.Status.IsEditable())
                {
                    throw new InvalidOperationException($"Cannot edit contract in {contract.Status} status.");
                }

                // Validate contract type exists
                var contractType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Id == updateDto.ContractTypeId && ct.TenantId == _tenantContext.TenantId && ct.IsActive);

                if (contractType == null)
                {
                    throw new ArgumentException("Contract type not found or inactive.");
                }

                // Validate business rules
                if (updateDto.EndDate.HasValue && updateDto.EndDate <= updateDto.StartDate)
                {
                    throw new ArgumentException("End date must be after start date.");
                }

                // Update contract
                _mapper.Map(updateDto, contract);
                contract.UpdatedAt = DateTime.UtcNow;
                contract.UpdatedBy = _tenantContext.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} updated successfully for tenant {TenantId}", id, _tenantContext.TenantId);

                return await GetContractByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract {ContractId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<bool> DeleteContractAsync(Guid id)
        {
            try
            {
                var contract = await _context.Contracts
                    .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);

                if (contract == null)
                {
                    return false;
                }

                // Check if contract can be deleted
                if (!contract.Status.IsEditable())
                {
                    throw new InvalidOperationException($"Cannot delete contract in {contract.Status} status.");
                }

                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} deleted successfully for tenant {TenantId}", id, _tenantContext.TenantId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contract {ContractId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractDto?> ChangeContractStatusAsync(Guid id, ContractStatusChangeDto statusChangeDto)
        {
            try
            {
                var contract = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);

                if (contract == null)
                {
                    return null;
                }

                if (!Enum.TryParse<ContractStatus>(statusChangeDto.NewStatus, true, out var newStatus))
                {
                    throw new ArgumentException("Invalid contract status.");
                }

                // Validate status transition
                var validNextStatuses = contract.Status.GetValidNextStatuses();
                if (!validNextStatuses.Contains(newStatus))
                {
                    throw new ArgumentException($"Cannot change status from {contract.Status} to {newStatus}.");
                }

                var previousStatus = contract.Status;
                contract.Status = newStatus;
                contract.LastStatusChangeDate = DateTime.UtcNow;
                contract.LastStatusChangedById = _tenantContext.UserId;
                contract.StatusChangeReason = statusChangeDto.Reason;
                contract.UpdatedAt = DateTime.UtcNow;
                contract.UpdatedBy = _tenantContext.UserId;

                // Handle specific status changes
                switch (newStatus)
                {
                    case ContractStatus.Active:
                        if (!contract.SignedDate.HasValue)
                        {
                            contract.SignedDate = DateTime.UtcNow;
                        }
                        break;
                    case ContractStatus.Approved:
                        contract.ApprovedById = _tenantContext.UserId;
                        contract.ApprovedDate = DateTime.UtcNow;
                        contract.ApprovalNotes = statusChangeDto.Notes;
                        break;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} status changed from {PreviousStatus} to {NewStatus} for tenant {TenantId}", 
                    id, previousStatus, newStatus, _tenantContext.TenantId);

                return await GetContractByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status for contract {ContractId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<List<ContractDto>> GetExpiringContractsAsync(int daysAhead = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.Date.AddDays(daysAhead);

                var contracts = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .Where(c => c.TenantId == _tenantContext.TenantId &&
                               c.EndDate.HasValue &&
                               c.EndDate.Value.Date <= cutoffDate &&
                               c.EndDate.Value.Date >= DateTime.UtcNow.Date &&
                               c.Status == ContractStatus.Active)
                    .OrderBy(c => c.EndDate)
                    .ToListAsync();

                var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
                await PopulateUserNamesAsync(contractDtos);

                return contractDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring contracts for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<List<ContractDto>> GetContractsByTypeAsync(Guid contractTypeId)
        {
            try
            {
                var contracts = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .Where(c => c.TenantId == _tenantContext.TenantId && c.ContractTypeId == contractTypeId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
                await PopulateUserNamesAsync(contractDtos);

                return contractDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts by type {ContractTypeId} for tenant {TenantId}", contractTypeId, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<List<ContractDto>> GetContractsByStatusAsync(string status)
        {
            try
            {
                if (!Enum.TryParse<ContractStatus>(status, true, out var statusEnum))
                {
                    throw new ArgumentException("Invalid contract status.");
                }

                var contracts = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .Where(c => c.TenantId == _tenantContext.TenantId && c.Status == statusEnum)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
                await PopulateUserNamesAsync(contractDtos);

                return contractDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts by status {Status} for tenant {TenantId}", status, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractDto?> RenewContractAsync(Guid id, CreateContractDto renewalDto)
        {
            try
            {
                var originalContract = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties)
                    .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == _tenantContext.TenantId);

                if (originalContract == null)
                {
                    return null;
                }

                if (originalContract.Status != ContractStatus.Active && originalContract.Status != ContractStatus.Expired)
                {
                    throw new InvalidOperationException($"Cannot renew contract in {originalContract.Status} status.");
                }

                // Create renewal contract
                renewalDto.ParentContractId = originalContract.Id;
                var renewedContract = await CreateContractAsync(renewalDto);

                // Update original contract status
                originalContract.Status = ContractStatus.Renewed;
                originalContract.UpdatedAt = DateTime.UtcNow;
                originalContract.UpdatedBy = _tenantContext.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} renewed successfully for tenant {TenantId}. New contract: {NewContractId}", 
                    id, _tenantContext.TenantId, renewedContract.Id);

                return renewedContract;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing contract {ContractId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        private async Task<string> GenerateContractNumberAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var prefix = $"CT-{currentYear}-";

            var lastContract = await _context.Contracts
                .Where(c => c.TenantId == _tenantContext.TenantId && c.ContractNumber.StartsWith(prefix))
                .OrderByDescending(c => c.ContractNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastContract != null)
            {
                var lastNumberPart = lastContract.ContractNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberPart, out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }

        private async Task PopulateUserNamesAsync(List<ContractDto> contracts)
        {
            // In a real implementation, you would call a user service here to get user names
            // For now, we'll just leave them as null or set placeholder values
            await Task.CompletedTask;

            foreach (var contract in contracts)
            {
                // Placeholder implementation - in real app, fetch from user service
                contract.OwnerName = contract.OwnerId?.ToString();
                contract.ApprovedByName = contract.ApprovedById?.ToString();
                contract.LastStatusChangedByName = contract.LastStatusChangedById?.ToString();
            }
        }
    }
}