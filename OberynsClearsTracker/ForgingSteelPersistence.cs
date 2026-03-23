using System;
using System.IO;
using System.Text;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;

namespace OberynsClearsTracker
{
    public class ForgingSteelPersistence
    {
        private static readonly Logger Logger = Logger.GetLogger<ForgingSteelPersistence>();
        private const string FILENAME = "forging_steel.json";

        private readonly DirectoriesManager _directoriesManager;

        [JsonProperty("clearedAt")]
        public DateTime? ClearedAt { get; set; }

        public ForgingSteelPersistence(DirectoriesManager directoriesManager)
        {
            _directoriesManager = directoriesManager;
        }

        public bool IsCleared
        {
            get
            {
                if (!ClearedAt.HasValue)
                    return false;

                return ClearedAt.Value > GetLastWeeklyReset();
            }
        }

        public void MarkCleared()
        {
            ClearedAt = DateTime.UtcNow;
            Save();
        }

        public void MarkUncleared()
        {
            ClearedAt = null;
            Save();
        }

        private static DateTime GetLastWeeklyReset()
        {
            var now = DateTime.UtcNow;

            int daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            var lastMonday = now.Date.AddDays(-daysUntilMonday);
            var lastReset = lastMonday.AddHours(7).AddMinutes(30);

            if (lastReset > now)
                lastReset = lastReset.AddDays(-7);

            return lastReset;
        }

        private FileInfo GetConfigFileInfo()
        {
            var dir = _directoriesManager.GetFullDirectoryPath("dailyraidtracker");
            return new FileInfo(Path.Combine(dir, FILENAME));
        }

        public void Save()
        {
            try
            {
                var fileInfo = GetConfigFileInfo();
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var writer = new StreamWriter(fileInfo.FullName, false, Encoding.UTF8))
                {
                    writer.Write(json);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Warn(ex, "Access denied saving Forging Steel persistence.");
                Blish_HUD.Debug.Contingency.NotifyFileSaveAccessDenied(
                    GetConfigFileInfo().FullName,
                    "Forging Steel clear state could not be saved."
                );
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to save Forging Steel persistence.");
            }
        }

        public static ForgingSteelPersistence Load(DirectoriesManager directoriesManager)
        {
            var instance = new ForgingSteelPersistence(directoriesManager);
            try
            {
                var fileInfo = instance.GetConfigFileInfo();
                if (!fileInfo.Exists)
                    return instance;

                using (var reader = new StreamReader(fileInfo.FullName, Encoding.UTF8))
                {
                    var json = reader.ReadToEnd();
                    var loaded = JsonConvert.DeserializeObject<ForgingSteelPersistence>(json);
                    if (loaded != null)
                        instance.ClearedAt = loaded.ClearedAt;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Failed to load Forging Steel persistence.");
            }
            return instance;
        }
    }
}