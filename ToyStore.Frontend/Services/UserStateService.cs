using System.Net.Http.Json;
using ToyStore.Frontend.Models;

namespace ToyStore.Frontend.Services
{
    public class UserStateService
    {
        private readonly HttpClient _http;
        public UserStateService(HttpClient http) => _http = http;

        public int UserId { get; set; } = 1;
        public string UserName { get; set; } = "Yüklənir...";
        public string UserEmail { get; set; } = "";
        public string UserPhone { get; set; } = "";
        public int ToyPoints { get; set; } = 150;

        public event Action OnChange;

        public async Task LoadUserProfileAsync()
        {
            try
            {
                var user = await _http.GetFromJsonAsync<UserDto>($"users/{UserId}");
                if (user != null)
                {
                    UserName = user.Name;
                    UserEmail = user.Email;
                    UserPhone = user.Phone;
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xəta: {ex.Message}");
            }
        }

        public void UpdateState(string newName, string newPhone)
        {
            UserName = newName;
            UserPhone = newPhone;
            NotifyStateChanged();
        }

        // ✅ BAX BU METODU ƏLAVƏ ETDİK:
        public void AddPoints(int amount)
        {
            ToyPoints += amount;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}