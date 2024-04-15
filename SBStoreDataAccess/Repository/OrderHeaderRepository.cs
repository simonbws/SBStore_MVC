using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using SBStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SBStore.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private AppDbContext _data;
        public OrderHeaderRepository(AppDbContext data) : base(data) 
        {
              _data = data;
        }

        

        public void Update(OrderHeader obj)
        {
            _data.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _data.OrderHeaders.FirstOrDefault(u => u.Id == id); //retrieve Order Header
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus; //update orderfromdb
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus; //update
                }
            }

        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _data.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId; //it checks when a user tries to make a payment. when it is successful then a payment intent id gets generated
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId; 
                orderFromDb.PaymentDate = DateTime.Now; 
            }
        }
    }
}
