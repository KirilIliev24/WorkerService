using System.ComponentModel.DataAnnotations;

namespace TestWorkService.DataBase.Model
{
    public class LinkPositionTracker
    {
        [Key]
        public string Keywords { get; set; }
        //add domain later
        [Key]
        public string Link { get; set; }
    }
}
