using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractService.Data;
using ContractService.Models;
using ContractService.DTOs;
using ContractService.Services;

namespace ContractService.Services
{
    public class ContractTypeService : IContractTypeService
    {
        private readonly ContractDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<ContractTypeService> _logger;

        public ContractTypeService(
            ContractDbContext context,
            IMapper mapper,
            ITenantContext tenantContext,
            ILogger<ContractTypeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task<List<ContractTypeDto>> GetContractTypesAsync()
        {
            try
            {
                var contractTypes = await _context.ContractTypes
                    .Where(ct => ct.TenantId == _tenantContext.TenantId && ct.IsActive)
                    .OrderBy(ct => ct.Name)
                    .ToListAsync();

                return _mapper.Map<List<ContractTypeDto>>(contractTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract types for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractTypeDto?> GetContractTypeByIdAsync(Guid id)
        {
            try
            {
                var contractType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id && ct.TenantId == _tenantContext.TenantId);

                return contractType != null ? _mapper.Map<ContractTypeDto>(contractType) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract type {ContractTypeId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractTypeDto> CreateContractTypeAsync(CreateContractTypeDto createDto)
        {
            try
            {
                // Check if contract type with same name already exists
                var existingType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == createDto.Name.ToLower() && 
                                              ct.TenantId == _tenantContext.TenantId);

                if (existingType != null)
                {
                    throw new ArgumentException("A contract type with this name already exists.");
                }

                var contractType = _mapper.Map<ContractType>(createDto);
                contractType.Id = Guid.NewGuid();
                contractType.TenantId = _tenantContext.TenantId;
                contractType.CreatedBy = _tenantContext.UserId;
                contractType.CreatedAt = DateTime.UtcNow;
                contractType.UpdatedAt = DateTime.UtcNow;
                contractType.IsActive = true;

                _context.ContractTypes.Add(contractType);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract type {ContractTypeId} created for tenant {TenantId}", contractType.Id, _tenantContext.TenantId);

                return _mapper.Map<ContractTypeDto>(contractType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract type for tenant {TenantId}", _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<ContractTypeDto?> UpdateContractTypeAsync(Guid id, CreateContractTypeDto updateDto)
        {
            try
            {
                var contractType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id && ct.TenantId == _tenantContext.TenantId);

                if (contractType == null)
                {
                    return null;
                }

                // Check if another contract type with same name already exists
                var existingType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Name.ToLower() == updateDto.Name.ToLower() && 
                                              ct.TenantId == _tenantContext.TenantId &&
                                              ct.Id != id);

                if (existingType != null)
                {
                    throw new ArgumentException("A contract type with this name already exists.");
                }

                _mapper.Map(updateDto, contractType);
                contractType.UpdatedAt = DateTime.UtcNow;
                contractType.UpdatedBy = _tenantContext.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract type {ContractTypeId} updated for tenant {TenantId}", id, _tenantContext.TenantId);

                return _mapper.Map<ContractTypeDto>(contractType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract type {ContractTypeId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }

        public async Task<bool> DeleteContractTypeAsync(Guid id)
        {
            try
            {
                var contractType = await _context.ContractTypes
                    .FirstOrDefaultAsync(ct => ct.Id == id && ct.TenantId == _tenantContext.TenantId);

                if (contractType == null)
                {
                    return false;
                }

                // Check if there are active contracts using this type
                var hasActiveContracts = await _context.Contracts
                    .AnyAsync(c => c.ContractTypeId == id && c.TenantId == _tenantContext.TenantId && c.IsActive);

                if (hasActiveContracts)
                {
                    throw new InvalidOperationException("Cannot delete contract type that is being used by active contracts.");
                }

                // Soft delete
                contractType.IsActive = false;
                contractType.UpdatedAt = DateTime.UtcNow;
                contractType.UpdatedBy = _tenantContext.UserId;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Contract type {ContractTypeId} deleted for tenant {TenantId}", id, _tenantContext.TenantId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contract type {ContractTypeId} for tenant {TenantId}", id, _tenantContext.TenantId);
                throw;
            }
        }
    }
}