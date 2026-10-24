namespace Library_API.Exceptions.Author
{
    public class AuthorNotFoundException : EntityNotFoundException
    {
        public AuthorNotFoundException(int id) : base($"Author with id {id} wasn't found")
        {
        }
    }
}
