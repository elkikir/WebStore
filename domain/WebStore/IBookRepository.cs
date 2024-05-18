namespace WebStore
{
    public interface IBookRepository
    {
        Book[] GetAllBooksByTitle(string titlePart);
    }
}
