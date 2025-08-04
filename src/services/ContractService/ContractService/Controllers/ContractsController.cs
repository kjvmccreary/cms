using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContractService.Services;
using ContractService.DTOs;
using ContractService.Models;

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
            [FromQuery] string? status = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _contractService.GetContractsAsync(page, pageSize, search, status);
                return Ok(result);
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
                {
                    return NotFound($"Contract with ID {id} not found");
                }
                    
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var contract = await _contractService.UpdateContractAsync(id, updateDto);
                
                if (contract == null)
                {
                    return NotFound($"Contract with ID {id} not found");
                }
                    
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
        /// Delete a contract (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(Guid id)
        {
            try
            {
                var success = await _contractService.DeleteContractAsync(id);
                
                if (!success)
                {
                    return NotFound($"Contract with ID {id} not found");
                }
                    
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var contract = await _contractService.ChangeContractStatusAsync(id, statusDto);
                
                if (contract == null)
                {
                    return NotFound($"Contract with ID {id} not found");
                }
                    
                return Ok(contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing status for contract {ContractId}", id);
                return StatusCode(500, "An error occurred while changing the contract status");
            }
        }

        /// <summary>
        /// Get contracts expiring within specified days
        /// </summary>
        [HttpGet("expiring")]
        public async Task<ActionResult<List<ContractDto>>> GetExpiringContracts([FromQuery] int daysAhead = 30)
        {
            try
            {
                if (daysAhead < 1 || daysAhead > 365) daysAhead = 30;

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
        /// Get contracts by contract type
        /// </summary>
        [HttpGet("by-type/{contractTypeId}")]
        public async Task<ActionResult<List<ContractDto>>> GetContractsByType(Guid contractTypeId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByTypeAsync(contractTypeId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts by type {ContractTypeId}", contractTypeId);
                return StatusCode(500, "An error occurred while retrieving contracts by type");
            }
        }

        /// <summary>
        /// Get all active contracts
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<ContractDto>>> GetActiveContracts()
        {
            try
            {
                var contracts = await _contractService.GetActiveContractsAsync();
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active contracts");
                return StatusCode(500, "An error occurred while retrieving active contracts");
            }
        }

        /// <summary>
        /// Get contract dashboard statistics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStats()
        {
            try
            {
                var totalContracts = await _contractService.GetContractsAsync(1, 1);
                var activeContracts = await _contractService.GetActiveContractsAsync();
                var expiringContracts = await _contractService.GetExpiringContractsAsync(30);
                
                var stats = new
                {
                    TotalContracts = totalContracts.TotalCount,
                    ActiveContracts = activeContracts.Count,
                    ExpiringContracts = expiringContracts.Count,
                    ExpiringIn7Days = expiringContracts.Count(c => c.DaysUntilExpiration <= 7),
                    ExpiringIn30Days = expiringContracts.Count(c => c.DaysUntilExpiration <= 30)
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, "An error occurred while retrieving dashboard statistics");
            }
        }

        /// <summary>
        /// Get contract status workflow options
        /// </summary>
        [HttpGet("{id}/status-options")]
        public async Task<ActionResult<object>> GetStatusOptions(Guid id)
        {
            try
            {
                var contract = await _contractService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    return NotFound($"Contract with ID {id} not found");
                }

                if (!Enum.TryParse<ContractStatus>(contract.Status, out var currentStatus))
                {
                    return BadRequest("Invalid current contract status");
                }

                var validNextStatuses = currentStatus.GetValidNextStatuses();
                var statusOptions = validNextStatuses.Select(status => new
                {
                    Value = status.ToString(),
                    Display = status.GetDescription(),
                    Color = status.GetStatusColor()
                }).ToList();

                return Ok(new
                {
                    CurrentStatus = new
                    {
                        Value = currentStatus.ToString(),
                        Display = currentStatus.GetDescription(),
                        Color = currentStatus.GetStatusColor()
                    },
                    ValidNextStatuses = statusOptions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving status options for contract {ContractId}", id);
                return StatusCode(500, "An error occurred while retrieving status options");
            }
        }
    }
}