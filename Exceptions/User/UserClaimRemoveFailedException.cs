using Library_API.Exceptions.Interfaces;

namespace Library_API.Exceptions.User
{
    public class UserClaimRemoveFailedException : ApiException, IHasErrors
    {
        public IReadOnlyList<string> Errors { get; }
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public UserClaimRemoveFailedException(IEnumerable<string> errors) : base("Failed to remove claim from user.")
        {
            Errors = errors.ToArray();
        }
    }
}
