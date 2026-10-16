namespace Library_API.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        protected EntityNotFoundException(string message)
           : base(message)
        {
        }
    }
}
