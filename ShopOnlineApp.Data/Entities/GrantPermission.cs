using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ShopOnlineApp.Infrastructure.SharedKernel;

namespace ShopOnlineApp.Data.Entities
{
    [Table("GrantPermission")]
    public class GrantPermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("BusinessAction")]
        [Column(Order = 1)]
        public int BusinessActionId { get; set; }
        [Column(Order = 2)]
        [ForeignKey("AppUser")]
        public Guid UserId { get; set; }
     

        public virtual AppUser AppUser { get; set; }

        public virtual BusinessAction BusinessAction { get; set; }
    }
}
