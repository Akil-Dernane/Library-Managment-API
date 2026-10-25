namespace Library_API.Exceptions.User
{
    public class LastAdminException : ConflictException
    {
        public LastAdminException() : base("Can't delete the last Admin")
        {
        }
    }
}
