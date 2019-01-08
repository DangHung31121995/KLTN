using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShopOnlineApp.Application.Interfaces;

namespace ShopOnlineApp.Areas.Admin.Controllers
{
    public class BusinessActionController:BaseController
    {
        private readonly IBusinessActionService _actionService;

        public BusinessActionController(IBusinessActionService actionService)
        {
            _actionService = actionService;
        }
     
        public IActionResult Index(string businessId)
        {
            var items = _actionService.GetByBusinessIds(businessId);
            return View(items);
        }

    }
}
