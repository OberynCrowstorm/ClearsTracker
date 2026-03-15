using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OberynsClearsTracker
{
    public class WingEntry
    {
        public string Name { get; set; }
        public string IconId { get; set; }
        public BossEntry[] Bosses { get; set; }
        public string Abbreviation { get; set; }

        public bool IsFullyCleared
        {
            get
            {
                foreach (var boss in Bosses)
                {
                    if (!boss.IsWeeklyCleared)
                        return false;
                }
                return true;
            }
        }
    }
}