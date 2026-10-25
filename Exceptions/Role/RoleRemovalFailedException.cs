using Library_API.Exceptions.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library_API.Exceptions.Role
{
    public class RoleRemovalFailedException : ApiException , IHasErrors
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;
        public IReadOnlyList<string> Errors { get; }
        public RoleRemovalFailedException(IEnumerable<string> errors) 
            : base("Failed to remove user from role.")
        {
            Errors = errors.ToArray();
        }
    }
}
