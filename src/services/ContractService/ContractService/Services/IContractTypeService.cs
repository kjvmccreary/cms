using ContractService.DTOs;

namespace ContractService.Services
{
    public interface IContractTypeService
    {
        Task<List<ContractTypeDto>> GetContractTypesAsync();
        Task<ContractTypeDto?> GetContractTypeByIdAsync(Guid id);
        Task<ContractTypeDto> CreateContractTypeAsync(CreateContractTypeDto createDto);
        Task<ContractTypeDto?> UpdateContractTypeAsync(Guid id, CreateContractTypeDto updateDto);
        Task<bool> DeleteContractTypeAsync(Guid id);
    }
}