using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OData_webapi_netcore6.Models
{
    public class Authors
    {
        [Key]
        [Column("AuthorGuid")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Indicates we provide the Key, not the database
        public Guid Guid { get; private set; }

        public string Name { get; set; }

        public string HomeState { get; set; }

        public string Genre { get; set; }

        public ICollection<Books>? Books { get; set; }
    }
}
