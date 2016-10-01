using System.ComponentModel.DataAnnotations;

namespace RevenueControl.DomainObjects.Entities
{
    public class DataSource
    {
        [Required]
        [MaxLength(50)]
        public string BankAccount { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Culture { get; set; }


        [MaxLength(30)]
        public string ClientName { get; set; }

        [Key]
        public int Id { get; set; }
    }
}