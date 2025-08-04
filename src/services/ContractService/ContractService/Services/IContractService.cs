using ContractService.DTOs;

namespace ContractService.Services
{
    public interface IContractService
    {
        Task<PagedResult<ContractDto>> GetContractsAsync(int page = 1, int pageSize = 10, string? search = null, string? status = null);
        Task<ContractDto?> GetContractByIdAsync(Guid id);
        Task<ContractDto> CreateContractAsync(CreateContractDto createDto);
        Task<ContractDto?> UpdateContractAsync(Guid id, UpdateContractDto updateDto);
        Task<bool> DeleteContractAsync(Guid id);
        Task<ContractDto?> ChangeContractStatusAsync(Guid id, ContractStatusChangeDto statusChangeDto);
        Task<List<ContractDto>> GetExpiringContractsAsync(int daysAhead = 30);
        Task<List<ContractDto>> GetContractsByTypeAsync(Guid contractTypeId);
        Task<List<ContractDto>> GetActiveContractsAsync();
    }

    public interface IContractTypeService
    {
        Task<List<ContractTypeDto>> GetContractTypesAsync();
        Task<ContractTypeDto?> GetContractTypeByIdAsync(Guid id);
        Task<ContractTypeDto> CreateContractTypeAsync(CreateContractTypeDto createDto);
        Task<ContractTypeDto?> UpdateContractTypeAsync(Guid id, CreateContractTypeDto updateDto);
        Task<bool> DeleteContractTypeAsync(Guid id);
    }
}