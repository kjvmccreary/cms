using ContractService.DTOs;
using Shared.DTOs; // ← ADD THIS LINE

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
        Task<List<ContractDto>> GetContractsByStatusAsync(string status);
        Task<ContractDto?> RenewContractAsync(Guid id, CreateContractDto renewalDto);
    }
}