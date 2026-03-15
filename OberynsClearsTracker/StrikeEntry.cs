using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OberynsClearsTracker
{
    public class StrikeEntry
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string IconId { get; set; }
        public string ApiId { get; set; }
        public int? DailyBountyAchievementId { get; set; }
        public bool IsWeeklyCleared { get; set; }
        public bool IsDailyCompleted { get; set; }
    }
}