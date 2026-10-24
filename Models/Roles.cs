namespace Library_API.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Librarian = "Librarian";
        public const string Member = "Member";

        public const string AdminOrLibrarian = Admin + "," + Librarian;
    }
}
