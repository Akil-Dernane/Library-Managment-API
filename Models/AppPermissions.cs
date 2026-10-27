namespace Library_API.Models
{
    public static class AppPermissions
    {
        // Books
        public const string BooksCreate = "books.create";
        public const string BooksUpdate = "books.update";
        public const string BooksDelete = "books.delete";

        // Authors
        public const string AuthorsCreate = "authors.create";
        public const string AuthorsUpdate = "authors.update";
        public const string AuthorsDelete = "authors.delete";

        // Categories
        public const string CategoriesCreate = "categories.create";
        public const string CategoriesUpdate = "categories.update";
        public const string CategoriesDelete = "categories.delete";

        // Loans
        public const string LoansRead = "loans.read";
        public const string LoansCreate = "loans.create";
        public const string LoansReturn = "loans.return";
        public const string LoansDelete = "loans.delete";

        // Members
        public const string MembersRead = "members.read";
        public const string MembersCreate = "members.create";
        public const string MembersUpdate = "members.update";
        public const string MembersDelete = "members.delete";

        public static readonly IReadOnlySet<string> All =
            new HashSet<string>(StringComparer.Ordinal)
            {
                BooksCreate,
                BooksUpdate,
                BooksDelete,

                AuthorsCreate,
                AuthorsUpdate,
                AuthorsDelete,

                CategoriesCreate,
                CategoriesUpdate,
                CategoriesDelete,

                LoansRead,
                LoansCreate,
                LoansReturn,
                LoansDelete,

                MembersRead,
                MembersCreate,
                MembersUpdate,
                MembersDelete
            };
    }
}