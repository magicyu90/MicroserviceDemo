using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace User.API.Database.Entities
{
    [Table("Certificate")]
    public class Certificate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public UserEntity User { get; set; }

        [ForeignKey("TbUser")]
        public int UserID { get; set; }

    }
}
