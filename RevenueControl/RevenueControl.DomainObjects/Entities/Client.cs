using System.ComponentModel.DataAnnotations;

namespace RevenueControl.DomainObjects.Entities
{
    public class Client
    {
        [Key]
        [MaxLength(75)]
        public string Name { get; set; }
    }
}