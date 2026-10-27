namespace Library_API.Exceptions.Permission
{
    public class InvalidPermissionException : ApiException
    {
        public override int StatusCode => StatusCodes.Status400BadRequest;

        public InvalidPermissionException(string perrmisionName) : base($"Invalid permission {perrmisionName}")
        {
        }
    }
}
