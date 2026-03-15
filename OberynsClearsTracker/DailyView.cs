using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace OberynsClearsTracker
{
    public class DailyView : Panel
    {
        private const int IconWidth = 85;
        private const int IconHeight = 64;
        private const int LabelHeight = 20;
        private const int ItemPadding = 6;

        private const string RaidIconPath = "icons/raid_icons/";
        private const string StrikeIconPath = "icons/strike_icons/";

        private readonly ContentsManager _contentsManager;

        private Label _loadingLabel;

        private Label _todayHeader;
        private Panel _todayPanel;
        private Image[] _todayImages;
        private Label[] _todayLabels;

        private Label _tomorrowHeader;
        private Panel _tomorrowPanel;
        private Image[] _tomorrowImages;
        private Label[] _tomorrowLabels;

        public DailyView(ContentsManager contentsManager) : base()
        {
            _contentsManager = contentsManager;
            BuildLayout();
        }

        private void BuildLayout()
        {
            _loadingLabel = new Label
            {
                Parent = this,
                Text = "Loading daily bounties...",
                Location = new Point(ItemPadding, ItemPadding),
                Width = 300,
                Height = 40,
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.White,
                Visible = true
            };

            // Today's header
            _todayHeader = new Label
            {
                Parent = this,
                Text = "Daily Raid Bounties",
                Location = new Point(ItemPadding, ItemPadding),
                Width = 300,
                Height = 24,
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.White,
                Visible = false
            };

            // Today's icons panel
            _todayPanel = new Panel
            {
                Parent = this,
                Location = new Point(ItemPadding, 30),
                Width = (IconWidth + ItemPadding) * 4,
                Height = IconHeight + LabelHeight + ItemPadding,
                Visible = false
            };

            _todayImages = new Image[4];
            _todayLabels = new Label[4];

            for (int i = 0; i < 4; i++)
            {
                int x = i * (IconWidth + ItemPadding);

                _todayImages[i] = new Image
                {
                    Parent = _todayPanel,
                    Size = new Point(IconWidth, IconHeight),
                    Location = new Point(x, 0)
                };

                _todayLabels[i] = new Label
                {
                    Parent = _todayPanel,
                    Text = "",
                    Location = new Point(x, IconHeight + 2),
                    Width = IconWidth,
                    Height = LabelHeight,
                    WrapText = false,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextColor = Color.White
                };
            }

            // Tomorrow's header — positioned to the right of today's panel
            int tomorrowX = ItemPadding + (IconWidth + ItemPadding) * 4 + 20;

            _tomorrowHeader = new Label
            {
                Parent = this,
                Text = "Tomorrow's Bounties",
                Location = new Point(tomorrowX, ItemPadding),
                Width = 300,
                Height = 24,
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.White,
                Visible = false
            };

            // Tomorrow's icons panel
            _tomorrowPanel = new Panel
            {
                Parent = this,
                Location = new Point(tomorrowX, 30),
                Width = (IconWidth + ItemPadding) * 4,
                Height = IconHeight + LabelHeight + ItemPadding,
                Visible = false
            };

            _tomorrowImages = new Image[4];
            _tomorrowLabels = new Label[4];

            for (int i = 0; i < 4; i++)
            {
                int x = i * (IconWidth + ItemPadding);

                _tomorrowImages[i] = new Image
                {
                    Parent = _tomorrowPanel,
                    Size = new Point(IconWidth, IconHeight),
                    Location = new Point(x, 0)
                };

                _tomorrowLabels[i] = new Label
                {
                    Parent = _tomorrowPanel,
                    Text = "",
                    Location = new Point(x, IconHeight + 2),
                    Width = IconWidth,
                    Height = LabelHeight,
                    WrapText = false,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextColor = Color.White
                };
            }
        }

        public void Refresh(List<DailyBountySlot> todaysBounties, List<DailyBountySlot> tomorrowsBounties)
        {
            if (todaysBounties == null || todaysBounties.Count == 0)
            {
                _loadingLabel.Visible = true;
                _todayHeader.Visible = false;
                _todayPanel.Visible = false;
                _tomorrowHeader.Visible = false;
                _tomorrowPanel.Visible = false;
                return;
            }

            _loadingLabel.Visible = false;
            _todayHeader.Visible = true;
            _todayPanel.Visible = true;
            _tomorrowHeader.Visible = true;
            _tomorrowPanel.Visible = true;

            // Populate today's icons
            for (int i = 0; i < 4 && i < todaysBounties.Count; i++)
            {
                var bounty = todaysBounties[i];
                var folder = bounty.IsStrike ? StrikeIconPath : RaidIconPath;
                var iconPath = bounty.IsCompleted
                    ? $"{folder}{bounty.IconId}_d.png"
                    : $"{folder}{bounty.IconId}.png";

                _todayImages[i].Texture = _contentsManager.GetTexture(iconPath);
                _todayImages[i].BasicTooltipText = bounty.WingName;
                _todayLabels[i].Text = bounty.Abbreviation;
            }

            // Populate tomorrow's icons — never completed
            for (int i = 0; i < 4 && i < tomorrowsBounties.Count; i++)
            {
                var bounty = tomorrowsBounties[i];
                var folder = bounty.IsStrike ? StrikeIconPath : RaidIconPath;
                var iconPath = $"{folder}{bounty.IconId}.png";

                _tomorrowImages[i].Texture = _contentsManager.GetTexture(iconPath);
                _tomorrowImages[i].BasicTooltipText = bounty.WingName;
                _tomorrowLabels[i].Text = bounty.Abbreviation;
            }
        }
    }
}