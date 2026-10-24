namespace Library_API.Exceptions.User
{
    public class UserNotFoundException : EntityNotFoundException
    {
        public UserNotFoundException(int id) : base($"User with id {id} wasn't found")
        {
        }
    }
}
