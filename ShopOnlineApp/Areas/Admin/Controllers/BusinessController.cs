using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.Business;
using ShopOnlineApp.Data.EF;
using ShopOnlineApp.Data.Entities;
using ShopOnlineApp.Helper;

namespace ShopOnlineApp.Areas.Admin.Controllers
{
    public class BusinessController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly IBusinessService _businessService;

        public BusinessController(AppDbContext context, IBusinessService businessService)
        {
            _context = context;
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<IActionResult>  Index()
        {
            var businesss = await _businessService.GetAll();

            var businessName = businesss.Select(x => x.Id).ToList();

            List<string> data = new List<string>()
            {
                "HomeController",
                "BaseController",
                "LoginController",
                "LogoutController"
            };

            var businnesDiff = businessName.Except(data);
            List<BusinessViewModel> businessVM = new List<BusinessViewModel>();

            foreach (var item in businnesDiff)
            {
                businessVM.Add(businesss.SingleOrDefault(x=>x.Id==item));
            }

            return View(businessVM);
        }

        [HttpGet]
        public IActionResult GetById(string id)
        {
            var item = _businessService.GetByIdAsync(id);
            return new OkObjectResult(item);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            _businessService.Delete(id);
            return new OkObjectResult(true);
        }


        public ActionResult UpdateBussiness()
        {
            ReflectionController rc = new ReflectionController();
            List<Type> lstType = rc.GetControllers("ShopOnlineApp.Areas.Admin");

            List<string> lstControllerOld = _context.Businesss.Select(x => x.Id).ToList();

            foreach (var item in lstType)
            {
                if (!lstControllerOld.Contains(item.Name))
                {
                    _context.Businesss.Add(new Business()
                    {
                        Id = item.Name,
                        Name = "Chưa có mô tả"
                    });
                }
                List<string> lisPermission = rc.GetActions(item);
                foreach (var it in lisPermission)
                {
                    if (!lisPermission.Contains(item.Name + "-" + it))
                    {
                        _context.BusinessActions.Add(new BusinessAction()
                        {
                            Name = item.Name + "-" + it,
                            Description = "Chưa có mô tả",
                            BusinessId = item.Name
                        });
                    }
                }
            }

            _context.SaveChanges();

            return Redirect("/Admin/Business/Index");

        }

        [HttpPost]
        public async Task<ActionResult> GetAllPaging(BusinessRequest request)
        {
            var items = await _businessService.GetAllPagingAsync(request);


            return new OkObjectResult(items);
        }

        [HttpPost]
        public IActionResult SaveEntity(BusinessViewModel businessVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }

            if (businessVm.Id == null)
            {
                //await _businessService.AddAsync(businessVm);
            }
            else
            {
                 _businessService.Update(businessVm);
            }
            return new OkObjectResult(businessVm);
        }

    }
}