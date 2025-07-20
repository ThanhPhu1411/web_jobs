using System.ComponentModel.DataAnnotations.Schema;

namespace web_jobs.Models
{
    [Table("Category")]
    public class Category
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
