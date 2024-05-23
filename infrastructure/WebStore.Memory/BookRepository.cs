﻿namespace WebStore.Memory
{
    public class BookRepository : IBookRepository
    {
        private readonly Book[] books = new[]
        {
            new Book(1, "ISBN 12345-12345", "Some author", "exemple book"),
            new Book(2, "ISBN 23456-12345", "Another author", "title book"),
            new Book(3, "ISBN 34567-12345", "Last author", "the last one")
        };
        public Book[] GetAllByTitleOrAuthor(string query) 
        { 
            return books.Where(book => 
            book.Author.ToUpper().Contains(query, StringComparison.OrdinalIgnoreCase) 
            || book.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(book => book.Isbn.Equals(isbn)).ToArray();
        }
    }
}