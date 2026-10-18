namespace Library_API.Exceptions.Loan
{
    public class LoanAlreadyReturnedException : ConflictException
    {
        public LoanAlreadyReturnedException(int id ): base($"Loan {id} has already been returned.")
        {
        }
    }
}
