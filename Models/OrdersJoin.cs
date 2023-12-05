using Microsoft.EntityFrameworkCore;

namespace Project.Models
{
    public class OrdersJoin
    {
        private readonly ModelContext _context; 

        public OrdersJoin(ModelContext context)
        {
            _context = context;
        }
        public ProductCustomer Order { get; set; }
        public Address Address { get; set; }
        public List<OrderInfo> OrderInfo { get; set; }
        public List<Product> Products { get; set; }

        //public Review GetUserReviewForProduct(decimal productId, int userId)
        //{
        //    var existingReview = _context.Reviews.FirstOrDefault(r => r.CustomerId == userId && r.ProductId == productId);
        //    Console.WriteLine(DateTime.Now);
        //    return existingReview;
        //}

        public Review GetUserReviewForProduct(decimal productId, int userId, decimal orderId)
        {
                var existingReview = _context.Reviews
                    .FirstOrDefault(r => r.CustomerId == userId && r.ProductId == productId && r.OrderId == orderId);

                if (existingReview != null)
                {
                    return existingReview;
                }
            

            return null;
        }



    }
}
