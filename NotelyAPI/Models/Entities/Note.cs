using System.ComponentModel.DataAnnotations;

namespace NotelyAPI.Models.Entities
{
    public class Note
    {
        public int NoteId { get; set; }

        [Required]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
        //("PERSONAL", "HOME", "BUSINESS")
        public NoteCategory Category { get; set; }

        public DateTime CreatedDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
