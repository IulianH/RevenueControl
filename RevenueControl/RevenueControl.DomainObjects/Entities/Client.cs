using System.ComponentModel.DataAnnotations;

namespace RevenueControl.DomainObjects.Entities
{
    public class Client
    {
        [Key]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}