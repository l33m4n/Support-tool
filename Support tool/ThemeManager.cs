using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System.Windows.Media;

namespace Support_tool
{
    public class ThemeManager
    {
        private readonly PaletteHelper _paletteHelper;

        public ThemeManager()
        {
            _paletteHelper = new PaletteHelper();
        }

        public void SetTheme(string themeName, bool isDarkMode)
        {
            var theme = _paletteHelper.GetTheme();

            switch (themeName)
            {
                case "Green":
                    theme.SetPrimaryColor(Colors.Green);
                    theme.SetSecondaryColor(Colors.LightGreen);
                    break;
                case "Gold":
                    theme.SetPrimaryColor(Colors.DarkGoldenrod);
                    theme.SetSecondaryColor(Colors.Goldenrod);
                    break;
                case "Blue":
                    theme.SetPrimaryColor(Colors.CadetBlue);
                    theme.SetSecondaryColor(Colors.BlueViolet);
                    break;
                case "Purple":
                    Color purpleColor = SwatchHelper.Lookup[(MaterialDesignColor)MaterialDesignColor.DeepPurple];
                    theme.SetPrimaryColor(purpleColor);
                    theme.SetSecondaryColor(Colors.BlueViolet);
                    break;
                default:
                    return; // Do nothing if an invalid theme is selected
            }

            // Set the base theme based on dark mode
            if (isDarkMode)
            {
                theme.SetDarkTheme();
            }
            else
            {
                theme.SetLightTheme();
            }

            // Apply the updated theme
            _paletteHelper.SetTheme(theme);
        }
    }
}
