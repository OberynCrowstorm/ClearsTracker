using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OberynsClearsTracker
{
    public class ExpansionEntry
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string IconId { get; set; }
        public StrikeEntry[] Strikes { get; set; }

        public bool IsFullyCleared
        {
            get
            {
                foreach (var strike in Strikes)
                {
                    if (!strike.IsWeeklyCleared)
                        return false;
                }
                return true;
            }
        }
    }
}