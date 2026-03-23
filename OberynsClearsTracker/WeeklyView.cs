using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;

namespace OberynsClearsTracker
{
    public class WeeklyView : Panel
    {
        private const int IconWidth = 115;
        private const int IconHeight = 86;
        private const int EmblemIconWidth = 130;
        private const int EmblemIconHeight = 98;
        private const int LabelHeight = 20;
        private const int ItemPadding = 6;
        private const int OrbSize = 64;
        private const int OrbSeparatorPadding = 24;
        private const int EmblemWidth = EmblemIconWidth + 20;
        private const int RowHeight = IconHeight + (LabelHeight * 2) + ItemPadding;
        private const int EmblemRowHeight = EmblemIconHeight + (LabelHeight * 2) + ItemPadding;

        // .dat asset IDs for orbs
        private const int OrbLockedAssetId = 2107931;
        private const int OrbSteelAssetId = 2604578; // I  - Steel Echo
        private const int OrbRavenAssetId = 2604579; // II - Raven Echo
        private const int OrbShivepeakAssetId = 2604580; // III - Shiverpeak Echo
        private const int OrbWhisperAssetId = 2604581; // IV - Whisper Echo
        private const int OrbColdAssetId = 2604582; // V  - Cold Echo

        private static readonly string[] OrbTooltips = new[]
        {
            "Steel Echo (Forging Steel)",
            "Raven Echo (V&C, FoJ or BS)",
            "Shiverpeak Echo (Shiverpeaks Pass)",
            "Whisper Echo (WoJ)",
            "Cold Echo (Cold War)"
        };

        private const string RaidIconPath = "icons/raid_icons/";
        private const string StrikeIconPath = "icons/strike_icons/";

        private readonly ContentsManager _contentsManager;
        private readonly bool _showRaids;
        private readonly ForgingSteelPersistence _forgingSteelPersistence;

        private Image[] _wingEmblemImages;
        private Label[] _wingProgressLabels;
        private Image[][] _bossImages;

        private Image[] _expansionEmblemImages;
        private Label[] _expansionProgressLabels;
        private Image[][] _strikeImages;

        // The 5 orb images for the Echo/Hum tracker
        private Image[] _orbImages;

        public WeeklyView(ContentsManager contentsManager, bool showRaids, ForgingSteelPersistence forgingSteelPersistence) : base()
        {
            _contentsManager = contentsManager;
            _showRaids = showRaids;
            _forgingSteelPersistence = forgingSteelPersistence;
        }

        public void Initialize(int width, int height)
        {
            Width = width;
            Height = height;
            BuildLayout();
        }

        private void BuildLayout()
        {
            var mainFlow = new FlowPanel
            {
                Parent = this,
                Width = Width - 20,
                Height = Height,
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                ControlPadding = new Vector2(0, ItemPadding),
                OuterControlPadding = new Vector2(ItemPadding, ItemPadding),
                CanScroll = true
            };

            if (_showRaids)
                BuildRaidWingsSection(mainFlow);
            else
                BuildStrikesSection(mainFlow);
        }

        private void BuildRaidWingsSection(FlowPanel parent)
        {
            new Label
            {
                Parent = parent,
                Text = "Weekly Raid Clears",
                Width = 300,
                Height = 24,
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.White
            };

            var wings = RaidData.Wings;
            _wingEmblemImages = new Image[wings.Length];
            _wingProgressLabels = new Label[wings.Length];
            _bossImages = new Image[wings.Length][];

            for (int w = 0; w < wings.Length; w++)
            {
                var wing = wings[w];
                _bossImages[w] = new Image[wing.Bosses.Length];

                var wingRow = new FlowPanel
                {
                    Parent = parent,
                    Width = Width - 20,
                    Height = EmblemRowHeight,
                    FlowDirection = ControlFlowDirection.SingleLeftToRight,
                    ControlPadding = new Vector2(ItemPadding, 0)
                };

                BuildWingEmblemPanel(wingRow, wing, w);

                for (int b = 0; b < wing.Bosses.Length; b++)
                {
                    var boss = wing.Bosses[b];
                    var iconPath = boss.IsWeeklyCleared
                        ? $"{RaidIconPath}{boss.IconId}_d.png"
                        : $"{RaidIconPath}{boss.IconId}.png";

                    _bossImages[w][b] = BuildIconPanel(wingRow, boss.Abbreviation, iconPath);
                }
            }
        }

        private void BuildStrikesSection(FlowPanel parent)
        {
            new Label
            {
                Parent = parent,
                Text = "Weekly Strike Clears",
                Width = 300,
                Height = 24,
                Font = GameService.Content.DefaultFont16,
                TextColor = Color.White
            };

            var expansions = RaidData.Expansions;
            _expansionEmblemImages = new Image[expansions.Length];
            _expansionProgressLabels = new Label[expansions.Length];
            _strikeImages = new Image[expansions.Length][];

            for (int e = 0; e < expansions.Length; e++)
            {
                var expansion = expansions[e];
                _strikeImages[e] = new Image[expansion.Strikes.Length];

                if (expansion.Abbreviation == "IBS")
                {
                    BuildIBSSection(parent, expansion, e);
                }
                else
                {
                    var expansionRow = new FlowPanel
                    {
                        Parent = parent,
                        Width = Width - 20,
                        Height = EmblemRowHeight,
                        FlowDirection = ControlFlowDirection.SingleLeftToRight,
                        ControlPadding = new Vector2(ItemPadding, 0)
                    };

                    BuildExpansionEmblemPanel(expansionRow, expansion, e);

                    for (int s = 0; s < expansion.Strikes.Length; s++)
                    {
                        var strike = expansion.Strikes[s];
                        var iconPath = strike.IsWeeklyCleared
                            ? $"{StrikeIconPath}{strike.IconId}_d.png"
                            : $"{StrikeIconPath}{strike.IconId}.png";

                        _strikeImages[e][s] = BuildIconPanel(expansionRow, strike.Abbreviation, iconPath);
                    }
                }
            }
        }

        private void BuildIBSSection(FlowPanel parent, ExpansionEntry expansion, int expansionIndex)
        {
            // Outer container holds both rows and the emblem
            var ibsContainer = new Panel
            {
                Parent = parent,
                Width = Width - 20,
                Height = (EmblemRowHeight * 2) + ItemPadding
            };

            // Emblem on the left
            BuildExpansionEmblemPanel(ibsContainer, expansion, expansionIndex, isContainer: true);

            // Right side: two rows stacked
            int rightX = EmblemWidth + ItemPadding;
            int rowWidth = Width - 20 - rightX;

            // Row 1: SP, Voice, FoJ, BS, WoJ (indices 0-4)
            var row1 = new FlowPanel
            {
                Parent = ibsContainer,
                Location = new Point(rightX, 0),
                Width = rowWidth,
                Height = EmblemRowHeight,
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                ControlPadding = new Vector2(ItemPadding, 0)
            };

            // Row 2: FS, CW + orbs (indices 5-6, then orbs)
            var row2 = new FlowPanel
            {
                Parent = ibsContainer,
                Location = new Point(rightX, EmblemRowHeight + ItemPadding),
                Width = rowWidth,
                Height = EmblemRowHeight,
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                ControlPadding = new Vector2(ItemPadding, 0)
            };

            for (int s = 0; s < expansion.Strikes.Length; s++)
            {
                var strike = expansion.Strikes[s];
                var isCleared = strike.ApiId == "forging_steel"
                    ? _forgingSteelPersistence.IsCleared
                    : strike.IsWeeklyCleared;

                var iconPath = isCleared
                    ? $"{StrikeIconPath}{strike.IconId}_d.png"
                    : $"{StrikeIconPath}{strike.IconId}.png";

                var targetRow = s < 5 ? row1 : row2;
                _strikeImages[expansionIndex][s] = BuildIconPanel(targetRow, strike.Abbreviation, iconPath);

                if (strike.ApiId == "forging_steel")
                {
                    var capturedImage = _strikeImages[expansionIndex][s];
                    var capturedExpansionIndex = expansionIndex;
                    capturedImage.LeftMouseButtonReleased += (sender, args) =>
                    {
                        if (_forgingSteelPersistence.IsCleared)
                            _forgingSteelPersistence.MarkUncleared();
                        else
                            _forgingSteelPersistence.MarkCleared();

                        var newPath = _forgingSteelPersistence.IsCleared
                            ? $"{StrikeIconPath}ibs_6_d.png"
                            : $"{StrikeIconPath}ibs_6.png";
                        capturedImage.Texture = _contentsManager.GetTexture(newPath);

                        var orbState = OrbLogic.Calculate(_forgingSteelPersistence);
                        int orbCount = orbState.TotalLit;

                        _expansionEmblemImages[capturedExpansionIndex].Texture = _contentsManager.GetTexture(
                            GetExpansionEmblemPath(expansion, orbCount)
                        ) ?? ContentService.Textures.Pixel;

                        int strikeCount = 0;
                        foreach (var st in expansion.Strikes)
                            if (st.IsWeeklyCleared) strikeCount++;
                        if (_forgingSteelPersistence.IsCleared) strikeCount++;

                        _expansionProgressLabels[capturedExpansionIndex].Text = $"{strikeCount}/{expansion.Strikes.Length}";

                        // Refresh orb visuals when FS is toggled
                        RefreshOrbs(OrbLogic.Calculate(_forgingSteelPersistence));
                    };
                }
            }

            // Separator + orbs at the end of row 2
            BuildOrbSeparator(row2);
            BuildOrbs(row2);
        }

        private void BuildOrbSeparator(FlowPanel parent)
        {
            // A blank panel acting as a visual gap between CW and the orbs
            new Panel
            {
                Parent = parent,
                Width = OrbSeparatorPadding,
                Height = EmblemRowHeight,
                BackgroundColor = Color.Transparent
            };
        }

        private void BuildOrbs(FlowPanel parent)
        {
            var orbState = OrbLogic.Calculate(_forgingSteelPersistence);
            _orbImages = new Image[5];

            bool[] lit = new[]
            {
                orbState.SteelOrb,
                orbState.RavenOrb,
                orbState.ShivepeakOrb,
                orbState.WhisperOrb,
                orbState.ColdOrb
            };

            int[] litAssetIds = new[]
            {
                OrbSteelAssetId,
                OrbRavenAssetId,
                OrbShivepeakAssetId,
                OrbWhisperAssetId,
                OrbColdAssetId
            };

            for (int i = 0; i < 5; i++)
            {
                var orbPanel = new Panel
                {
                    Parent = parent,
                    Width = OrbSize,
                    Height = EmblemRowHeight,
                    BackgroundColor = Color.Transparent
                };

                int assetId = lit[i] ? litAssetIds[i] : OrbLockedAssetId;
                int verticalOffset = (EmblemRowHeight - OrbSize) / 2;

                _orbImages[i] = new Image(GameService.Content.DatAssetCache.GetTextureFromAssetId(assetId))
                {
                    Parent = orbPanel,
                    Size = new Point(OrbSize, OrbSize),
                    Location = new Point(0, verticalOffset),
                    BasicTooltipText = OrbTooltips[i]
                };
            }
        }

        private void RefreshOrbs(OrbState orbState)
        {
            if (_orbImages == null) return;

            bool[] lit = new[]
            {
                orbState.SteelOrb,
                orbState.RavenOrb,
                orbState.ShivepeakOrb,
                orbState.WhisperOrb,
                orbState.ColdOrb
            };

            int[] litAssetIds = new[]
            {
                OrbSteelAssetId,
                OrbRavenAssetId,
                OrbShivepeakAssetId,
                OrbWhisperAssetId,
                OrbColdAssetId
            };

            for (int i = 0; i < 5; i++)
            {
                int assetId = lit[i] ? litAssetIds[i] : OrbLockedAssetId;
                _orbImages[i].Texture = GameService.Content.DatAssetCache.GetTextureFromAssetId(assetId);
            }
        }

        private void BuildWingEmblemPanel(FlowPanel parent, WingEntry wing, int wingIndex)
        {
            var panel = new Panel
            {
                Parent = parent,
                Width = EmblemWidth,
                Height = EmblemRowHeight
            };

            int clearedCount = 0;
            foreach (var boss in wing.Bosses)
                if (boss.IsWeeklyCleared) clearedCount++;

            var iconPath = GetWingEmblemPath(wingIndex + 1, clearedCount, wing.IsFullyCleared);

            _wingEmblemImages[wingIndex] = new Image(_contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel)
            {
                Parent = panel,
                Size = new Point(EmblemIconWidth, EmblemIconHeight),
                Location = new Point((EmblemWidth - EmblemIconWidth) / 2, 0)
            };

            new Label
            {
                Parent = panel,
                Text = wing.Name,
                Location = new Point(0, EmblemIconHeight + 2),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            _wingProgressLabels[wingIndex] = new Label
            {
                Parent = panel,
                Text = $"{clearedCount}/{wing.Bosses.Length}",
                Location = new Point(0, EmblemIconHeight + 2 + LabelHeight),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };
        }

        // Overload for IBS container (parent is Panel, not FlowPanel)
        private void BuildExpansionEmblemPanel(Panel parent, ExpansionEntry expansion, int expansionIndex, bool isContainer)
        {
            var panel = new Panel
            {
                Parent = parent,
                Location = new Point(0, 0),
                Width = EmblemWidth,
                Height = (EmblemRowHeight * 2) + ItemPadding
            };

            int clearedCount = 0;
            foreach (var strike in expansion.Strikes)
                if (strike.IsWeeklyCleared) clearedCount++;

            var iconPath = GetExpansionEmblemPath(expansion, clearedCount);

            _expansionEmblemImages[expansionIndex] = new Image(_contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel)
            {
                Parent = panel,
                Size = new Point(EmblemIconWidth, EmblemIconHeight),
                Location = new Point((EmblemWidth - EmblemIconWidth) / 2, 0)
            };

            new Label
            {
                Parent = panel,
                Text = expansion.Name,
                Location = new Point(0, EmblemIconHeight + 2),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            _expansionProgressLabels[expansionIndex] = new Label
            {
                Parent = panel,
                Text = $"{clearedCount}/{expansion.Strikes.Length}",
                Location = new Point(0, EmblemIconHeight + 2 + LabelHeight),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };
        }

        // Original overload for non-IBS expansions (parent is FlowPanel)
        private void BuildExpansionEmblemPanel(FlowPanel parent, ExpansionEntry expansion, int expansionIndex)
        {
            var panel = new Panel
            {
                Parent = parent,
                Width = EmblemWidth,
                Height = EmblemRowHeight
            };

            int clearedCount = 0;
            foreach (var strike in expansion.Strikes)
                if (strike.IsWeeklyCleared) clearedCount++;

            var iconPath = GetExpansionEmblemPath(expansion, clearedCount);

            _expansionEmblemImages[expansionIndex] = new Image(_contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel)
            {
                Parent = panel,
                Size = new Point(EmblemIconWidth, EmblemIconHeight),
                Location = new Point((EmblemWidth - EmblemIconWidth) / 2, 0)
            };

            new Label
            {
                Parent = panel,
                Text = expansion.Name,
                Location = new Point(0, EmblemIconHeight + 2),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            _expansionProgressLabels[expansionIndex] = new Label
            {
                Parent = panel,
                Text = $"{clearedCount}/{expansion.Strikes.Length}",
                Location = new Point(0, EmblemIconHeight + 2 + LabelHeight),
                Width = EmblemWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };
        }

        private Image BuildIconPanel(FlowPanel parent, string label, string iconPath)
        {
            var panel = new Panel
            {
                Parent = parent,
                Width = IconWidth,
                Height = EmblemRowHeight
            };

            var image = new Image(_contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel)
            {
                Parent = panel,
                Size = new Point(IconWidth, IconHeight),
                Location = new Point(0, (EmblemIconHeight - IconHeight) / 2)
            };

            new Label
            {
                Parent = panel,
                Text = label,
                Location = new Point(0, EmblemIconHeight + 2 + ((EmblemIconHeight - IconHeight) / 2)),
                Width = IconWidth,
                Height = LabelHeight,
                WrapText = false,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextColor = Color.White,
                Font = GameService.Content.DefaultFont14
            };

            return image;
        }

        private string GetWingEmblemPath(int wingNumber, int clearedCount, bool isFullyCleared)
        {
            if (isFullyCleared)
                return $"{RaidIconPath}wing{wingNumber}_d.png";
            return $"{RaidIconPath}wing{wingNumber}_{clearedCount}.png";
        }

        private string GetExpansionEmblemPath(ExpansionEntry expansion, int clearedCount)
        {
            if (expansion.Abbreviation == "IBS")
            {
                var orbState = OrbLogic.Calculate(_forgingSteelPersistence);
                if (orbState.AllLit)
                    return $"{StrikeIconPath}{expansion.IconId}_d.png";
                return $"{StrikeIconPath}{expansion.IconId}_{orbState.TotalLit}.png";
            }

            if (expansion.IsFullyCleared)
                return $"{StrikeIconPath}{expansion.IconId}_d.png";

            return $"{StrikeIconPath}{expansion.IconId}_{clearedCount}.png";
        }

        public void Refresh()
        {
            if (_showRaids)
                RefreshRaids();
            else
                RefreshStrikes();
        }

        private void RefreshRaids()
        {
            if (_wingEmblemImages == null) return;

            var wings = RaidData.Wings;
            for (int w = 0; w < wings.Length; w++)
            {
                var wing = wings[w];

                int clearedCount = 0;
                foreach (var boss in wing.Bosses)
                    if (boss.IsWeeklyCleared) clearedCount++;

                _wingEmblemImages[w].Texture = _contentsManager.GetTexture(
                    GetWingEmblemPath(w + 1, clearedCount, wing.IsFullyCleared)
                ) ?? ContentService.Textures.Pixel;

                _wingProgressLabels[w].Text = $"{clearedCount}/{wing.Bosses.Length}";

                for (int b = 0; b < wing.Bosses.Length; b++)
                {
                    var boss = wing.Bosses[b];
                    var iconPath = boss.IsWeeklyCleared
                        ? $"{RaidIconPath}{boss.IconId}_d.png"
                        : $"{RaidIconPath}{boss.IconId}.png";
                    _bossImages[w][b].Texture = _contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel;
                }
            }
        }

        private void RefreshStrikes()
        {
            if (_expansionEmblemImages == null) return;

            var expansions = RaidData.Expansions;
            for (int e = 0; e < expansions.Length; e++)
            {
                var expansion = expansions[e];

                int clearedCount = 0;
                foreach (var strike in expansion.Strikes)
                    if (strike.IsWeeklyCleared) clearedCount++;

                _expansionEmblemImages[e].Texture = _contentsManager.GetTexture(
                    GetExpansionEmblemPath(expansion, clearedCount)
                ) ?? ContentService.Textures.Pixel;

                _expansionProgressLabels[e].Text = $"{clearedCount}/{expansion.Strikes.Length}";

                for (int s = 0; s < expansion.Strikes.Length; s++)
                {
                    var strike = expansion.Strikes[s];
                    var isCleared = strike.ApiId == "forging_steel"
                        ? _forgingSteelPersistence.IsCleared
                        : strike.IsWeeklyCleared;

                    var iconPath = isCleared
                        ? $"{StrikeIconPath}{strike.IconId}_d.png"
                        : $"{StrikeIconPath}{strike.IconId}.png";
                    _strikeImages[e][s].Texture = _contentsManager.GetTexture(iconPath) ?? ContentService.Textures.Pixel;
                }
            }

            // Refresh orbs on every strike refresh
            RefreshOrbs(OrbLogic.Calculate(_forgingSteelPersistence));
        }
    }
}