using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestWorkService.DataBase.Model
{
    public class KeywordsInMeaningfulText
    {
        [Key]
        public int IDOfKMT { get; set; }
        [ForeignKey("LinkDetails")]
        public int Id { get; set; }
        public DateTime date { get; set; }
        public string keyword { get; set; }
        public int keywordsInText { get; set; }
        public int keywordsInMetaTags { get; set; }
    }
}
