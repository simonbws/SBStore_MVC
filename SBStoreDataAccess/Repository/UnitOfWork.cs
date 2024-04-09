using SBStore.DataAccess.Data;
using SBStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private AppDbContext _data;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IAppUserRepository AppUser { get; private set; }
        public UnitOfWork(AppDbContext data) 
        {
            _data = data;
            AppUser = new AppUserRepository(_data);
            ShoppingCart = new ShoppingCartRepository(_data);
            Category = new CategoryRepository(_data);
            Product = new ProductRepository(_data);
            Company = new CompanyRepository(_data);
        }
       
        public void Save()
        {
            _data.SaveChanges();
        }
    }
}
