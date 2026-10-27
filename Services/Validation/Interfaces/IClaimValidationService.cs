namespace Library_API.Services.Validation.Interfaces
{
    public interface IClaimValidationService
    {
        void Validate(ClaimRequest dto, IReadOnlySet<string> allowedClaimTypes);
    }
}
