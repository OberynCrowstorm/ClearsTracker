using Blish_HUD.Settings;

namespace OberynsClearsTracker
{
    public class ModuleSettings
    {
        public SettingEntry<bool> ShowDailyTab { get; private set; }
        public SettingEntry<bool> ShowWeeklyRaids { get; private set; }
        public SettingEntry<bool> ShowWeeklyStrikes { get; private set; }

        public ModuleSettings(SettingCollection settings)
        {
            ShowDailyTab = settings.DefineSetting(
                "ShowDailyTab",
                true,
                () => "Daily Tab",
                () => "Show or hide the Daily Raid Bounties tab."
            );

            ShowWeeklyRaids = settings.DefineSetting(
                "ShowWeeklyRaids",
                true,
                () => "Weekly Raids",
                () => "Show or hide the Weekly Raid Clears tab."
            );

            ShowWeeklyStrikes = settings.DefineSetting(
                "ShowWeeklyStrikes",
                true,
                () => "Weekly Strikes",
                () => "Show or hide the Weekly Strike Clears tab."
            );
        }
    }
}