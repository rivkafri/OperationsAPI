using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationsAPI.Models
{
    public class OperationHistory
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        [Required] 
        public int OperationId { get; set; }

        [Required]
        [MaxLength(100)] 
        public string ValueA { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ValueB { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Result { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // קשר לטבלת Operations (Foreign Key)
        [ForeignKey("OperationId")]
        public Operation Operation { get; set; }
    }
}
