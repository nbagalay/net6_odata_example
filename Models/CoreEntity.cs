using System.ComponentModel.DataAnnotations;

namespace OData_webapi_netcore6.Models
{
    public class CoreEntity
    {
        public CoreEntity()
        {
            this.CreatedDate = DateTime.UtcNow;
            this.ModifiedDate = DateTime.UtcNow;
        }

        public DateTimeOffset CreatedDate { get; set; }

        [ConcurrencyCheck]
        public DateTimeOffset ModifiedDate { get; set; }

        [MaxLength(100)]
        public string? CreatedBy { get; set; }

        [MaxLength(100)]
        public string? ModifiedBy { get; set; }
    }
}
