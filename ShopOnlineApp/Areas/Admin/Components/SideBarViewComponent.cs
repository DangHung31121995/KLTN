using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.Function;
using ShopOnlineApp.Extensions;
using ShopOnlineApp.Utilities.Enum;

namespace ShopOnlineApp.Areas.Admin.Components
{
    public class SideBarViewComponent:ViewComponent
    {
        public readonly IFunctionService _functionService;
        public SideBarViewComponent(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var roles =  ((ClaimsPrincipal) User).GetSpecificDefault("Role");
            List<FunctionViewModel> functions;

            if (roles.Split(";").Contains(ConstantSystem.AdminRole))
            {
                functions = await _functionService.GetAll(string.Empty);
            }
            else
            {
                functions= new List<FunctionViewModel>();
            }
            return View(functions);
        }
    }
}
