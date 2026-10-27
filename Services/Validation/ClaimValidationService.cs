using Library_API.Exceptions.Claim;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation.Interfaces;

namespace Library_API.Services.Validation;

public class ClaimValidationService : IClaimValidationService
{
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ClaimValidationService> _logger;

    public ClaimValidationService(
        ICurrentUserService currentUser,
        ILogger<ClaimValidationService> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public void Validate(
        ClaimRequest dto,
        IReadOnlySet<string> allowedClaimTypes)
    {
        //validating the dto.type
        if (!allowedClaimTypes.Contains(dto.Type))
        {
            _logger.LogWarning(
                "User {ActorId} attempted to use invalid claim type {ClaimType}",
                _currentUser.UserId,
                dto.Type);

            throw new InvalidClaimException(dto.Type);
        }

        //validating the dto.value depending on the dto.type
        bool isValid = dto.Type switch
        {
            AppClaims.Permission =>
                AppPermissions.All.Contains(dto.Value),

            _ => false
        };

        if (!isValid)
        {
            _logger.LogWarning(
                "User {ActorId} supplied an invalid value for claim type {ClaimType}",
                _currentUser.UserId,
                dto.Type);

            throw new InvalidClaimValueException(dto.Type, dto.Value);
        }
    }
}