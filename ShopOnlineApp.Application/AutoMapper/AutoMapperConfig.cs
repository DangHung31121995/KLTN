using System;
using System.Runtime.CompilerServices;
using AutoMapper;
using ShopOnlineApp.Application.ViewModels.Bill;
using ShopOnlineApp.Application.ViewModels.Business;
using ShopOnlineApp.Application.ViewModels.BusinessAction;
using ShopOnlineApp.Application.ViewModels.Function;
using ShopOnlineApp.Application.ViewModels.Product;
using ShopOnlineApp.Application.ViewModels.Role;
using ShopOnlineApp.Application.ViewModels.Size;
using ShopOnlineApp.Application.ViewModels.User;
using ShopOnlineApp.Data.Entities;

namespace ShopOnlineApp.Application.AutoMapper
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration Config = new MapperConfiguration(cfg =>
        {
            try
            {
                #region Product
                cfg.CreateMap<Product, ProductViewModel>();
                cfg.CreateMap<ProductViewModel, Product>()
                    .ConstructUsing(c => new Product(c.Name, c.CategoryId, c.Image, c.Price, c.OriginalPrice,
                        c.PromotionPrice, c.Description, c.Content, c.HomeFlag, c.HotFlag, c.Tags, c.Unit, c.Status,
                        c.SeoPageTitle, c.SeoAlias, c.SeoKeywords, c.SeoDescription));
                #endregion

                #region Function
                cfg.CreateMap<Function, FunctionViewModel>();
                cfg.CreateMap<FunctionViewModel, Function>();

                #endregion

                #region ProductCategory
                cfg.CreateMap<ProductCategory, ProductCategoryViewModel>().ForMember(x=>x.Children,opt=>opt.Ignore());
                cfg.CreateMap<ProductCategoryViewModel, ProductCategory>(); ;

                #endregion

                #region user
                cfg.CreateMap<AppUser, AppUserViewModel>();
                cfg.CreateMap<AppUserViewModel, AppUser>();

                #endregion
                #region approle
                cfg.CreateMap<AppRole, AppRoleViewModel>();
                cfg.CreateMap<AppRoleViewModel, AppRole>();
                #endregion approle

                #region Permission
                cfg.CreateMap<Permission, PermissionViewModel>();
                cfg.CreateMap<PermissionViewModel, Permission>();
                #endregion

                #region Bussiness
                cfg.CreateMap<Business, BusinessViewModel>();
                cfg.CreateMap<BusinessViewModel, Business>();
                #endregion

                #region BussinessAction
                cfg.CreateMap<BusinessAction, BusinessActionViewModel>();
                cfg.CreateMap<BusinessActionViewModel, BusinessAction>();
                #endregion

                #region BusinessAction



                #endregion

                #region Bill
                cfg.CreateMap<Bill, BillViewModel>().MaxDepth(2);
                cfg.CreateMap<BillViewModel, Bill>().MaxDepth(2);
                #endregion

                #region Size
                cfg.CreateMap<Size, SizeViewModel>();
                cfg.CreateMap<SizeViewModel, Size>();
                #endregion

                #region Size
                cfg.CreateMap<Size, SizeViewModel>();
                cfg.CreateMap<SizeViewModel, Size>();
                #endregion


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        });
    }

}
