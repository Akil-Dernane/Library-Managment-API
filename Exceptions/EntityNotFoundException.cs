namespace Library_API.Exceptions
{
    public class EntityNotFoundException : ApiException
    {
        protected EntityNotFoundException(string message)
           : base(message)
        {
        }
        public override int StatusCode => StatusCodes.Status404NotFound;
    }
}
