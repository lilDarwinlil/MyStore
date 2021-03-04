using System;
using System.Linq;

namespace Store.Memory
{
    public class BookRepository : IBookRepository
    {
        readonly Book[] books = new[]
        {
            new Book (1, "Art Of Programming", "ISBN 12345-12345", "D. Knuth", "Descrip1", 7.19m),
            new Book (2, "Refactoring", "ISBN 12345-12346", "M. Fowler", "Descrip2", 12.45m),
            new Book (3, "C Programming Language", "ISBN 12345-12347", "B. Kernighan, D. Ritchie", "Descrip3", 14.98m),
        };

        public Book[] GetAllByIsbn(string isbn)
        {
            return books.Where(b => b.Isbn == isbn).ToArray();
        }

        public Book[] GetAllByTitleOrAuthor(string titleorAuthor)
        {
            return books.Where(b => b.Title.Contains(titleorAuthor)
                                 || b.Author.Contains(titleorAuthor))
                        .ToArray();
        }

        public Book GetById(int id)
        {
            return books.Single(b => b.Id == id);
        }
    }
}
