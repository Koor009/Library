namespace СentralLibrary_Db.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreateBookViewModels
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Enter a name of a book.")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A book genre.")]
        [Display(Name = "Genre")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Enter a name of an author.")]
        [Display(Name = "Author")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Number publication a book.")]
        [Display(Name = "Publication")]
        public int Publication { get; set; }

        [Required(ErrorMessage = "Number of books.")]
        [Display(Name = "Count of books")]
        public int CountOfBooks { get; set; }

        [Required(ErrorMessage = "Enter date of publication")]
        [Display(Name = "Date of publication.", Prompt ="dd/mm/yyyy")]
        public DateTime DateOfPublication { get; set; }
    }
}