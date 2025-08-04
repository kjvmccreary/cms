using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContractService.Services;
using ContractService.DTOs;

namespace ContractService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize]
    public class ContractTypesController : ControllerBase
    {
        private readonly IContractTypeService _contractTypeService;
        private readonly ILogger<ContractTypesController> _logger;

        public ContractTypesController(
            IContractTypeService contractTypeService,
            ILogger<ContractTypesController> logger)
        {
            _contractTypeService = contractTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all contract types
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ContractTypeDto>>> GetContractTypes()
        {
            try
            {
                var contractTypes = await _contractTypeService.GetContractTypesAsync();
                return Ok(contractTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract types");
                return StatusCode(500, "An error occurred while retrieving contract types");
            }
        }

        /// <summary>
        /// Get a specific contract type by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractTypeDto>> GetContractType(Guid id)
        {
            try
            {
                var contractType = await _contractTypeService.GetContractTypeByIdAsync(id);
                
                if (contractType == null)
                {
                    return NotFound($"Contract type with ID {id} not found");
                }
                    
                return Ok(contractType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract type {ContractTypeId}", id);
                return StatusCode(500, "An error occurred while retrieving the contract type");
            }
        }

        /// <summary>
        /// Create a new contract type
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ContractTypeDto>> CreateContractType(CreateContractTypeDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var contractType = await _contractTypeService.CreateContractTypeAsync(createDto);
                return CreatedAtAction(nameof(GetContractType), new { id = contractType.Id }, contractType);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract type");
                return StatusCode(500, "An error occurred while creating the contract type");
            }
        }

        /// <summary>
        /// Update an existing contract type
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ContractTypeDto>> UpdateContractType(Guid id, CreateContractTypeDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var contractType = await _contractTypeService.UpdateContractTypeAsync(id, updateDto);
                
                if (contractType == null)
                {
                    return NotFound($"Contract type with ID {id} not found");
                }
                    
                return Ok(contractType);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract type {ContractTypeId}", id);
                return StatusCode(500, "An error occurred while updating the contract type");
            }
        }

        /// <summary>
        /// Delete a contract type (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContractType(Guid id)
        {
            try
            {
                var success = await _contractTypeService.DeleteContractTypeAsync(id);
                
                if (!success)
                {
                    return NotFound($"Contract type with ID {id} not found");
                }
                    
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contract type {ContractTypeId}", id);
                return StatusCode(500, "An error occurred while deleting the contract type");
            }
        }
    }
}