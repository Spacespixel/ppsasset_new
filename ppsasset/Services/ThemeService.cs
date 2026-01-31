using PPSAsset.Models;

namespace PPSAsset.Services
{
    public interface IThemeService
    {
        ProjectTheme GetProjectTheme(string projectId);
        ProjectTheme GetDefaultTheme();
    }

    /// <summary>
    /// Static theme configuration service - themes don't change frequently
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly Dictionary<string, ProjectTheme> _themes;

        public ThemeService()
        {
            _themes = InitializeThemes();
        }

        public ProjectTheme GetProjectTheme(string projectId)
        {
            return _themes.TryGetValue(projectId, out var theme) ? theme : GetDefaultTheme();
        }

        public ProjectTheme GetDefaultTheme()
        {
            return new ProjectTheme
            {
                ThemeName = "default",
                PrimaryColor = "#365523",
                SecondaryColor = "#4A7030",
                LightBackground = "#F8FAF7",
                CssClass = "theme-default"
            };
        }

        private Dictionary<string, ProjectTheme> InitializeThemes()
        {
            return new Dictionary<string, ProjectTheme>
            {
                ["ricco-residence-hathairat"] = new ProjectTheme
                {
                    ThemeName = "magenta",
                    PrimaryColor = "#AF017F",
                    SecondaryColor = "#D91E6F",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-magenta"
                },
                ["ricco-residence-chatuchot"] = new ProjectTheme
                {
                    ThemeName = "red",
                    PrimaryColor = "#B71C1C",
                    SecondaryColor = "#D32F2F",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-red"
                },
                ["ricco-residence-prime-hathairat"] = new ProjectTheme
                {
                    ThemeName = "dark-red",
                    PrimaryColor = "#580709",
                    SecondaryColor = "#b9834c",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-dark-red"
                },
                ["ricco-residence-prime-chatuchot"] = new ProjectTheme
                {
                    ThemeName = "blue",
                    PrimaryColor = "#1976D2",
                    SecondaryColor = "#2196F3",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-blue"
                },
                ["ricco-town-phahonyothin-saimai53"] = new ProjectTheme
                {
                    ThemeName = "maroon",
                    PrimaryColor = "#e5218a",
                    SecondaryColor = "#f857b8",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-maroon"
                },
                ["ricco-town-wongwaen-lamlukka"] = new ProjectTheme
                {
                    ThemeName = "pink",
                    PrimaryColor = "#e5218a",
                    SecondaryColor = "#f857b8",
                    LightBackground = "#f8f9fa",
                    CssClass = "theme-pink"
                }
            };
        }
    }
}