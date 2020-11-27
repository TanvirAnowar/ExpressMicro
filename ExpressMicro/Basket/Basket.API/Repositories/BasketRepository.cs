using Basket.API.Data.Interfaces;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IBasketContext _basketContext;

        public BasketRepository(IBasketContext basketContext)
        {
            _basketContext = basketContext;
        }
         
        public async Task<bool> DeletBasket(string userName)
        {
            var basketStatus = await _basketContext.Redis.KeyDeleteAsync(userName);

            return basketStatus;
        }

        public async Task<BasketCart> GetBasket(string userName)
        {
            var basket = await _basketContext.Redis.StringGetAsync(userName);

            if(basket.IsNull)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BasketCart>(basket);
        }

        public async Task<BasketCart> UpdateBasket(BasketCart basketCart)
        {
            var basket = await _basketContext.Redis.StringSetAsync(basketCart.UserName, JsonConvert.SerializeObject(basketCart));
            
            if(!basket)
            {
                return null;
            }

            return await this.GetBasket(basketCart.UserName);
        }
    }
}
