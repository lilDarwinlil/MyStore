using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Data
{
    public class BookDto
    {
        public int Id { get; }

        public string Title { get; }

        public string Isbn { get; }

        public string Author { get; }

        public string Description { get; }

        public decimal Price { get; }
    }
}
