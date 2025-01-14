using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Refund
    {
        [Key]
        public int Id { get; set; }  
        public int TransactionId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }  
        public string? RefundReason { get; set; }  
        public DateTime CreatedDate { get; set; }  
        public Transaction Transaction { get; set; }
    }
}
