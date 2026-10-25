namespace Library_API.Exceptions.Interfaces
{
    public interface IHasErrors
    {
        IReadOnlyList<string> Errors { get; }
    }
}
