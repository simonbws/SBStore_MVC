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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext _data;
        public CategoryRepository(AppDbContext data) : base(data) 
        {
              _data = data;
        }

        public void Save()
        {
            _data.SaveChanges();
        }

        public void Update(Category obj)
        {
            _data.Categories.Update(obj);
        }
    }
}
