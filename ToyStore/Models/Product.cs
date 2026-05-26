namespace ToyStore.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal  Price { get; set; }
        public string Description { get; set; } 
        public int Stock_quantity { get; set; }
        public int Category_id { get; set; }
        public int Brand_id { get; set; }
       
        public string Image_url { get; set; }
        public bool Is_active { get; set; }
        public DateTime Created_at { get; set; }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }


        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
