using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace OberynsClearsTracker
{
    [Export(typeof(Module))]
    public class OberynsClearsTracker : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<OberynsClearsTracker>();

        private class TabDefinition
        {
            public string Id { get; set; }
            public Func<string> GetLabel { get; set; }
            public Panel Content { get; set; }
            public Panel Button { get; set; }
            public Label Label { get; set; }
            public Panel Underline { get; set; }
            public SettingEntry<bool> IsEnabled { get; set; }
        }

        private ApiService _apiService;
        private ForgingSteelPersistence _forgingSteelPersistence;
        private ModuleSettings _moduleSettings;
        private SettingsWindow _settingsWindow;

        private CornerIcon _cornerIcon;
        private StandardWindow _mainWindow;
        private Panel _tabBar;

        private List<TabDefinition> _tabs = new List<TabDefinition>();
        private TabDefinition _activeTab;

        private Panel _dailyContent;
        private Panel _weeklyRaidsContent;
        private Panel _weeklyStrikesContent;

        private DailyView _dailyView;
        private WeeklyView _weeklyRaidsView;
        private WeeklyView _weeklyStrikesView;

        private DateTime _lastRefresh = DateTime.MinValue;
        private System.Threading.Timer _refreshTimer;

        private const int TabBarY = 6;
        private const int TabBarHeight = 40;
        private const int ContentY = TabBarY + TabBarHeight + 5;
        private const int ContentWidth = 840;
        private const int ContentHeight = 610;
        private const int WindowWidth = 840;

        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;

        [ImportingConstructor]
        public OberynsClearsTracker([Import("ModuleParameters")] ModuleParameters moduleParameters)
            : base(moduleParameters)
        {
            Logger.Info("Module constructor called.");
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _moduleSettings = new ModuleSettings(settings);
        }

        private void OnKeyPressed(object sender, Blish_HUD.Input.KeyboardEventArgs e)
        {
            if (e.Key == Microsoft.Xna.Framework.Input.Keys.Escape && _mainWindow.Visible)
                _mainWindow.Hide();
        }

        private void OnShowDailyTabChanged(object s, ValueChangedEventArgs<bool> e) => RebuildTabBar();
        private void OnShowWeeklyRaidsChanged(object s, ValueChangedEventArgs<bool> e) => RebuildTabBar();
        private void OnShowWeeklyStrikesChanged(object s, ValueChangedEventArgs<bool> e) => RebuildTabBar();

        protected override async Task LoadAsync()
        {
            Logger.Info("LoadAsync started.");

            _forgingSteelPersistence = ForgingSteelPersistence.Load(DirectoriesManager);
            _apiService = new ApiService(Gw2ApiManager);

            GameService.Input.Keyboard.KeyPressed += OnKeyPressed;

            BuildWindow();
            BuildContentPanels();
            BuildTabBar();
            ShowFirstVisibleTab();

            _cornerIcon = new CornerIcon
            {
                Icon = ContentsManager.GetTexture("icons/icon.png"),
                HoverIcon = ContentsManager.GetTexture("icons/icon_hover.png"),
                BasicTooltipText = "Clears Tracker",
                Parent = GameService.Graphics.SpriteScreen,
            };

            _cornerIcon.Click += (s, e) =>
            {
                if (_mainWindow.Visible)
                    _mainWindow.Hide();
                else
                {
                    _mainWindow.Show();
                    _ = TryRefreshAsync();
                }
            };

            _settingsWindow = new SettingsWindow(_moduleSettings);

            var contextMenu = new ContextMenuStrip();
            var settingsItem = new ContextMenuStripItem("Settings");
            settingsItem.Click += (s, e) => _settingsWindow.Show();
            contextMenu.AddMenuItem(settingsItem);
            _cornerIcon.Menu = contextMenu;

            // Wire up setting change listeners
            _moduleSettings.ShowDailyTab.SettingChanged += OnShowDailyTabChanged;
            _moduleSettings.ShowWeeklyRaids.SettingChanged += OnShowWeeklyRaidsChanged;
            _moduleSettings.ShowWeeklyStrikes.SettingChanged += OnShowWeeklyStrikesChanged;

            Gw2ApiManager.SubtokenUpdated += OnSubtokenUpdated;

            _refreshTimer = new System.Threading.Timer(
                async _ => await TryRefreshAsync(),
                null,
                TimeSpan.FromMinutes(5),
                TimeSpan.FromMinutes(5)
            );

            await TryRefreshAsync();

            Logger.Info("LoadAsync finished.");
        }

        private void BuildWindow()
        {
            var windowBackground = ContentsManager.GetTexture("icons/background.png");

            _mainWindow = new StandardWindow(
                windowBackground,
                new Rectangle(25, 26, 860, 720),
                new Rectangle(40, 50, 840, 680))
            {
                Parent = GameService.Graphics.SpriteScreen,
                Title = "Clears Tracker",
                Subtitle = "Never refreshed",
                Location = new Point(50, 50),
                SavesPosition = true,
                Id = "ClearsTracker_MainWindow"
            };
        }

        private void BuildContentPanels()
        {
            _dailyContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, ContentY),
                Width = ContentWidth,
                Height = ContentHeight,
                Visible = false
            };

            _weeklyRaidsContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, ContentY),
                Width = ContentWidth,
                Height = ContentHeight,
                Visible = false
            };

            _weeklyStrikesContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, ContentY),
                Width = ContentWidth,
                Height = ContentHeight,
                Visible = false
            };

            _dailyView = new DailyView(ContentsManager);
            _dailyView.Parent = _dailyContent;
            _dailyView.Width = ContentWidth;
            _dailyView.Height = ContentHeight;

            _weeklyRaidsView = new WeeklyView(ContentsManager, showRaids: true, _forgingSteelPersistence);
            _weeklyRaidsView.Parent = _weeklyRaidsContent;
            _weeklyRaidsView.Initialize(ContentWidth, ContentHeight);

            _weeklyStrikesView = new WeeklyView(ContentsManager, showRaids: false, _forgingSteelPersistence);
            _weeklyStrikesView.Parent = _weeklyStrikesContent;
            _weeklyStrikesView.Initialize(ContentWidth, ContentHeight);
        }

        private void BuildTabBar()
        {
            // Dispose existing tab buttons if rebuilding
            foreach (var tab in _tabs)
                tab.Button?.Dispose();
            _tabs.Clear();

            // Dispose existing tab bar if rebuilding
            _tabBar?.Dispose();

            _tabBar = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, TabBarY),
                Width = WindowWidth,
                Height = TabBarHeight,
                BackgroundColor = Color.Black * 0.3f
            };

            // Build tab definitions based on settings
            var visibleTabs = new List<(string id, Func<string> getLabel, Panel content, SettingEntry<bool> setting)>();

            if (_moduleSettings.ShowDailyTab.Value)
                visibleTabs.Add(("daily", GetDailyLabel, _dailyContent, _moduleSettings.ShowDailyTab));

            if (_moduleSettings.ShowWeeklyRaids.Value)
                visibleTabs.Add(("raids", GetRaidsLabel, _weeklyRaidsContent, _moduleSettings.ShowWeeklyRaids));

            if (_moduleSettings.ShowWeeklyStrikes.Value)
                visibleTabs.Add(("strikes", GetStrikesLabel, _weeklyStrikesContent, _moduleSettings.ShowWeeklyStrikes));

            if (visibleTabs.Count == 0) return;

            int tabWidth = WindowWidth / visibleTabs.Count;

            for (int i = 0; i < visibleTabs.Count; i++)
            {
                var (id, getLabel, content, setting) = visibleTabs[i];
                int x = i * tabWidth;

                var button = new Panel
                {
                    Parent = _tabBar,
                    Location = new Point(x, 0),
                    Width = tabWidth,
                    Height = TabBarHeight,
                    BackgroundColor = Color.Transparent
                };

                var label = new Label
                {
                    Parent = button,
                    Text = getLabel(),
                    Width = tabWidth,
                    Height = 34,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Middle,
                    TextColor = Color.White,
                    Font = GameService.Content.DefaultFont14
                };

                var underline = new Panel
                {
                    Parent = button,
                    Location = new Point(10, 36),
                    Width = tabWidth - 20,
                    Height = 3,
                    BackgroundColor = new Color(255, 200, 0),
                    Visible = false
                };

                var tab = new TabDefinition
                {
                    Id = id,
                    GetLabel = getLabel,
                    Content = content,
                    Button = button,
                    Label = label,
                    Underline = underline,
                    IsEnabled = setting
                };

                var capturedTab = tab;
                button.LeftMouseButtonReleased += (s, e) =>
                    GameService.Graphics.QueueMainThreadRender(_ => ShowTab(capturedTab));

                _tabs.Add(tab);
            }
        }

        private void RebuildTabBar()
        {
            GameService.Graphics.QueueMainThreadRender(_ =>
            {
                BuildTabBar();
                ShowFirstVisibleTab();
            });
            _ = TryRefreshAsync();
        }

        private void ShowTab(TabDefinition tab)
        {
            // Hide all content panels
            _dailyContent.Visible = false;
            _weeklyRaidsContent.Visible = false;
            _weeklyStrikesContent.Visible = false;

            // Hide all underlines
            foreach (var t in _tabs)
                t.Underline.Visible = false;

            // Show selected
            tab.Content.Visible = true;
            tab.Underline.Visible = true;
            _activeTab = tab;
        }

        private void ShowFirstVisibleTab()
        {
            if (_tabs.Count > 0)
                ShowTab(_tabs[0]);
        }

        private string GetDailyLabel()
        {
            int completed = 0;
            int total = 0;

            if (_moduleSettings.ShowDailyTab.Value && _apiService?.TodaysBounties != null)
            {
                completed += _apiService.TodaysBounties.Count(b => b.IsCompleted);
                total += 4;
            }

            return total > 0 ? $"Daily ({completed}/{total})" : "Daily";
        }

        private string GetRaidsLabel()
        {
            int clearedWings = 0;
            foreach (var wing in RaidData.Wings)
                if (wing.IsFullyCleared) clearedWings++;
            return $"Weekly Raids ({clearedWings}/8 wings)";
        }

        private string GetStrikesLabel()
        {
            int clearedStrikes = 0;
            foreach (var expansion in RaidData.Expansions)
                foreach (var strike in expansion.Strikes)
                    if (strike.IsWeeklyCleared) clearedStrikes++;
            return $"Weekly Strikes ({clearedStrikes}/14)";
        }

        private void UpdateTabLabels()
        {
            foreach (var tab in _tabs)
                tab.Label.Text = tab.GetLabel();
        }

        private async void OnSubtokenUpdated(object sender, ValueEventArgs<IEnumerable<TokenPermission>> e)
        {
            Logger.Info("SubtokenUpdated fired.");
            await TryRefreshAsync();
        }

        private async Task TryRefreshAsync()
        {
            if (_apiService == null || _dailyView == null)
                return;

            GameService.Graphics.QueueMainThreadRender(_ =>
            {
                _mainWindow.Subtitle = "Refreshing...";
            });

            // Only fetch what's enabled
            await _apiService.UpdateAllClearsAsync(
                fetchRaids: _moduleSettings.ShowWeeklyRaids.Value,
                fetchStrikes: _moduleSettings.ShowWeeklyStrikes.Value,
                fetchBounties: _moduleSettings.ShowDailyTab.Value
            );

            GameService.Graphics.QueueMainThreadRender(_ =>
            {
                UpdateTabLabels();

                if (_moduleSettings.ShowDailyTab.Value)
                    _dailyView.Refresh(_apiService.TodaysBounties, _apiService.TomorrowsBounties);

                if (_moduleSettings.ShowWeeklyRaids.Value)
                    _weeklyRaidsView.Refresh();

                if (_moduleSettings.ShowWeeklyStrikes.Value)
                    _weeklyStrikesView.Refresh();

                _lastRefresh = DateTime.Now;
                _mainWindow.Subtitle = $"Refreshed {_lastRefresh:HH:mm}";
            });
        }

        protected override void Unload()
        {
            Logger.Info("Unload called.");
            Gw2ApiManager.SubtokenUpdated -= OnSubtokenUpdated;
            GameService.Input.Keyboard.KeyPressed -= OnKeyPressed;

            if (_moduleSettings != null)
            {
                _moduleSettings.ShowDailyTab.SettingChanged -= OnShowDailyTabChanged;
                _moduleSettings.ShowWeeklyRaids.SettingChanged -= OnShowWeeklyRaidsChanged;
                _moduleSettings.ShowWeeklyStrikes.SettingChanged -= OnShowWeeklyStrikesChanged;
            }

            _cornerIcon?.Dispose();
            _mainWindow?.Dispose();
            _settingsWindow?.Dispose();
            _refreshTimer?.Dispose();
        }
    }
}