using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }       
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
