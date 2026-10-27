namespace Library_API.Exceptions.Auth
{
    public class InvalidCredentialsException : ApiException
    {
        public override int StatusCode => StatusCodes.Status401Unauthorized;
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }
    }
}
