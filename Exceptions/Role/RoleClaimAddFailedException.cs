using Library_API.Exceptions.Interfaces;

namespace Library_API.Exceptions.Role
{
    public class RoleClaimAddFailedException : ApiException, IHasErrors
    {
        public IReadOnlyList<string> Errors { get; }
        public override int StatusCode =>   StatusCodes.Status500InternalServerError;

        public RoleClaimAddFailedException(IEnumerable<string> errors)
            : base("Failed to add claim to role.")
        {
            Errors = errors.ToArray();
        }
    }
}
