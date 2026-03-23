using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OberynsClearsTracker
{
    public static class RaidData
    {
        public static WingEntry[] Wings = new WingEntry[]
        {
            // Wing 1 - Spirit Vale
            new WingEntry
            {
                Name = "Spirit Vale",
                Abbreviation = "W1",
                IconId = "wing1",
                Bosses = new[]
                {
                    new BossEntry { Name = "Vale Guardian",  Abbreviation = "VG",       IconId = "w1_1", ApiId = "vale_guardian",  DailyBountyAchievementId = 9126 },
                    new BossEntry { Name = "Spirit Woods",   Abbreviation = "SW",       IconId = "w1_2", ApiId = "spirit_woods",   DailyBountyAchievementId = null },
                    new BossEntry { Name = "Gorseval",       Abbreviation = "Gorseval", IconId = "w1_3", ApiId = "gorseval",       DailyBountyAchievementId = 9122 },
                    new BossEntry { Name = "Sabetha",        Abbreviation = "Sabetha",  IconId = "w1_4", ApiId = "sabetha",        DailyBountyAchievementId = 9185 },
                }
            },

            // Wing 2 - Salvation Pass
            new WingEntry
            {
                Name = "Salvation Pass",
                Abbreviation = "W2",
                IconId = "wing2",
                Bosses = new[]
                {
                    new BossEntry { Name = "Slothasor",    Abbreviation = "Slothasor", IconId = "w2_1", ApiId = "slothasor",   DailyBountyAchievementId = 9135 },
                    new BossEntry { Name = "Prison Camp",  Abbreviation = "PC",        IconId = "w2_2", ApiId = "bandit_trio", DailyBountyAchievementId = null },
                    new BossEntry { Name = "Matthias",     Abbreviation = "Matthias",  IconId = "w2_3", ApiId = "matthias",    DailyBountyAchievementId = 9160 },
                }
            },

            // Wing 3 - Stronghold of the Faithful
            new WingEntry
            {
                Name = "Stronghold of the Faithful",
                Abbreviation = "W3",
                IconId = "wing3",
                Bosses = new[]
                {
                    new BossEntry { Name = "Escort",         Abbreviation = "Escort", IconId = "w3_1", ApiId = "escort",         DailyBountyAchievementId = null },
                    new BossEntry { Name = "Keep Construct", Abbreviation = "KC",     IconId = "w3_2", ApiId = "keep_construct", DailyBountyAchievementId = 9163 },
                    new BossEntry { Name = "Twisted Castle", Abbreviation = "Castle", IconId = "w3_3", ApiId = "twisted_castle", DailyBountyAchievementId = null },
                    new BossEntry { Name = "Xera",           Abbreviation = "Xera",   IconId = "w3_4", ApiId = "xera",           DailyBountyAchievementId = 9107 },
                }
            },

            // Wing 4 - Bastion of the Penitent
            new WingEntry
            {
                Name = "Bastion of the Penitent",
                Abbreviation = "W4",
                IconId = "wing4",
                Bosses = new[]
                {
                    new BossEntry { Name = "Cairn",           Abbreviation = "Cairn",   IconId = "w4_1", ApiId = "cairn",           DailyBountyAchievementId = 9109 },
                    new BossEntry { Name = "Mursaat Overseer",Abbreviation = "Mursaat", IconId = "w4_2", ApiId = "mursaat_overseer", DailyBountyAchievementId = 9192 },
                    new BossEntry { Name = "Samarog",         Abbreviation = "Samarog", IconId = "w4_3", ApiId = "samarog",          DailyBountyAchievementId = 9103 },
                    new BossEntry { Name = "Deimos",          Abbreviation = "Deimos",  IconId = "w4_4", ApiId = "deimos",           DailyBountyAchievementId = 9176 },
                }
            },

            // Wing 5 - Hall of Chains
            new WingEntry
            {
                Name = "Hall of Chains",
                Abbreviation = "W5",
                IconId = "wing5",
                Bosses = new[]
                {
                    new BossEntry { Name = "Soulless Horror",  Abbreviation = "Horror",  IconId = "w5_1", ApiId = "soulless_horror",  DailyBountyAchievementId = 9146 },
                    new BossEntry { Name = "River of Souls",   Abbreviation = "River",   IconId = "w5_2", ApiId = "river_of_souls",   DailyBountyAchievementId = null },
                    new BossEntry { Name = "Statues of Grenth",Abbreviation = "Statues", IconId = "w5_3", ApiId = "statues_of_grenth",DailyBountyAchievementId = null },
                    new BossEntry { Name = "Dhuum",            Abbreviation = "Dhuum",   IconId = "w5_4", ApiId = "voice_in_the_void",DailyBountyAchievementId = 9191 },
                }
            },

            // Wing 6 - Mythwright Gambit
            new WingEntry
            {
                Name = "Mythwright Gambit",
                Abbreviation = "W6",
                IconId = "wing6",
                Bosses = new[]
                {
                    new BossEntry { Name = "Conjured Amalgamate", Abbreviation = "CA",     IconId = "w6_1", ApiId = "conjured_amalgamate", DailyBountyAchievementId = 9119 },
                    new BossEntry { Name = "Twin Largos",         Abbreviation = "Twins",  IconId = "w6_2", ApiId = "twin_largos",         DailyBountyAchievementId = 9162 },
                    new BossEntry { Name = "Qadim",               Abbreviation = "Qadim1", IconId = "w6_3", ApiId = "qadim",               DailyBountyAchievementId = 9201 },
                }
            },

            // Wing 7 - The Key of Ahdashim
            new WingEntry
            {
                Name = "The Key of Ahdashim",
                Abbreviation = "W7",
                IconId = "wing7",
                Bosses = new[]
                {
                    new BossEntry { Name = "Gate",               Abbreviation = "Gate",   IconId = "w7_1", ApiId = "gate",               DailyBountyAchievementId = null },
                    new BossEntry { Name = "Cardinal Adina",     Abbreviation = "Adina",  IconId = "w7_2", ApiId = "adina",              DailyBountyAchievementId = 9145 },
                    new BossEntry { Name = "Cardinal Sabir",     Abbreviation = "Sabir",  IconId = "w7_3", ApiId = "sabir",              DailyBountyAchievementId = 9127 },
                    new BossEntry { Name = "Qadim the Peerless", Abbreviation = "Qadim2", IconId = "w7_4", ApiId = "qadim_the_peerless", DailyBountyAchievementId = 9124 },
                }
            },

            // Wing 8 - Mount Balrior
            new WingEntry
            {
                Name = "Mount Balrior",
                Abbreviation = "W8",
                IconId = "wing8",
                Bosses = new[]
                {
                    new BossEntry { Name = "Camp",   Abbreviation = "Camp",   IconId = "w8_1", ApiId = "camp",  DailyBountyAchievementId = null },
                    new BossEntry { Name = "Decima", Abbreviation = "Decima", IconId = "w8_2", ApiId = "decima",DailyBountyAchievementId = 9137 },
                    new BossEntry { Name = "Greer",  Abbreviation = "Greer",  IconId = "w8_3", ApiId = "greer", DailyBountyAchievementId = 9161 },
                    new BossEntry { Name = "Ura",    Abbreviation = "Ura",    IconId = "w8_4", ApiId = "ura",   DailyBountyAchievementId = 9189 },
                }
            },
        };

        public static ExpansionEntry[] Expansions = new ExpansionEntry[]
        {
            // IBS - Icebrood Saga
            new ExpansionEntry
            {
                Name = "Icebrood Saga",
                Abbreviation = "IBS",
                IconId = "ibs_weekly",
                Strikes = new[]
                {
                    new StrikeEntry { Name = "Shiverpeaks Pass", Abbreviation = "SP",    IconId = "ibs_1", ApiId = "shiverpeak_pass",   DailyBountyAchievementId = 9121 },
                    new StrikeEntry { Name = "Voice and Claw",   Abbreviation = "Voice", IconId = "ibs_2", ApiId = "voice_and_claw",    DailyBountyAchievementId = 9175 },
                    new StrikeEntry { Name = "Fraenir of Jormag",Abbreviation = "FoJ",   IconId = "ibs_3", ApiId = "fraenir_of_jormag", DailyBountyAchievementId = 9139 },
                    new StrikeEntry { Name = "Boneskinner",      Abbreviation = "BS",    IconId = "ibs_4", ApiId = "boneskinner",       DailyBountyAchievementId = 9197 },
                    new StrikeEntry { Name = "Whisper of Jormag",Abbreviation = "WoJ",   IconId = "ibs_5", ApiId = "whisper_of_jormag", DailyBountyAchievementId = 9131 },
                    new StrikeEntry { Name = "Forging Steel",    Abbreviation = "FS",    IconId = "ibs_6", ApiId = "forging_steel",     DailyBountyAchievementId = null },
                    new StrikeEntry { Name = "Cold War",         Abbreviation = "CW",    IconId = "ibs_7", ApiId = "cold_war",          DailyBountyAchievementId = 9115 },
                }
            },

            // EoD - End of Dragons
            new ExpansionEntry
            {
                Name = "End of Dragons",
                Abbreviation = "EoD",
                IconId = "eod_weekly",
                Strikes = new[]
                {
                    new StrikeEntry { Name = "Aetherblade Hideout", Abbreviation = "AH",  IconId = "eod_1", ApiId = "aetherblade_hideout",  DailyBountyAchievementId = 9153 },
                    new StrikeEntry { Name = "Xunlai Jade Junkyard",Abbreviation = "XJJ", IconId = "eod_2", ApiId = "xunlai_jade_junkyard", DailyBountyAchievementId = 9196 },
                    new StrikeEntry { Name = "Kaineng Overlook",    Abbreviation = "KO",  IconId = "eod_3", ApiId = "kaineng_overlook",     DailyBountyAchievementId = 9179 },
                    new StrikeEntry { Name = "Harvest Temple",      Abbreviation = "HT",  IconId = "eod_4", ApiId = "harvest_temple",       DailyBountyAchievementId = 9168 },
                    new StrikeEntry { Name = "Old Lion's Court",    Abbreviation = "OLC", IconId = "eod_5", ApiId = "old_lion_court",       DailyBountyAchievementId = 9138 },
                }
            },

            // SotO - Secrets of the Obscure
            new ExpansionEntry
            {
                Name = "Secrets of the Obscure",
                Abbreviation = "SotO",
                IconId = "soto_weekly",
                Strikes = new[]
                {
                    new StrikeEntry { Name = "Cosmic Observatory", Abbreviation = "CO",  IconId = "soto_1", ApiId = "cosmic_observatory", DailyBountyAchievementId = 9155 },
                    new StrikeEntry { Name = "Temple of Febe",     Abbreviation = "ToF", IconId = "soto_2", ApiId = "temple_of_febe",     DailyBountyAchievementId = 9134 },
                }
            },

            // VoE - Visions of Eternity
            new ExpansionEntry
            {
                Name = "Visions of Eternity",
                Abbreviation = "VoE",
                IconId = "voe_weekly",
                Strikes = new[]
                {
                    new StrikeEntry { Name = "Guardian's Glade", Abbreviation = "Kela", IconId = "voe_1", ApiId = "guardians_glade", DailyBountyAchievementId = 9158 },
                }
            },
        };

        public const int WeeklyStrikeAchievementId = 9125;

        public static readonly string[] WeeklyStrikeBitOrder = new string[]
        {
            "shiverpeak_pass",       // bit 0
            "fraenir_of_jormag",     // bit 1
            "voice_and_claw",        // bit 2
            "whisper_of_jormag",     // bit 3
            "boneskinner",           // bit 4
            "cold_war",              // bit 5
            "aetherblade_hideout",   // bit 6
            "xunlai_jade_junkyard",  // bit 7
            "kaineng_overlook",      // bit 8
            "harvest_temple",        // bit 9
            "cosmic_observatory",    // bit 10
            "temple_of_febe",        // bit 11
            "old_lion_court",        // bit 12
            "guardians_glade",       // bit 13
        };
    }
}