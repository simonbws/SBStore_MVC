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
    }
}
