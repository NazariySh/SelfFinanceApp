using Application.Interfaces;
using Shared.Dtos;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/financial_operations")]
    public class FinancialOperationController(IFinancialOperationService financialOperationService) : ControllerBase
    {
        private readonly IFinancialOperationService _financialOperationService = financialOperationService ?? throw new ArgumentNullException(nameof(financialOperationService));

        [HttpPost]
        public async Task<IActionResult> CreateOperation([FromBody] FinancialOperationForCreationDto operationDto, CancellationToken cancellationToken = default)
        {
            var userId = GetAccountIdentifier();

            await _financialOperationService.CreateAsync(operationDto, userId, cancellationToken);

            return CreatedAtAction(nameof(GetOperationById), new { id = operationDto.Id }, operationDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOperation(Guid id, [FromBody] FinancialOperationForUpdateDto operationDto, CancellationToken cancellationToken = default)
        {
            var userId = GetAccountIdentifier();

            await _financialOperationService.UpdateAsync(id, operationDto, userId, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOperation(Guid id, CancellationToken cancellationToken = default)
        {
            var userId = GetAccountIdentifier();

            await _financialOperationService.DeleteAsync(id, userId, cancellationToken);

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOperationById(Guid id, CancellationToken cancellationToken = default)
        {
            var userId = GetAccountIdentifier();

            var operation = await _financialOperationService.GetByIdAsync(id, userId, cancellationToken);

            return operation is null ? NotFound() : Ok(operation);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOperations([FromQuery] PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            var userId = GetAccountIdentifier();

            var operations = await _financialOperationService.GetAllAsync(userId, parameters, cancellationToken);

            return Ok(operations);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/account/{userId}/operations/{id}")]
        public async Task<IActionResult> GetUserOperationById(string userId, Guid id, CancellationToken cancellationToken = default)
        {
            var operation = await _financialOperationService.GetByIdAsync(id, userId, cancellationToken);

            return operation is null ? NotFound() : Ok(operation);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/account/{userId}/operations")]
        public async Task<IActionResult> GetAllUserOperations(string userId, [FromQuery] PaginationParameters parameters, CancellationToken cancellationToken = default)
        {
            var operations = await _financialOperationService.GetAllAsync(userId, parameters, cancellationToken);

            return Ok(operations);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/account/{userId}/operations/{id}")]
        public async Task<IActionResult> UpdateUserOperation(string userId, Guid id, [FromBody] FinancialOperationForUpdateDto operationDto, CancellationToken cancellationToken = default)
        {
            await _financialOperationService.UpdateAsync(id, operationDto, userId, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/account/{userId}/operations/{id}")]
        public async Task<IActionResult> DeleteUserOperation(string userId, Guid id, CancellationToken cancellationToken = default)
        {
            await _financialOperationService.DeleteAsync(id, userId, cancellationToken);

            return Ok();
        }

        private string GetAccountIdentifier()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }
    }
}
