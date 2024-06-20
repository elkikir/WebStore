namespace WebStore
{
    public interface IBookRepository
    {
        Book[] GetAllByIsbn(string isbn);

        Book[] GetAllByTitleOrAuthor(string titleOrAuthorPart);
        Book GetById(int id);
    }
}
