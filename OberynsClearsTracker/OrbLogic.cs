namespace OberynsClearsTracker
{
    public class OrbState
    {
        public bool SteelOrb { get; set; }      // Forging Steel (manual)
        public bool RavenOrb { get; set; }       // V&C OR Fraenir OR Boneskinner
        public bool ShivepeakOrb { get; set; }   // Shiverpeaks Pass
        public bool WhisperOrb { get; set; }     // Whisper of Jormag
        public bool ColdOrb { get; set; }        // Cold War

        public int TotalLit
        {
            get
            {
                int count = 0;
                if (SteelOrb) count++;
                if (RavenOrb) count++;
                if (ShivepeakOrb) count++;
                if (WhisperOrb) count++;
                if (ColdOrb) count++;
                return count;
            }
        }

        public bool AllLit => TotalLit == 5;
    }

    public static class OrbLogic
    {
        public static OrbState Calculate(ForgingSteelPersistence forgingSteel)
        {
            // Find the IBS expansion
            ExpansionEntry ibs = null;
            foreach (var expansion in RaidData.Expansions)
            {
                if (expansion.Abbreviation == "IBS")
                {
                    ibs = expansion;
                    break;
                }
            }

            if (ibs == null)
                return new OrbState();

            // Find individual strikes by ApiId
            bool shiverpeaksCleared = false;
            bool voiceAndClawCleared = false;
            bool fraenirCleared = false;
            bool whisperCleared = false;
            bool boneskinnerCleared = false;
            bool coldWarCleared = false;

            foreach (var strike in ibs.Strikes)
            {
                switch (strike.ApiId)
                {
                    case "shiverpeak_pass": shiverpeaksCleared = strike.IsWeeklyCleared; break;
                    case "voice_and_claw": voiceAndClawCleared = strike.IsWeeklyCleared; break;
                    case "fraenir_of_jormag": fraenirCleared = strike.IsWeeklyCleared; break;
                    case "whisper_of_jormag": whisperCleared = strike.IsWeeklyCleared; break;
                    case "boneskinner": boneskinnerCleared = strike.IsWeeklyCleared; break;
                    case "cold_war": coldWarCleared = strike.IsWeeklyCleared; break;
                }
            }

            return new OrbState
            {
                SteelOrb = forgingSteel.IsCleared,
                RavenOrb = voiceAndClawCleared || fraenirCleared || boneskinnerCleared,
                ShivepeakOrb = shiverpeaksCleared,
                WhisperOrb = whisperCleared,
                ColdOrb = coldWarCleared
            };
        }
    }
}