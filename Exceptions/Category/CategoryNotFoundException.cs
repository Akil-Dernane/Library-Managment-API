namespace Library_API.Exceptions.Category
{
    public class CategoryNotFoundException : EntityNotFoundException
    {
        public CategoryNotFoundException(int id) : base($"Category with id {id} wasn't found")
        {
        }
    }
}
