namespace Library_API.Exceptions.Role
{
    public class RoleNotFoundException : EntityNotFoundException
    {
        public RoleNotFoundException(string role) : base($"{role} was not found")
        {
        }
    }
}
