using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class MedicalFile
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; } = "";

        [Required]
        public string FileType { get; set; } = "";

        [Required]
        public string FilePath { get; set; } = "";

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
