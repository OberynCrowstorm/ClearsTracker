using Blish_HUD;
using Blish_HUD.Content; 
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;

namespace OberynsClearsTracker
{
    public class SettingsWindow : StandardWindow
    {
        public SettingsWindow(ModuleSettings settings) : base(
            ContentService.Textures.Pixel,
            new Rectangle(0, 0, 300, 200),
            new Rectangle(10, 10, 280, 180))
        {
            Parent = GameService.Graphics.SpriteScreen;
            Title = "Clears Tracker Settings";
            SavesPosition = true;
            Id = "OberynsClearsTracker_SettingsWindow";

            BuildLayout(settings);
            Location = new Point(
                (GameService.Graphics.SpriteScreen.Width / 2) - 150,
                (GameService.Graphics.SpriteScreen.Height / 2) - 100
            );
        }

        private void BuildLayout(ModuleSettings settings)
        {
            var flow = new FlowPanel
            {
                Parent = this,
                Width = 260,
                Height = 160,
                Location = new Point(10, 10),
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, 10)
            };

            BuildToggle(flow, "Daily Tab", settings.ShowDailyTab);
            BuildToggle(flow, "Weekly Raids", settings.ShowWeeklyRaids);
            BuildToggle(flow, "Weekly Strikes", settings.ShowWeeklyStrikes);
        }

        private void BuildToggle(FlowPanel parent, string label, Blish_HUD.Settings.SettingEntry<bool> setting)
        {
            var row = new Panel
            {
                Parent = parent,
                Width = 260,
                Height = 30
            };

            new Label
            {
                Parent = row,
                Text = label,
                Location = new Point(0, 5),
                Width = 180,
                Height = 24,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            var checkbox = new Checkbox
            {
                Parent = row,
                Location = new Point(190, 5),
                Checked = setting.Value
            };

            checkbox.CheckedChanged += (s, e) =>
            {
                setting.Value = e.Checked;
            };
        }
    }
}