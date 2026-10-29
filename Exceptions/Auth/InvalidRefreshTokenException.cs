namespace Library_API.Exceptions.Auth
{
    public class InvalidRefreshTokenException() 
        : ApiException("The refresh token is invalid, expired, or has been revoked")
    {
        public override int StatusCode => StatusCodes.Status401Unauthorized;
    }
}
