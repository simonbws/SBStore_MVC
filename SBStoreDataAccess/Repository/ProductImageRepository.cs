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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private AppDbContext _data;
        public ProductImageRepository(AppDbContext data) : base(data) 
        {
              _data = data;
        }

        

        public void Update(ProductImage obj)
        {
            _data.ProductImages.Update(obj);
        }
    }
}
