﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.Product;
using ShopOnlineApp.Data.EF.Common;
using ShopOnlineApp.Data.Entities;
using ShopOnlineApp.Data.IRepositories;
using ShopOnlineApp.Infrastructure.Interfaces;
using ShopOnlineApp.Utilities.Constants;
using ShopOnlineApp.Utilities.Enum;
using ShopOnlineApp.Utilities.Helpers;
using Status = ShopOnlineApp.Data.Enums.Status;

namespace ShopOnlineApp.Application.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductTagRepository _productTagRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productionRepository, IProductTagRepository productTagRepository, ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productionRepository;
            _productTagRepository = productTagRepository;
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<ProductViewModel>> GetAll()
        {
            try
            {
                var response = await _productRepository.FindAll().ToListAsync();

                return response.Any() ? new ProductViewModel().Map(response).ToList() : new List<ProductViewModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<BaseReponse<ModelListResult<ProductViewModel>>> GetAllPaging(ProductRequest request)
        {
            var response = _productRepository.FindAll(x => x.Status == Status.Active).AsNoTracking();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                response = response.Where(x => x.Name.Contains(request.SearchText));
            }
            else if (!string.IsNullOrEmpty(request.Name))
            {
                response = response.Where(x => x.Name.Contains(request.Name));
            }
            else if (request?.CategoryId > 0)
            {
                response = response.Where(x => x.CategoryId == request.CategoryId);
            }

            var totalCount = await response.CountAsync();

            if (request.IsPaging)
            {
                response = response.OrderByDescending(x => x.DateCreated)

                    .Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
            }

            var items = new ProductViewModel().Map(response).ToList();

            return new BaseReponse<ModelListResult<ProductViewModel>>()
            {
                Data = new ModelListResult<ProductViewModel>()
                {
                    Items = items,
                    Message = Message.Success,
                    RowCount = totalCount,
                    PageSize = request.PageSize,
                    PageIndex = request.PageIndex
                },
                Message = Message.Success,
                Status = (int)QueryStatus.Success
            };
        }

        public async Task<ProductViewModel> Add(ProductViewModel productVm)
        {
            List<ProductTag> productTags = new List<ProductTag>();
            if (!string.IsNullOrEmpty(productVm.Tags))
            {
                string[] tags = productVm.Tags.Split(',');
                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);
                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag
                        {
                            Id = tagId,
                            Name = t,
                            Type = CommonConstants.ProductTag
                        };
                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId,
                    };
                    productTags.Add(productTag);
                }

                var product = new ProductViewModel().Map(productVm);

                foreach (var productTag in productTags)
                {
                    product.ProductTags.Add(productTag);
                }
                _productRepository.Add(product);

            }
            return productVm;
        }

        public ProductViewModel GetById(int id)
        {
            return new ProductViewModel().Map(_productRepository.FindById(id));
        }

        public void Update(ProductViewModel productVm)
        {
            List<ProductTag> productTags = new List<ProductTag>();

            if (!string.IsNullOrEmpty(productVm.Tags))
            {
                string[] tags = productVm.Tags.Split(',');
                foreach (string t in tags)
                {
                    var tagId = TextHelper.ToUnsignString(t);
                    if (!_tagRepository.FindAll(x => x.Id == tagId).Any())
                    {
                        Tag tag = new Tag {Id = tagId, Name = t, Type = CommonConstants.ProductTag};
                        _tagRepository.Add(tag);
                    }
                    _productTagRepository.RemoveMultiple(_productTagRepository.FindAll(x => x.Id == productVm.Id).ToList());
                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };
                    productTags.Add(productTag);
                }
            }

            var product =  new ProductViewModel().Map(productVm);

            foreach (var productTag in productTags)
            {
                product.ProductTags.Add(productTag);
            }
            _productRepository.Update(product);
        }

        public void Delete(int id)
        {
            _productRepository.Remove(id);
        }
        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
