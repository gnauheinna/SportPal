using System.ComponentModel.DataAnnotations;
namespace SportMeApp.Models
{
    public class Sport
    {
        [Key]
        public int SportId { get; set; }
        public string SportName { get; set; }
    }
}
