using Application.Common.Models;

namespace Application.Common.Interfaces;

public interface IBeeFiApiClient
{
    Task<BeeFiCustomerDto?> GetCustomerByContractAsync(string contractNumber);
    Task<BeeFiCustomerDto?> GetCustomerByIdAsync(string customerId);
    Task<bool> IsSubscriptionActiveAsync(string customerId);
    Task<BeeFiPlanDto?> GetCustomerPlanAsync(string customerId);
}