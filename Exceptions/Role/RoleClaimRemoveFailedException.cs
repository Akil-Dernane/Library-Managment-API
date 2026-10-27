using Library_API.Exceptions.Interfaces;

namespace Library_API.Exceptions.Role
{
    public class RoleClaimRemoveFailedException : ApiException, IHasErrors
    {
        public IReadOnlyList<string> Errors { get; }
        public override int StatusCode => StatusCodes.Status500InternalServerError;

        public RoleClaimRemoveFailedException(IEnumerable<string> errors)
            : base("Failed to remove claim to role.")
        {
            Errors = errors.ToArray();
        }
    }
}
