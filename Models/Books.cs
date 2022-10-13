using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OData_webapi_netcore6.Models
{
    public class Books : CoreEntity
    {
        [Key]
        [Column("BookGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indicates we provide the Key, not the database
        public Guid Guid { get; private set; }

        [ForeignKey("Authors")]
        public Guid AuthorGuid { get; set; }

        public string Title { get; set; }

        public int PublishYear { get; set; }

        public int CopiesSold { get; set; }

        public Authors? Authors { get; set; }
    }
}
