using Application.Interfaces;
using Shared.Models;
using System.Net.Http.Json;

namespace Application.Services
{
    public class FinancialReportService(
        IProtectedHttpClientProvider protectedHttpClientProvider,
        IErrorResponseHandler errorResponseHandler)
        : IFinancialReportService
    {
        private const string _basePath = "api/financial_report";
        private readonly IProtectedHttpClientProvider _protectedHttpClientProvider = protectedHttpClientProvider ?? throw new ArgumentNullException(nameof(protectedHttpClientProvider));
        private readonly IErrorResponseHandler _errorResponseHandler = errorResponseHandler ?? throw new ArgumentNullException(nameof(errorResponseHandler));

        public async Task<DailyReport?> GetDailyReportAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            ValidateDate(date);

            using var response = await _protectedHttpClientProvider.GetAsync($"{_basePath}/date?date={FormatDate(date)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<DailyReport>(cancellationToken);
        }

        public async Task<DatePeriodReport?> GetDatePeriodReportAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            ValidateDate(startDate);

            ValidateDate(endDate);

            if (startDate > endDate)
            {
                throw new ArgumentException("The start date cannot be greater than the end date.", nameof(startDate));
            }

            using var response = await _protectedHttpClientProvider.GetAsync($"{_basePath}/date_period?startDate={FormatDate(startDate)}&endDate={FormatDate(endDate)}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await _errorResponseHandler.HandleResponseAsync(response, cancellationToken);
            }

            return await response.Content.ReadFromJsonAsync<DatePeriodReport>(cancellationToken);
        }

        private static void ValidateDate(DateOnly date)
        {
            if (date == default)
            {
                throw new ArgumentException("The date cannot be empty.", nameof(date));
            }

            if (date < new DateOnly(1991, 1, 1))
            {
                throw new ArgumentException("The date cannot be earlier than 1991-01-01.", nameof(date));
            }

            if (date > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("The date cannot be in the future.", nameof(date));
            }
        }

        private static string FormatDate(DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
