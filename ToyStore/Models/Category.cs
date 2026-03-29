namespace ToyStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Parent_id { get; set; }
       
    }
}
