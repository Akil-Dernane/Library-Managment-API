using Library_API.Exceptions.Interfaces;

namespace Library_API.Exceptions.Auth;

public sealed class RegistrationFailedException : ApiException, IHasErrors
{
    public IReadOnlyList<string> Errors { get; }
    public override int StatusCode => StatusCodes.Status400BadRequest;
    public RegistrationFailedException(IEnumerable<string> errors) 
        : base("User registration failed.")
    {
        Errors = errors.ToArray();
    }
}