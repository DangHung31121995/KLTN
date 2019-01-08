using System;
using System.Collections.Generic;
using System.Text;
using ShopOnlineApp.Application.ViewModels.BusinessAction;

namespace ShopOnlineApp.Application.Interfaces
{
    public interface IBusinessActionService
    {
       List<BusinessActionViewModel>  GetByBusinessIds(string businessId);
    }
}
