using Ticketing.Db.Models;

namespace Ticketing.Db.Providers
{
    public class CartProvider
    {
        public List<Cart> Carts { get; set; }

        public CartProvider()
        {
            Carts = new List<Cart>();
            CreateCarts();
        }

        private void CreateCarts()
        {
            for (var i = 0; i < 10; i++)
            {
                Carts.Add(new Cart() { Id = Guid.NewGuid() });
            }
        }
    }
}
