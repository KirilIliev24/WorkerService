using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWorkService.DataBase.Model
{
    public class LinkDetails
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("LinkPositionTracker")]
        public string Link { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public int Index { get; set; }
    }
}
