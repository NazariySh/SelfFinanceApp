using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/financial_report")]
    public class FinancialReportController(IFinancialReportService financialReportService) : ControllerBase
    {
        private readonly IFinancialReportService _financialReportService = financialReportService ?? throw new ArgumentNullException(nameof(financialReportService));

        [HttpGet("date")]
        public async Task<IActionResult> GetDailyReport(DateOnly date, CancellationToken cancellationToken = default)
        {
            var accountId = GetAccountIdentifier();

            var result = await _financialReportService.GetDailyReportAsync(date, accountId, cancellationToken);

            return Ok(result);
        }

        [HttpGet("date_period")]
        public async Task<IActionResult> GetDatePeriodReport(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            var accountId = GetAccountIdentifier();

            var result = await _financialReportService.GetDatePeriodReportAsync(startDate, endDate, accountId, cancellationToken);

            return Ok(result);
        }

        private string GetAccountIdentifier()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }
    }
}
