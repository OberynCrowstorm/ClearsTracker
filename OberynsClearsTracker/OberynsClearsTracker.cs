using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

        private ApiService _apiService;
        private ForgingSteelPersistence _forgingSteelPersistence;

        private CornerIcon _cornerIcon;
        private StandardWindow _mainWindow;

        // Tab buttons
        private Panel _dailyTabButton;
        private Panel _weeklyRaidsTabButton;
        private Panel _weeklyStrikesTabButton;

        // Tab labels
        private Label _dailyTabLabel;
        private Label _weeklyRaidsTabLabel;
        private Label _weeklyStrikesTabLabel;

        // Tab underlines
        private Panel _dailyTabUnderline;
        private Panel _weeklyRaidsTabUnderline;
        private Panel _weeklyStrikesTabUnderline;

        // Content panels
        private Panel _dailyContent;
        private Panel _weeklyRaidsContent;
        private Panel _weeklyStrikesContent;

        // Views
        private DailyView _dailyView;
        private WeeklyView _weeklyRaidsView;
        private WeeklyView _weeklyStrikesView;

        internal Gw2ApiManager Gw2ApiManager => this.ModuleParameters.Gw2ApiManager;
        internal ContentsManager ContentsManager => this.ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => this.ModuleParameters.DirectoriesManager;

        [ImportingConstructor]
        public OberynsClearsTracker([Import("ModuleParameters")] ModuleParameters moduleParameters)
            : base(moduleParameters)
        {
            Logger.Info("Module constructor called.");
        }

        protected override async Task LoadAsync()
        {
            Logger.Info("LoadAsync started.");

            _forgingSteelPersistence = ForgingSteelPersistence.Load(DirectoriesManager);
            _apiService = new ApiService(Gw2ApiManager);

            BuildWindow();

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
                    _mainWindow.Show();
            };

            Gw2ApiManager.SubtokenUpdated += OnSubtokenUpdated;

            await TryRefreshAsync();

            Logger.Info("LoadAsync finished.");
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

            await _apiService.UpdateAllClearsAsync();

            GameService.Graphics.QueueMainThreadRender(_ =>
            {
                if (GameService.GameIntegration.Gw2Instance.IsInGame)
                    //  _mainWindow.Show();
                   
                UpdateTabLabels();
                _dailyView.Refresh(_apiService.TodaysBounties, _apiService.TomorrowsBounties);
                _weeklyRaidsView.Refresh();
                _weeklyStrikesView.Refresh();
            });
        }

        private void UpdateTabLabels()
        {
            int dailyCompleted = _apiService.TodaysBounties.Count(b => b.IsCompleted);
            _dailyTabLabel.Text = $"Daily ({dailyCompleted}/4)";

            int clearedWings = 0;
            foreach (var wing in RaidData.Wings)
                if (wing.IsFullyCleared) clearedWings++;
            _weeklyRaidsTabLabel.Text = $"Weekly Raids ({clearedWings}/8 wings)";

            int clearedStrikes = 0;
            foreach (var expansion in RaidData.Expansions)
                foreach (var strike in expansion.Strikes)
                    if (strike.IsWeeklyCleared) clearedStrikes++;
            _weeklyStrikesTabLabel.Text = $"Weekly Strikes ({clearedStrikes}/14)";
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
                Subtitle = "Oberyn",
                Location = new Point(50, 50),
                SavesPosition = true,
                Id = "ClearsTracker_MainWindow"
            };

            BuildTabBar();
            BuildContentPanels();

            ShowTab(_dailyTabButton, _dailyContent);

            // Only show window if GW2 is running
            //if (GameService.GameIntegration.Gw2Instance.IsInGame)
            //    _mainWindow.Show();
        }

        private void BuildTabBar()
        {
            var tabBar = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, 0),
                Width = 840,
                Height = 40,
                BackgroundColor = Color.Black * 0.3f
            };

            int tabWidth = 270;

            _dailyTabButton = BuildTabButton(tabBar, "Daily (0/4)", 0, tabWidth, out _dailyTabLabel, out _dailyTabUnderline);
            _dailyTabButton.LeftMouseButtonReleased += (s, e) =>
                GameService.Graphics.QueueMainThreadRender(_ => ShowTab(_dailyTabButton, _dailyContent));

            _weeklyRaidsTabButton = BuildTabButton(tabBar, "Weekly Raids (0/8 wings)", tabWidth, tabWidth, out _weeklyRaidsTabLabel, out _weeklyRaidsTabUnderline);
            _weeklyRaidsTabButton.LeftMouseButtonReleased += (s, e) =>
                GameService.Graphics.QueueMainThreadRender(_ => ShowTab(_weeklyRaidsTabButton, _weeklyRaidsContent));

            _weeklyStrikesTabButton = BuildTabButton(tabBar, "Weekly Strikes (0/14)", tabWidth * 2, tabWidth, out _weeklyStrikesTabLabel, out _weeklyStrikesTabUnderline);
            _weeklyStrikesTabButton.LeftMouseButtonReleased += (s, e) =>
                GameService.Graphics.QueueMainThreadRender(_ => ShowTab(_weeklyStrikesTabButton, _weeklyStrikesContent));
        }

        private Panel BuildTabButton(Panel parent, string text, int x, int width, out Label label, out Panel underline)
        {
            var button = new Panel
            {
                Parent = parent,
                Location = new Point(x, 0),
                Width = width,
                Height = 40,
                BackgroundColor = Color.Transparent
            };

            label = new Label
            {
                Parent = button,
                Text = text,
                Width = width,
                Height = 34,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Middle,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            underline = new Panel
            {
                Parent = button,
                Location = new Point(10, 36),
                Width = width - 20,
                Height = 3,
                BackgroundColor = new Color(255, 200, 0),
                Visible = false
            };

            return button;
        }

        private void BuildContentPanels()
        {
            _dailyContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, 45),
                Width = 840,
                Height = 650,
                Visible = false
            };

            _weeklyRaidsContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, 45),
                Width = 840,
                Height = 620,
                Visible = false
            };

            _weeklyStrikesContent = new Panel
            {
                Parent = _mainWindow,
                Location = new Point(0, 45),
                Width = 840,
                Height = 620,
                Visible = false
            };

            _dailyView = new DailyView(ContentsManager);
            _dailyView.Parent = _dailyContent;
            _dailyView.Width = 840;
            _dailyView.Height = 620;

            _weeklyRaidsView = new WeeklyView(ContentsManager, showRaids: true, _forgingSteelPersistence);
            _weeklyRaidsView.Parent = _weeklyRaidsContent;
            _weeklyRaidsView.Initialize(840, 620);

            _weeklyStrikesView = new WeeklyView(ContentsManager, showRaids: false, _forgingSteelPersistence);
            _weeklyStrikesView.Parent = _weeklyStrikesContent;
            _weeklyStrikesView.Initialize(840, 620);
        }

        private void ShowTab(Panel tabButton, Panel content)
        {
            _dailyContent.Visible = false;
            _weeklyRaidsContent.Visible = false;
            _weeklyStrikesContent.Visible = false;

            _dailyTabUnderline.Visible = false;
            _weeklyRaidsTabUnderline.Visible = false;
            _weeklyStrikesTabUnderline.Visible = false;

            content.Visible = true;

            if (tabButton == _dailyTabButton)
                _dailyTabUnderline.Visible = true;
            else if (tabButton == _weeklyRaidsTabButton)
                _weeklyRaidsTabUnderline.Visible = true;
            else if (tabButton == _weeklyStrikesTabButton)
                _weeklyStrikesTabUnderline.Visible = true;
        }

        protected override void Unload()
        {
            Logger.Info("Unload called.");
            Gw2ApiManager.SubtokenUpdated -= OnSubtokenUpdated;
            _cornerIcon?.Dispose();
            _mainWindow?.Dispose();
        }
    }
}