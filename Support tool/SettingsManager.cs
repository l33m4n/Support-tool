using System;
using System.IO;

namespace Support_tool
{
    public class SettingsManager
    {
        private readonly string settingsFilePath;

        public bool IsDarkMode { get; set; }
        public bool IsAlwaysOnTop { get; set; }
        public string SelectedTheme { get; set; }

        public SettingsManager()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appFolder = Path.Combine(appDataFolder, "CopyPastaSettings");
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            settingsFilePath = Path.Combine(appFolder, "CopypastaSettings.txt");

            LoadSettings();
        }

        public void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string[] settingsLines = File.ReadAllLines(settingsFilePath);

                foreach (var line in settingsLines)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        switch (parts[0].Trim())
                        {
                            case "IsDarkMode":
                                IsDarkMode = parts[1].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
                                break;
                            case "IsAlwaysOnTop":
                                IsAlwaysOnTop = parts[1].Trim().Equals("True", StringComparison.OrdinalIgnoreCase);
                                break;
                            case "SelectedTheme":
                                SelectedTheme = parts[1].Trim();
                                break;
                        }
                    }
                }
            }
            else
            {
                // Default settings
                IsDarkMode = false;
                IsAlwaysOnTop = false;
                SelectedTheme = "Purple";
            }
        }



        public void SaveSettings()
        {
            string[] settings = new string[]
            {
                IsDarkMode.ToString(),
                IsAlwaysOnTop.ToString(),
                SelectedTheme
            };
            File.WriteAllLines(settingsFilePath, settings);
        }
    }
}
