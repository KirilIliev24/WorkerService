using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestWorkService.DataBase.Model
{
    public class PositonAndDate
    {
        [Key]
        public int PositionAndDateId { get; set; }
        [ForeignKey("LinkPositionTracker")]
        public string Keywords { get; set; }
        [ForeignKey("LinkPositionTracker")]
        public string Link { get; set; }
        public int Position { get; set; }
        public double Js { get; set; }
        public double Css { get; set; }
        public int WordCount { get; set; }
        public string MeaningfulText { get; set; }
        public DateTime Date { get; set; }

    }
}
