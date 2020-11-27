using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Entities
{
    public class BasketCart
    {
        public string UserName { get; set; }
        public List<BasketCartItem> Items { get; set; } = new List<BasketCartItem>();

        //Calculate total price of the cart items
        public decimal TotalPrice {
            get
            {
                decimal totalPrice = 0;

                foreach(var item in Items)
                {
                    totalPrice += item.Qunatity * item.Price;

                }
                return totalPrice;
            }
                }
        public BasketCart()
        {
        }

        public BasketCart(string userName)
        {
            UserName = userName;
        }

        

    }
}
