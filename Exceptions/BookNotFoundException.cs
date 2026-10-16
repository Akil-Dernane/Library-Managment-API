namespace Library_API.Exceptions
{
    public class BookNotFoundException : EntityNotFoundException
    {
        public BookNotFoundException(int id) : base($"Book with id {id} wasn't found")
        {
            
        }
    }
}
