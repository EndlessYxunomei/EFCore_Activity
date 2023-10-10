using InventoryModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels
{
    public abstract class FullAuditModel : IIdentityModel, IAuditedModel, IActivatableModel, ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        [StringLength(InventoryModelConstants.MAX_USERID_LENGTH)]
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(InventoryModelConstants.MAX_USERID_LENGTH)]
        public string? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
