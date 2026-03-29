using ToyStore.Frontend.Models;

namespace ToyStore.Frontend.Services
{
    public class WishlistService
    {
        public event Action OnWishlistChanged;
        private List<ProductDto> wishlistItems = new();

        public List<ProductDto> GetWishlistItems() => wishlistItems;

        public int GetWishlistCount() => wishlistItems.Count;

        public bool IsInWishlist(int productId) => wishlistItems.Any(p => p.Id == productId);

        public void AddToWishlist(ProductDto product)
        {
            if (!IsInWishlist(product.Id))
            {
                wishlistItems.Add(product);
                NotifyWishlistChanged();
            }
        }

        public void RemoveFromWishlist(int productId)
        {
            var item = wishlistItems.FirstOrDefault(p => p.Id == productId);
            if (item != null)
            {
                wishlistItems.Remove(item);
                NotifyWishlistChanged();
            }
        }

        public void ToggleWishlist(ProductDto product)
        {
            if (IsInWishlist(product.Id))
                RemoveFromWishlist(product.Id);
            else
                AddToWishlist(product);
        }

        public void ClearWishlist()
        {
            wishlistItems.Clear();
            NotifyWishlistChanged();
        }

        private void NotifyWishlistChanged()
        {
            OnWishlistChanged?.Invoke();
        }
    }
}
