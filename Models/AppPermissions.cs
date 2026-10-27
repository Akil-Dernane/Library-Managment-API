namespace Library_API.Models
{
    public class AppPermissions
    {
        public const string BooksRead = "books.read";
        public const string BooksCreate = "books.create";
        public const string BooksUpdate = "books.update";
        public const string BooksDelete = "books.delete";

        public static readonly IReadOnlySet<string> All =
            new HashSet<string>(StringComparer.Ordinal)
            {
            BooksRead,
            BooksCreate,
            BooksUpdate,
            BooksDelete
            };
    }
}
