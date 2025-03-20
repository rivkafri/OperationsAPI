using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OperationsAPI.Models
{
    public class Operation
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int OperationId { get; set; }

        [Required] 
        [MaxLength(50)] 
        public string Name { get; set; } = string.Empty;

        public bool SupportsNumbers { get; set; }
        public bool SupportsStrings { get; set; } 

        // קשר לטבלת היסטוריית הפעולות
        public ICollection<OperationHistory> OperationHistories { get; set; } = new List<OperationHistory>();
    }
}

