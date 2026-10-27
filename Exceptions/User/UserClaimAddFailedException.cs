using Library_API.Exceptions.Interfaces;

namespace Library_API.Exceptions.User
{
    public class UserClaimAddFailedException : ApiException, IHasErrors
    {
        public IReadOnlyList<string> Errors { get; }
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public UserClaimAddFailedException(IEnumerable<string> errors) : base("Failed to add claim to user.")
        {
            Errors = errors.ToArray();
        }
    }
}
