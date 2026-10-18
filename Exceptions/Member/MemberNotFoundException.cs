namespace Library_API.Exceptions.Member
{
    public class MemberNotFoundException : EntityNotFoundException
    {
        public MemberNotFoundException(int id) : base($"Member with id {id} wasn't found")
        {
        }
    }
}
