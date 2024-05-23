using System.Text.RegularExpressions;

namespace WebStore
{
    public class Book
    {
        public int Id { get; }
        public string Title { get; }
        public string Isbn { get; }
        public string Author { get; } //ненормализрованные данные - лучше использовать IdAuthor для обращения к сущности через идентификатор

        public Book(int id, string isbn, string author,string title) 
        { 
            (Id, Isbn, Author, Title) = (id, isbn, author, title); 
        }

        internal static bool IsIsbn(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            s = s.Replace(" ", "").Replace("-", "").ToUpper();
            return Regex.IsMatch(s, @"^ISBN\d{10}(\d{3})?$");
        }
    }
}
