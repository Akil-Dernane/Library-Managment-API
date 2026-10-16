namespace Library_API.Exceptions
{
    public class LoanNotFoundException : EntityNotFoundException
    {
        public LoanNotFoundException(int id) : base($"Laon with id {id} wasn't found")
        {
            
        }
    }
}
