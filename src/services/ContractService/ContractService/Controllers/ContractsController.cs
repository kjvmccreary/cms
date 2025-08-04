using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContractService.Services;
using ContractService.DTOs;
using ContractService.Models;
using Shared.DTOs; // ← ADD THIS LINE

namespace ContractService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(
            IContractService contractService,
            ILogger<ContractsController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        /// <summary>
        /// Get contracts with pagination, search, and filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<ContractDto>>> GetContracts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? contractTypeId = null,
            [FromQuery] string? department = null)
        {
            try
            {
                var contracts = await _contractService.GetContractsAsync(page, pageSize, search, status);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts");
                return StatusCode(500, "An error occurred while retrieving contracts");
            }
        }

        /// <summary>
        /// Get a specific contract by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetContract(Guid id)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                    return NotFound();

                return Ok(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract {ContractId}", id);
                return StatusCode(500, "An error occurred while retrieving the contract");
            }
        }

        /// <summary>
        /// Create a new contract
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContractDto>> CreateContract(CreateContractDto createDto)
        {
            try
            {
                var contract = await _contractService.CreateContractAsync(createDto);
                return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                return StatusCode(500, "An error occurred while creating the contract");
            }
        }

        /// <summary>
        /// Update an existing contract
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ContractDto>> UpdateContract(Guid id, UpdateContractDto updateDto)
        {
            try
            {
                var contract = await _contractService.UpdateContractAsync(id, updateDto);
                if (contract == null)
                    return NotFound();

                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract {ContractId}", id);
                return StatusCode(500, "An error occurred while updating the contract");
            }
        }

        /// <summary>
        /// Delete a contract
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(Guid id)
        {
            try
            {
                var deleted = await _contractService.DeleteContractAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contract {ContractId}", id);
                return StatusCode(500, "An error occurred while deleting the contract");
            }
        }

        /// <summary>
        /// Change contract status
        /// </summary>
        [HttpPost("{id}/status")]
        public async Task<ActionResult<ContractDto>> ChangeStatus(Guid id, ContractStatusChangeDto statusDto)
        {
            try
            {
                var contract = await _contractService.ChangeContractStatusAsync(id, statusDto);
                if (contract == null)
                    return NotFound();

                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing contract status for {ContractId}", id);
                return StatusCode(500, "An error occurred while changing the contract status");
            }
        }

        /// <summary>
        /// Get contracts expiring soon
        /// </summary>
        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetExpiringContracts([FromQuery] int daysAhead = 30)
        {
            try
            {
                var contracts = await _contractService.GetExpiringContractsAsync(daysAhead);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring contracts");
                return StatusCode(500, "An error occurred while retrieving expiring contracts");
            }
        }

        /// <summary>
        /// Get contracts by type
        /// </summary>
        [HttpGet("by-type/{contractTypeId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetContractsByType(Guid contractTypeId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByTypeAsync(contractTypeId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts by type {ContractTypeId}", contractTypeId);
                return StatusCode(500, "An error occurred while retrieving contracts");
            }
        }

        /// <summary>
        /// Renew a contract
        /// </summary>
        [HttpPost("{id}/renew")]
        public async Task<ActionResult<ContractDto>> RenewContract(Guid id, CreateContractDto renewalDto)
        {
            try
            {
                var contract = await _contractService.RenewContractAsync(id, renewalDto);
                if (contract == null)
                    return NotFound();

                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error renewing contract {ContractId}", id);
                return StatusCode(500, "An error occurred while renewing the contract");
            }
        }
    }
}