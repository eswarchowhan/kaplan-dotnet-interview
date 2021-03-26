using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using WebApiTest.Data;
using WebApiTest.Models;

namespace WebApiTest.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        public OrderItemsService(TestDbContext dbContext)
        {
            testDbContext = dbContext;
        }

        public OrderItemsModel Get(int orderID)
        {
            IEnumerable<OrderItem> orderItems = testDbContext.OrderItems.Where(oi => oi.OrderID == orderID);

            if (orderItems.ToList().Count() == 0)
            {
                return null;  //("order id is not available");
            }

            return new OrderItemsModel
            {
                OrderID = orderID,
                Items = orderItems.Select(oi => new OrderItemModel
                {
                    LineNumber = oi.LineNumber,
                    ProductID = oi.ProductID,
                    StudentPersonID = oi.StudentPersonID,
                    Price = oi.Price
                })
            };
        }

        public bool Delete(int orderID)
        {
            IEnumerable<OrderItem> orderItems = testDbContext.OrderItems.Where(oi => oi.OrderID == orderID);

            //IEnumerable<OrderItem> orderItemsList = testDbContext.OrderItems.RemoveRange(orderItems);
            if (orderItems.ToList().Count() == 0)
            {
                return false;
            }

            foreach (var item in orderItems)
            {

                testDbContext.OrderItems.Remove(item);

            }

            testDbContext.SaveChangesAsync();

            //if (orderItems == null) { 

            //}
            //    orderItemsList.de
            //  orderItems.
            //   orderItems.Remove(orderItems);
            return true;
        }


        public async Task<short> AddAsync(int orderID, OrderItemModel item)

        {
            if (item.LineNumber != 0)
            {
                throw new ValidationException("LineNumber is generated and cannot be specified");
            }

            var list = testDbContext.OrderItems.Where(oi => oi.OrderID == orderID).ToList();
            var lineNumber = (short)(testDbContext.OrderItems.Max(oi => oi.LineNumber) + 1);

            await testDbContext.OrderItems.AddAsync(new OrderItem
            {
                OrderID = orderID,
                LineNumber = lineNumber,
                Price = item.Price,
                ProductID = item.ProductID,
                StudentPersonID = item.StudentPersonID
            });

            await testDbContext.SaveChangesAsync();

            return lineNumber;
        }

        private readonly TestDbContext testDbContext;
    }
}
