using ToyStore.Frontend.Models;

namespace ToyStore.Frontend.Services
{
    public class CartService
    {
        // 🔔 EVENT: Səbət dəyişdikdə bütün listener'lar (MainLayout, vs) bildirilsin
        public event Action OnCartChanged;

        // Səbətdəki məhsulların siyahısı
        public List<CartItem> SelectedItems { get; set; } = new List<CartItem>();

        // Səbət dəyişdikdə event'i çağır
        private void NotifyCartChanged()
        {
            OnCartChanged?.Invoke();
        }

        // Səbətə əlavə etmək metodu
        public void AddToCart(ProductDto product)
        {
            var item = SelectedItems.FirstOrDefault(p => p.ProductId == product.Id);
            if (item == null)
            {
                SelectedItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                item.Quantity++;
            }
            NotifyCartChanged();
        }

        // Məhsulu səbətdən tamamilə silmək
        public void RemoveItem(int productId)
        {
            var item = SelectedItems.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                SelectedItems.Remove(item);
                NotifyCartChanged();
            }
        }

        // Səbəti tamamilə təmizləmək
        public void ClearCart()
        {
            SelectedItems.Clear();
            NotifyCartChanged();
        }

        // Məhsulun sayını azaltmaq (əgər 1-dirsə, silmək)
        public void DecreaseQuantity(int productId)
        {
            var item = SelectedItems.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    SelectedItems.Remove(item);
                NotifyCartChanged();
            }
        }

        // Səbətin ümumi məbləğini hesablamaq
        public decimal GetTotal() => SelectedItems.Sum(x => x.Price * x.Quantity);

        // Səbətdəki ümumi məhsul sayını almaq (Badge üçün lazım olacaq)
        public int GetTotalCount() => SelectedItems.Sum(x => x.Quantity);
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}