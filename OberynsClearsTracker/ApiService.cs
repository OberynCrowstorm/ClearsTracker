using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;

namespace OberynsClearsTracker
{
    public class ApiService
    {
        private static readonly Logger Logger = Logger.GetLogger<ApiService>();

        private const int DailyBountyCategoryId = 475;

        private readonly Gw2ApiManager _gw2ApiManager;

        // Today's 4 active bounties — populated after UpdateAllClearsAsync
        public List<DailyBountySlot> TodaysBounties { get; private set; } = new List<DailyBountySlot>();
        public List<DailyBountySlot> TomorrowsBounties { get; private set; } = new List<DailyBountySlot>();

        public ApiService(Gw2ApiManager gw2ApiManager)
        {
            _gw2ApiManager = gw2ApiManager;
        }

        private bool HasRequiredPermissions()
        {
            return _gw2ApiManager.HasPermissions(new[]
            {
                TokenPermission.Account,
                TokenPermission.Progression
            });
        }

        public async Task UpdateAllClearsAsync(bool fetchRaids = true, bool fetchStrikes = true, bool fetchBounties = true)
        {
            if (!HasRequiredPermissions())
            {
                Logger.Info("API permissions not available yet.");
                return;
            }

            try
            {
                var raidsTask = fetchRaids
                    ? FetchRaidClearsAsync()
                    : Task.FromResult(new HashSet<string>());

                var achievementsTask = (fetchStrikes || fetchBounties)
                    ? FetchAchievementsAsync()
                    : Task.FromResult(new List<AccountAchievement>());

                var todayBountyTask = fetchBounties
                    ? FetchTodaysBountyIdsAsync()
                    : Task.FromResult(new HashSet<int>());

                await Task.WhenAll(raidsTask, achievementsTask, todayBountyTask);

                var clearedRaidIds = raidsTask.Result;
                var accountAchievements = achievementsTask.Result;
                var todaysBountyIds = todayBountyTask.Result;

                if (fetchRaids)
                    UpdateWeeklyRaidClears(clearedRaidIds);

                if (fetchStrikes)
                    UpdateWeeklyStrikeClears(accountAchievements);

                if (fetchBounties)
                {
                    UpdateDailyBounties(accountAchievements, todaysBountyIds);
                    BuildTodaysBountySlots(todaysBountyIds, accountAchievements);
                    TomorrowsBounties = BountyRotationService.GetTomorrowsBounties();
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to update clears from API.");
            }
        }

        private async Task<HashSet<string>> FetchRaidClearsAsync()
        {
            var result = await _gw2ApiManager.Gw2ApiClient.V2.Account.Raids.GetAsync();
            return new HashSet<string>(result, StringComparer.OrdinalIgnoreCase);
        }

        private async Task<List<AccountAchievement>> FetchAchievementsAsync()
        {
            var result = await _gw2ApiManager.Gw2ApiClient.V2.Account.Achievements.GetAsync();
            return result.ToList();
        }

        private async Task<HashSet<int>> FetchTodaysBountyIdsAsync()
        {
            try
            {
                var category = await _gw2ApiManager.Gw2ApiClient.V2.Achievements.Categories.GetAsync(DailyBountyCategoryId);
                return new HashSet<int>(category.Achievements.Select(a => (int)a));
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to fetch today's bounty category.");
                return new HashSet<int>();
            }
        }

        private void UpdateWeeklyRaidClears(HashSet<string> clearedRaidIds)
        {
            foreach (var wing in RaidData.Wings)
                foreach (var boss in wing.Bosses)
                    boss.IsWeeklyCleared = clearedRaidIds.Contains(boss.ApiId);

            Logger.Info("Weekly raid clears updated.");
        }

        private void UpdateWeeklyStrikeClears(List<AccountAchievement> achievements)
        {
            var weeklyAchievement = achievements
                .FirstOrDefault(a => a.Id == RaidData.WeeklyStrikeAchievementId);

            if (weeklyAchievement == null || weeklyAchievement.Bits == null)
            {
                Logger.Info("Weekly strike achievement not found or has no bits.");
                return;
            }

            var clearedBits = new HashSet<int>(weeklyAchievement.Bits);

            foreach (var expansion in RaidData.Expansions)
            {
                foreach (var strike in expansion.Strikes)
                {
                    var bitIndex = Array.IndexOf(RaidData.WeeklyStrikeBitOrder, strike.ApiId);
                    if (bitIndex >= 0)
                        strike.IsWeeklyCleared = clearedBits.Contains(bitIndex);
                }
            }

            Logger.Info("Weekly strike clears updated.");
        }

        private void UpdateDailyBounties(List<AccountAchievement> achievements, HashSet<int> todaysBountyIds)
        {
            if (todaysBountyIds.Count == 0)
            {
                ResetAllDailyBounties();
                return;
            }

            var completedTodayIds = new HashSet<int>(
                achievements
                    .Where(a => a.Done && todaysBountyIds.Contains(a.Id))
                    .Select(a => a.Id)
            );

            foreach (var wing in RaidData.Wings)
                foreach (var boss in wing.Bosses)
                    if (boss.DailyBountyAchievementId.HasValue)
                        boss.IsDailyCompleted = completedTodayIds.Contains(boss.DailyBountyAchievementId.Value);
                    else
                        boss.IsDailyCompleted = false;

            foreach (var expansion in RaidData.Expansions)
                foreach (var strike in expansion.Strikes)
                    if (strike.DailyBountyAchievementId.HasValue)
                        strike.IsDailyCompleted = completedTodayIds.Contains(strike.DailyBountyAchievementId.Value);
                    else
                        strike.IsDailyCompleted = false;

            Logger.Info($"Daily bounties updated. Completed: {completedTodayIds.Count}");
        }

        private void BuildTodaysBountySlots(HashSet<int> todaysBountyIds, List<AccountAchievement> achievements)
        {
            TodaysBounties = new List<DailyBountySlot>();

            if (todaysBountyIds.Count == 0)
                return;

            var completedIds = new HashSet<int>(
                achievements.Where(a => a.Done).Select(a => a.Id)
            );

            // Check raid bosses first
            foreach (var wing in RaidData.Wings)
            {
                foreach (var boss in wing.Bosses)
                {
                    if (!boss.DailyBountyAchievementId.HasValue)
                        continue;

                    if (!todaysBountyIds.Contains(boss.DailyBountyAchievementId.Value))
                        continue;

                    TodaysBounties.Add(new DailyBountySlot
                    {
                        Name = boss.Name,
                        Abbreviation = boss.Abbreviation,
                        WingName = wing.Name,
                        IconId = boss.IconId,
                        IsStrike = false,
                        IsCompleted = completedIds.Contains(boss.DailyBountyAchievementId.Value),
                        AchievementId = boss.DailyBountyAchievementId.Value
                    });
                }
            }

            // Then check strikes
            foreach (var expansion in RaidData.Expansions)
            {
                foreach (var strike in expansion.Strikes)
                {
                    if (!strike.DailyBountyAchievementId.HasValue)
                        continue;

                    if (!todaysBountyIds.Contains(strike.DailyBountyAchievementId.Value))
                        continue;

                    TodaysBounties.Add(new DailyBountySlot
                    {
                        Name = strike.Name,
                        Abbreviation = strike.Abbreviation,
                        WingName = strike.Name,
                        IconId = strike.IconId,
                        IsStrike = true,
                        IsCompleted = completedIds.Contains(strike.DailyBountyAchievementId.Value),
                        AchievementId = strike.DailyBountyAchievementId.Value
                    });
                }
            }

            Logger.Info($"Built {TodaysBounties.Count} daily bounty slots.");
        }

        private void ResetAllDailyBounties()
        {
            foreach (var wing in RaidData.Wings)
                foreach (var boss in wing.Bosses)
                    boss.IsDailyCompleted = false;

            foreach (var expansion in RaidData.Expansions)
                foreach (var strike in expansion.Strikes)
                    strike.IsDailyCompleted = false;
        }
    }
}