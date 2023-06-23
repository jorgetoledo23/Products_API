namespace Products_API.Model
{
    public class AddProductDTO
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
    }
}
