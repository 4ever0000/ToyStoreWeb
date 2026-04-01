using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ToyStore.Frontend.Services
{
    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        public event Action OnThemeChanged;

        private string currentTheme = "light";

        // Constructor-da JS Runtime-ı qəbul edirik
        public ThemeService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string CurrentTheme => currentTheme;
        public bool IsDarkMode => currentTheme == "dark";

        // Toggle metodu artıq async olmalıdır (JS çağırdığı üçün)
        public async Task ToggleTheme()
        {
            var newTheme = currentTheme == "light" ? "dark" : "light";
            await SetTheme(newTheme);
        }

        public async Task SetTheme(string theme)
        {
            if (theme == "light" || theme == "dark")
            {
                currentTheme = theme;

                // 🛑 ƏSAS HİSSƏ: Brauzerdəki JS funksiyasını çağırırıq
                await _jsRuntime.InvokeVoidAsync("setTheme", theme);

                NotifyThemeChanged();
            }
        }

        private void NotifyThemeChanged() => OnThemeChanged?.Invoke();
    }
}