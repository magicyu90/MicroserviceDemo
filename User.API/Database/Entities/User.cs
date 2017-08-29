using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Database.Entities
{
    [Table("TbUser")]
    public class UserEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; }

        [Required, MaxLength(100)]
        public string EnterpriseCode { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public string Mobile { get; set; }

        public virtual IList<Certificate> Certificates { get; set; }
    }
}
