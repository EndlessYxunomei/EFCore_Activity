﻿using InventoryModels.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels
{
    public class CategoryDetail : IIdentityModel
    {
        [Key, ForeignKey("Category")]
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(InventoryModelConstants.MAX_COLORVALUE_LENGTH)]
        public required string ColorValue { get; set; }
        [Required]
        [StringLength(InventoryModelConstants.MAX_COLORNAME_LENGTH)]
        public required string ColorName { get; set; }
        public virtual Category Category { get; set; }
    }
}
