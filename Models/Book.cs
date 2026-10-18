using Library_API.Exceptions.Book;
using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required,StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required,Range(1,int.MaxValue)]
        public int Pages { get; set; }
        public bool IsAvailable { get; set; } = true;
        public List<Category> Categories { get; set; } = new();
        public List<Author> Authors { get; set; } = new();

        public void Borrow()
        {
            if (!this.IsAvailable)
                throw new BookNotAvailableException(this.Id);

            IsAvailable = false;
        }

        public void Return()
        {
            IsAvailable = true;
        }
    }
}
