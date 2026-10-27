namespace Library_API.Exceptions.Claim
{
    public class InvalidClaimException : ApiException
    {
        public override int StatusCode =>  StatusCodes.Status400BadRequest;

        public InvalidClaimException(string claimName) : base($"Invalid claim {claimName}")
        {
        }
    }
}
