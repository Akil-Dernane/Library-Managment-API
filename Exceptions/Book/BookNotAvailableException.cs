namespace Library_API.Exceptions.Book
{
    public class BookNotAvailableException : ConflictException
    {
        public BookNotAvailableException(int id) : base($"The book: {id} is not available for borrowing.")
        {
        }
    }
}
