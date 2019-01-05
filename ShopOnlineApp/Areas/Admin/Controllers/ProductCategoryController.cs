﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.Product;
using ShopOnlineApp.Infrastructure.Interfaces;
using ShopOnlineApp.Utilities.Helpers;

namespace ShopOnlineApp.Areas.Admin.Controllers
{
    public class ProductCategoryController : BaseController
    {
        #region private property

        private readonly IProductCategoryService _productCategoryService;
        private readonly IUnitOfWork _unitOfWork;


        #endregion

        #region constructor
        public ProductCategoryController(IProductCategoryService productCategoryService, IUnitOfWork unitOfWork)
        {
            _productCategoryService = productCategoryService;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region method

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            var items = _productCategoryService.GetById(id);
            if (items != null)
            {
                return new OkObjectResult(items);
            }

            return new OkResult();

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new BadRequestResult();
            }

            _productCategoryService.Delete(id);
            _unitOfWork.Commit();
            return new OkObjectResult(id);
        }


        [HttpPost]
        public IActionResult SaveEntity(ProductCategoryViewModel productVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            productVm.SeoAlias = TextHelper.ToUnsignString(productVm.Name);
            if (productVm.Id == 0)
            {
                _productCategoryService.Add(productVm);
            }
            else
            {
                _productCategoryService.Update(productVm);
            }
            _unitOfWork.Commit();
            return new OkObjectResult(productVm);
        }
        public IActionResult UpdateParentId(int sourceId, int targetId, Dictionary<int, int> items)
        {

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (sourceId == targetId)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _productCategoryService.UpdateParentId(sourceId, targetId,items);
            _unitOfWork.Commit();
            return new OkResult();
        }

        public IActionResult ReOrder(int sourceId,int targetId)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            if (sourceId == targetId)
            {
                return new BadRequestObjectResult(ModelState);
            }

            _productCategoryService.ReOrder(sourceId,targetId);
            _unitOfWork.Commit();
            return new OkResult();
        }

        [HttpGet]
        public async Task<IActionResult>  GetAll()
        {
            var items = await _productCategoryService.GetAll();
            return new OkObjectResult(items);
        }

        #endregion

    }
}