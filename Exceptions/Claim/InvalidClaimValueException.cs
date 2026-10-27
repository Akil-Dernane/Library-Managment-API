namespace Library_API.Exceptions.Claim;

public class InvalidClaimValueException : ApiException
{
    public override int StatusCode => StatusCodes.Status400BadRequest;

    public InvalidClaimValueException(string claimType, string claimValue)
        : base($"Invalid value '{claimValue}' for claim type '{claimType}'.")
    {
    }
}