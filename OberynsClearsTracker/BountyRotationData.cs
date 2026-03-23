namespace OberynsClearsTracker
{
    public static class BountyRotationData
    {
        // Each array is one boss slot. Index 0 = day 1, wraps via modulo.
        // Source: https://wiki.guildwars2.com/wiki/Daily_Raid_Bounties

        public static readonly string[] Slot1 = new[]
        {
            "shiverpeak_pass",
            "voice_and_claw",
            "fraenir_of_jormag",
            "gorseval",
            "cairn",
            "mursaat_overseer"
        };

        public static readonly string[] Slot2 = new[]
        {
            "aetherblade_hideout",
            "sabir",
            "whisper_of_jormag",
            "vale_guardian",
            "cosmic_observatory",
            "cold_war",
            "boneskinner",
            "sabetha",
            "xunlai_jade_junkyard",
            "temple_of_febe",
            "keep_construct",
            "guardians_glade"
        };

        public static readonly string[] Slot3 = new[]
        {
            "slothasor",
            "matthias",
            "xera",
            "samarog",
            "conjured_amalgamate",
            "twin_largos",
            "decima",
            "adina",
            "old_lion_court",
            "ura",
            "kaineng_overlook",
            "deimos"
        };

        public static readonly string[] Slot4 = new[]
        {
            "qadim",
            "qadim_the_peerless",
            "soulless_horror",
            "harvest_temple",
            "voice_in_the_void",
            "greer"
        };
    }
}