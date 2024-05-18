namespace WebStore
{
    public class Book
    {
        public int Id { get; }
        public string Title { get; }

        public Book(int id, string title) { (Id, Title) = (id, title); }
    }
}
