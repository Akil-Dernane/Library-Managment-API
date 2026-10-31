using Library_API.Migrations;
using System.Reflection;

namespace Library_API.Models
{
    public static class AppPermissions
    {
        public static class Books
        { 
            public const string Create = "books.create";
            public const string Update = "books.update";
            public const string Delete = "books.delete";
        }
        public static class Authors
        {
            public const string Create = "authors.create";
            public const string Update = "authors.update";
            public const string Delete = "authors.delete";
        }

        public static class Categories
        {
            public const string Create = "categories.create";
            public const string Update = "categories.update";
            public const string Delete = "categories.delete";
        }

        public static class Loans
        {
            public const string Lend = "loans.lend";
            public const string Return = "loans.return";
            public const string ReadOwn = "loans.read.own";
            public const string ReadAll = "loans.read.all";
            public const string Delete = "loans.delete";
        }

        public static class Members
        {
            public const string ReadAll = "members.read.all";
            public const string ReadOwn = "members.read.own";
            public const string Update = "members.update";
            public const string Delete = "members.delete";
        }

        public static class Users
        {
            public const string Manage = "users.manage";
        }

        public static class AuditLogs {
            public const string ReadAll = "audit-logs.read.all";
        }

        public static IReadOnlyList<string> All { get; } = typeof(AppPermissions)
        .GetNestedTypes()
        .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
        .Where(f => f.IsLiteral)
        .Select(f => (string)f.GetRawConstantValue()!)
        .ToList();
    }
}