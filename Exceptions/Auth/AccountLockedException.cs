namespace Library_API.Exceptions.Auth
{
    public class AccountLockedException : Exception
    {
        public DateTimeOffset? LockoutEnd { get; }

        public AccountLockedException(DateTimeOffset? lockoutEnd = null)
            : base("Your account is temporarily locked. Please try again later.")
        {
            LockoutEnd = lockoutEnd;
        }
    }
}
