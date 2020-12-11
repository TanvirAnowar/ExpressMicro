using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static void SeedData(OrderContext orderContext, ILoggerFactory loggerFactory, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            try
            {
                orderContext.Database.Migrate();

                if(!orderContext.Orders.Any())
                {
                    orderContext.Orders.AddRange(GetPreConfiguredOrders());
                    orderContext.SaveChanges();
                }
            }catch(Exception ex)
            {
                if(retryForAvailability <  3)
                {
                    retryForAvailability++;

                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(ex.Message);

                    SeedData(orderContext, loggerFactory, retryForAvailability);
                }
            }

        }

        private static IEnumerable<Order> GetPreConfiguredOrders()
        {
            return new List<Order>()
            {
                new Order() { UserName = "string", FirstName = "Mehmet", LastName = "Ozkaya", EmailAddress = "meh@ozk.com", AddressLine = "Bahcelievler", TotalPrice = 5239 },
                new Order() { UserName = "string", FirstName = "Selim", LastName = "Arslan", EmailAddress ="sel@ars.com", AddressLine = "Ferah", TotalPrice = 3486 }
            };
        }
    }
}
