using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.Product;
using ShopOnlineApp.Data.Enums;
using ShopOnlineApp.Data.IRepositories;
using ShopOnlineApp.Infrastructure.Interfaces;

namespace ShopOnlineApp.Application.Implementation
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductCategoryService(IProductCategoryRepository productCategoryRepository,
            IUnitOfWork unitOfWork)
        {
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
        }

        public ProductCategoryViewModel Add(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = new ProductCategoryViewModel().Map(productCategoryVm);
            _productCategoryRepository.Add(productCategory);

            return productCategoryVm;
        }

        public void Delete(int id)
        {
            _productCategoryRepository.Remove(id);
        }

        public async Task<List<ProductCategoryViewModel>> GetAll()
        {
            try
            {
                var dataReturn = await _productCategoryRepository.FindAll().OrderBy(x => x.ParentId).AsNoTracking().ToListAsync();
                var items = new ProductCategoryViewModel().Map(dataReturn).ToList();
                return items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public async Task<List<ProductCategoryViewModel>> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _productCategoryRepository.FindAll(x => x.Name.Contains(keyword)
                || x.Description.Contains(keyword))
                    .OrderBy(x => x.ParentId).AsNoTracking().ProjectTo<ProductCategoryViewModel>().ToList();

            return _productCategoryRepository.FindAll().AsNoTracking().OrderBy(x => x.ParentId)
                .ProjectTo<ProductCategoryViewModel>()
                .ToList();
        }

        public async Task<List<ProductCategoryViewModel>> GetAllByParentId(int parentId)
        {
            var dataReturn = await _productCategoryRepository.FindAll(x => x.Status == Status.Active
              && x.ParentId == parentId).ToListAsync();

            return new ProductCategoryViewModel().Map(dataReturn).ToList();
        }

        public ProductCategoryViewModel GetById(int id)
        {
            return new ProductCategoryViewModel().Map(_productCategoryRepository.FindById(id));

        }

        public async Task<List<ProductCategoryViewModel>> GetHomeCategories(int top)
        {
            var query = _productCategoryRepository
                .FindAll(x => x.HomeFlag == true, c => c.Products)
                  .OrderBy(x => x.HomeOrder)
                  .Take(top).ProjectTo<ProductCategoryViewModel>();

            var categories = await query.ToListAsync();
            foreach (var category in categories)
            {
                category.Products = _productCategoryRepository.FindAll(x => x.HomeFlag.Value && x.ParentId == category.Id)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(5)
                    .ProjectTo<ProductViewModel>().ToList();
            }
            return categories;
        }

        public void ReOrder(int sourceId, int targetId)
        {
            var source = _productCategoryRepository.FindById(sourceId);
            var target = _productCategoryRepository.FindById(targetId);

            var temp = 0;
            if (source != null && target != null)
            {
                temp = source.SortOrder;
                source.SortOrder = target.SortOrder;
                target.SortOrder = temp;
            }
            _productCategoryRepository.Update(source);
            _productCategoryRepository.Update(target);
            Save();
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        //public List<ProductCategoryViewModel> Unflatern()
        //{
        //    var listCategory = new ProductCategoryViewModel().Map(_productCategoryRepository.FindAll());
        //    List<ProductCategoryViewModel> lstProductCategoryViewModels= new List<ProductCategoryViewModel>();

        //    var productCategoryViewModels = listCategory as ProductCategoryViewModel[] ?? listCategory.ToArray();
        //    foreach (var item in productCategoryViewModels)
        //    {
        //        if (item.ParentId == null)
        //        {
        //            item.Children = productCategoryViewModels.Where(x => x.ParentId == item.Id).ToList();

        //        }
        //        else
        //        {

        //        }

        //    }

        //    return lstProductCategoryViewModels;

        //}

        public void Update(ProductCategoryViewModel productCategoryVm)
        {
            var productCategory = new ProductCategoryViewModel().Map(productCategoryVm);

            _productCategoryRepository.Update(productCategory);
           
        }

        public void UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {
            var sourceCategory = _productCategoryRepository.FindById(sourceId);

            sourceCategory.ParentId = targetId;

            _productCategoryRepository.Update(sourceCategory);

            var sibling = _productCategoryRepository.FindAll(x => items.ContainsKey(x.Id));
            foreach (var child in sibling)
            {
                child.SortOrder = items[child.Id];

                _productCategoryRepository.Update(child);
            }
         
        }
    }
}
