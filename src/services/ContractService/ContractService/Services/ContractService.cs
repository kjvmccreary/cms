using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractService.Data;
using ContractService.Models;
using ContractService.DTOs;
using ContractService.Services;

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
                    search = search.ToLower();
                    query = query.Where(c => 
                        c.Title.ToLower().Contains(search) ||
                        c.ContractNumber.ToLower().Contains(search) ||
                        c.Description != null && c.Description.ToLower().Contains(search) ||
                        c.Department != null && c.Department.ToLower().Contains(search) ||
                        c.ProjectCode != null && c.ProjectCode.ToLower().Contains(search) ||
                        c.Tags != null && c.Tags.ToLower().Contains(search));
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ContractStatus>(status, true, out var statusEnum))
                {
                    query = query.Where(c => c.Status == statusEnum);
                }

                // Get total count
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
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
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

                // Validate parent contract if specified
                if (createDto.ParentContractId.HasValue)
                {
                    var parentExists = await _context.Contracts
                        .AnyAsync(c => c.Id == createDto.ParentContractId && c.TenantId == _tenantContext.TenantId);
                    if (!parentExists)
                    {
                        throw new ArgumentException("Parent contract not found.");
                    }
                }

                // Create contract entity
                var contract = _mapper.Map<Contract>(createDto);
                contract.Id = Guid.NewGuid();
                contract.ContractNumber = await GenerateContractNumberAsync();
                contract.Status = ContractStatus.Draft;
                contract.LastStatusChangeDate = DateTime.UtcNow;
                contract.TenantId = _tenantContext.TenantId;
                contract.CreatedBy = _tenantContext.UserId;
                contract.CreatedAt = DateTime.UtcNow;
                contract.UpdatedAt = DateTime.UtcNow;

                // Set default values from contract type
                if (!contract.RenewalReminderDays.HasValue || contract.RenewalReminderDays == 0)
                {
                    contract.RenewalReminderDays = contractType.DefaultReminderDays;
                }

                // Add parties
                foreach (var partyDto in createDto.Parties)
                {
                    var party = _mapper.Map<ContractParty>(partyDto);
                    party.Id = Guid.NewGuid();
                    party.ContractId = contract.Id;
                    party.TenantId = _tenantContext.TenantId;
                    party.CreatedAt = DateTime.UtcNow;
                    party.UpdatedAt = DateTime.UtcNow;
                    party.CreatedBy = _tenantContext.UserId;
                    contract.Parties.Add(party);
                }

                _context.Contracts.Add(contract);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} ({ContractNumber}) created for tenant {TenantId}", 
                    contract.Id, contract.ContractNumber, _tenantContext.TenantId);

                // Return the created contract with includes
                var createdContract = await GetContractByIdAsync(contract.Id);
                return createdContract!;
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

                // Update contract properties
                _mapper.Map(updateDto, contract);
                contract.UpdatedAt = DateTime.UtcNow;
                contract.UpdatedBy = _tenantContext.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} ({ContractNumber}) updated for tenant {TenantId}", 
                    id, contract.ContractNumber, _tenantContext.TenantId);

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
                if (contract.Status.IsFinalized())
                {
                    throw new InvalidOperationException($"Cannot delete contract in {contract.Status} status.");
                }

                // Check for child contracts
                var hasChildContracts = await _context.Contracts
                    .AnyAsync(c => c.ParentContractId == id && c.TenantId == _tenantContext.TenantId);

                if (hasChildContracts)
                {
                    throw new InvalidOperationException("Cannot delete contract that has child contracts (renewals/amendments).");
                }

                // For draft contracts, we can do a hard delete
                if (contract.Status == ContractStatus.Draft)
                {
                    _context.Contracts.Remove(contract);
                }
                else
                {
                    // Soft delete for other statuses
                    contract.Status = ContractStatus.Cancelled;
                    contract.StatusChangeReason = "Contract deleted";
                    contract.LastStatusChangeDate = DateTime.UtcNow;
                    contract.LastStatusChangedById = _tenantContext.UserId;
                    contract.UpdatedAt = DateTime.UtcNow;
                    contract.UpdatedBy = _tenantContext.UserId;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract {ContractId} deleted for tenant {TenantId}", id, _tenantContext.TenantId);

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
                    throw new ArgumentException("Invalid status value.");
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
                var cutoffDate = DateTime.UtcNow.AddDays(daysAhead);

                var contracts = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties.OrderBy(p => p.SortOrder))
                    .Where(c => c.TenantId == _tenantContext.TenantId &&
                               c.Status == ContractStatus.Active &&
                               c.EndDate.HasValue &&
                               c.EndDate <= cutoffDate &&
                               c.NotificationsEnabled)
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
                    .Include(c => c.Parties.OrderBy(p => p.SortOrder))
                    .Where(c => c.TenantId == _tenantContext.TenantId &&
                               c.ContractTypeId == contractTypeId)
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

        public async Task<List<ContractDto>> GetActiveContractsAsync()
        {
            try
            {
                var contracts = await _context.Contracts
                    .Include(c => c.ContractType)
                    .Include(c => c.Parties.OrderBy(p => p.SortOrder))
                    .Where(c => c.TenantId == _tenantContext.TenantId &&
                               c.Status == ContractStatus.Active)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                var contractDtos = _mapper.Map<List<ContractDto>>(contracts);
                await PopulateUserNamesAsync(contractDtos);

                return contractDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active contracts for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        private async Task<string> GenerateContractNumberAsync()
        {
            // Get tenant info for prefix
            var tenantPrefix = "CTR"; // Default prefix
            try
            {
                // In a real implementation, you might fetch tenant details
                // For now, we'll use a simple format
                tenantPrefix = _tenantContext.TenantId.ToString("N")[..3].ToUpper();
            }
            catch
            {
                // Fallback to default
            }
            
            var year = DateTime.UtcNow.Year;
            var yearSuffix = year.ToString()[2..]; // Last 2 digits of year
            
            // Find the next sequential number for this tenant and year
            var lastContract = await _context.Contracts
                .Where(c => c.TenantId == _tenantContext.TenantId && 
                           c.ContractNumber.StartsWith($"{tenantPrefix}-{yearSuffix}"))
                .OrderByDescending(c => c.ContractNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastContract != null)
            {
                var parts = lastContract.ContractNumber.Split('-');
                if (parts.Length >= 3 && int.TryParse(parts[2], out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"{tenantPrefix}-{yearSuffix}-{nextNumber:D4}";
        }

        private async Task PopulateUserNamesAsync(List<ContractDto> contractDtos)
        {
            // In a real implementation, you would call a user service to get user names
            // For now, we'll just leave them as placeholders or implement a simple lookup
            await Task.CompletedTask;
            
            foreach (var contract in contractDtos)
            {
                contract.CreatedByName = "System User"; // Placeholder
                contract.UpdatedByName = "System User"; // Placeholder
                contract.OwnerName = contract.OwnerId.HasValue ? "Contract Owner" : null;
                contract.ApprovedByName = contract.ApprovedById.HasValue ? "Approver" : null;
                contract.LastStatusChangedByName = contract.LastStatusChangedById.HasValue ? "System User" : null;
            }
        }
    }
}