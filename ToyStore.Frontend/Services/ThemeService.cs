using System;

namespace ToyStore.Frontend.Services
{
    public class ThemeService
    {
        public event Action OnThemeChanged;
        
        private string currentTheme = "light";
        private const string ThemeStorageKey = "app-theme";

        public string CurrentTheme
        {
            get => currentTheme;
            set
            {
                currentTheme = value;
                NotifyThemeChanged();
            }
        }

        public void ToggleTheme()
        {
            CurrentTheme = currentTheme == "light" ? "dark" : "light";
        }

        public void SetTheme(string theme)
        {
            if (theme == "light" || theme == "dark")
            {
                CurrentTheme = theme;
            }
        }

        public bool IsDarkMode => currentTheme == "dark";

        private void NotifyThemeChanged()
        {
            OnThemeChanged?.Invoke();
        }
    }
}
