namespace Library_API.Exceptions.Auth;

public sealed class RegistrationFailedException(IEnumerable<string> errors)
    : ApiException("User registration failed.")
{
    public IReadOnlyList<string> Errors { get; } = errors.ToArray();

    public override int StatusCode => StatusCodes.Status400BadRequest;
}