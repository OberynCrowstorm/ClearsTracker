namespace OberynsClearsTracker
{
    public class DailyBountySlot
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string WingName { get; set; }
        public string IconId { get; set; }
        public bool IsStrike { get; set; }
        public bool IsCompleted { get; set; }
        public int AchievementId { get; set; }
    }
}