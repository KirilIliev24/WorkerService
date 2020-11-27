using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestWorkService.DataBase.Model
{
    public class ExternalLinks
    {
        [Key]
        public int IDOfExternal { get; set; }
        [ForeignKey("LinkDetails")]
        public int Id { get; set; }
        public DateTime date { get; set; }
        public string externalLink { get; set; }
    }
}
