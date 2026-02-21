using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class BeeFiApiClient : IBeeFiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly BeeFiApiSettings _settings;
    private readonly ILogger<BeeFiApiClient> _logger;

    public BeeFiApiClient(
        HttpClient httpClient,
        IOptions<BeeFiApiSettings> settings,
        ILogger<BeeFiApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", _settings.ApiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
    }

    public async Task<BeeFiCustomerDto?> GetCustomerByContractAsync(string contractNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/by-contract/{contractNumber}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to get BeeFi customer by contract {ContractNumber}. Status: {StatusCode}",
                    contractNumber,
                    response.StatusCode);
                return null;
            }

            var customer = await response.Content.ReadFromJsonAsync<BeeFiCustomerDto>();
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error calling BeeFi API for contract {ContractNumber}",
                contractNumber);
            return null;
        }
    }

    public async Task<BeeFiCustomerDto?> GetCustomerByIdAsync(string customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/{customerId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to get BeeFi customer {CustomerId}. Status: {StatusCode}",
                    customerId,
                    response.StatusCode);
                return null;
            }

            var customer = await response.Content.ReadFromJsonAsync<BeeFiCustomerDto>();
            return customer;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error calling BeeFi API for customer {CustomerId}",
                customerId);
            return null;
        }
    }

    public async Task<bool> IsSubscriptionActiveAsync(string customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/{customerId}/subscription/status");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var status = await response.Content.ReadFromJsonAsync<SubscriptionStatusDto>();
            return status?.IsActive ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking subscription status for customer {CustomerId}",
                customerId);
            return false;
        }
    }

    public async Task<BeeFiPlanDto?> GetCustomerPlanAsync(string customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/customers/{customerId}/plan");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var plan = await response.Content.ReadFromJsonAsync<BeeFiPlanDto>();
            return plan;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting plan for customer {CustomerId}",
                customerId);
            return null;
        }
    }
}