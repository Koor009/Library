namespace СentralLibrary_Db.Models
{
    using System;

    /// <summary>
    /// Book registration with binding by user id and book id.
    /// </summary>
    public sealed class Registration
    {
        public Guid Id { get; set; }

        public bool Availability { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string BookId { get; set; }

        public Book GetBook { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Registration"/> class.
        /// </summary>
        public Registration() => this.Id = Guid.NewGuid();

    }
}