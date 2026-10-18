namespace Library_API.Exceptions.Member
{
    public class BorrowingLimitExceededException : ConflictException
    {
        public BorrowingLimitExceededException(int id)
       : base($"The member. {id} has reached the borrowing limit.")
        {
        }
    }
}
