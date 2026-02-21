namespace Application.Common.Models;

public record BeeFiCustomerDto(
    string Id,
    string ContractNumber,
    string Email,
    string FullName,
    int PlanId,
    string PlanName,
    bool IsActive
);
