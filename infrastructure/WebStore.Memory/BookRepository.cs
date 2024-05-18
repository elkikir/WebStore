namespace WebStore.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books = new[]
        {
            new Book(1, "exemple book"),
            new Book(2, "title book"),
            new Book(3, "the last one")
        };
        public Book[] GetAllBooksByTitle(string titlePart) 
        { 
            return books.Where(book => book.Title.Contains(titlePart)).ToArray();
        }
    }
}