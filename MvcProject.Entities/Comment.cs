using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcProject.Entities
{
    [Table("Comments")]
    public class Comment : MyEntityBase
    {
        [Required, StringLength(300)]
        public string Text { get; set; }
        /*
         public int IsApproval {get; set} Yorumları böyle kontrol edebiliriz !
         */

        public virtual Note Note { get; set; }
        public virtual MvcProjectUser Owner { get; set; }
    }
}
