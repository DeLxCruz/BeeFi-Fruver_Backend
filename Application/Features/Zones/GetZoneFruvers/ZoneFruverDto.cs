namespace Application.Features.Zones.GetZoneFruvers;

public record ZoneFruverDto(
    Guid FruverId,
    string FruverName,
    string FruverEmail,
    DateTime AssignedAt
);
