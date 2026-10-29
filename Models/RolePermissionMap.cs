namespace Library_API.Models
{
    public static class RolePermissionMap
    {
        private static readonly string[] MemberPermissions =
        {
            AppPermissions.Loans.ReadOwn
        };

        private static readonly string[] LibrarianPermission = MemberPermissions.Concat(new[]
        {
            AppPermissions.Books.Create,
            AppPermissions.Books.Update,
            AppPermissions.Loans.Lend,
            AppPermissions.Loans.Return,
            AppPermissions.Loans.ReadAll,
            AppPermissions.Members.ReadAll
        }).ToArray();

        public static readonly Dictionary<string, string[]> Defaults = new() {
            [Roles.Member]      = MemberPermissions,
            [Roles.Librarian]   = LibrarianPermission,
            [Roles.Admin]       = AppPermissions.All.ToArray(),
        };
    }
}
