using System;
using System.Collections.Generic;

namespace OberynsClearsTracker
{
    public static class BountyRotationService
    {
        public static int GetDayIndex()
        {
            return GetDayIndex(DateTime.UtcNow);
        }

        public static int GetDayIndex(DateTime date)
        {
            var day = date.DayOfYear - 1;
            if (DateTime.IsLeapYear(date.Year))
                return day;
            else
            {
                if (date.Month >= 3)
                    return day + 1;
                return day;
            }
        }

        public static List<DailyBountySlot> GetBountiesForDay(int dayIndex)
        {
            var slots = new[]
            {
                BountyRotationData.Slot1,
                BountyRotationData.Slot2,
                BountyRotationData.Slot3,
                BountyRotationData.Slot4
            };

            var result = new List<DailyBountySlot>();

            foreach (var slot in slots)
            {
                var apiId = slot[dayIndex % slot.Length];
                var slot2 = FindBountySlot(apiId);
                if (slot2 != null)
                    result.Add(slot2);
            }

            return result;
        }

        public static List<DailyBountySlot> GetTomorrowsBounties()
        {
            return GetBountiesForDay(GetDayIndex() + 1);
        }

        private static DailyBountySlot FindBountySlot(string apiId)
        {
            // Check raid bosses first
            foreach (var wing in RaidData.Wings)
            {
                foreach (var boss in wing.Bosses)
                {
                    if (boss.ApiId == apiId)
                    {
                        return new DailyBountySlot
                        {
                            Name = boss.Name,
                            Abbreviation = boss.Abbreviation,
                            WingName = wing.Name,
                            IconId = boss.IconId,
                            IsStrike = false,
                            IsCompleted = false
                        };
                    }
                }
            }

            // Then check strikes
            foreach (var expansion in RaidData.Expansions)
            {
                foreach (var strike in expansion.Strikes)
                {
                    if (strike.ApiId == apiId)
                    {
                        return new DailyBountySlot
                        {
                            Name = strike.Name,
                            Abbreviation = strike.Abbreviation,
                            WingName = expansion.Name,
                            IconId = strike.IconId,
                            IsStrike = true,
                            IsCompleted = false
                        };
                    }
                }
            }

            return null;
        }
    }
}