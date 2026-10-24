namespace Library_API.Exceptions.Auth
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Invalid email or password.")
        {
        }
    }
}
