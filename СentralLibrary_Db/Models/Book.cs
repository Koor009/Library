namespace СentralLibrary_Db.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Information about the book by contacting a registration id.
    /// </summary>
    public class Book
    {
        public Guid Id { get; set; }

        public byte[] Image { get; set; }

        public string Name { get; set; }

        public string Genre { get; set; }

        public string Author { get; set; }

        public bool Blocked { get; set; }

        public int Publication { get; set; }

        public int CountOfBooks { get; set; }

        public DateTime DateOfPublication { get; set; }

        public ICollection<Registration> Registrations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class.
        /// </summary>
        public Book()
        {
            this.Id = Guid.NewGuid();
            this.Registrations = new List<Registration>();
        }
    }
}