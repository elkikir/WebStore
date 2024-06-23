
namespace WebStore
{
    public interface IBookRepository
    {
        Book[] GetAllById(IEnumerable<int> bookId);
        Book[] GetAllByIsbn(string isbn);

        Book[] GetAllByTitleOrAuthor(string titleOrAuthorPart);
        Book GetById(int id);
    }
}
