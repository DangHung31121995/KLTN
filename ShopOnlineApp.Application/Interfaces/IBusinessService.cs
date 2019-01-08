using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ShopOnlineApp.Application.ViewModels.Business;
using ShopOnlineApp.Application.ViewModels.Function;
using ShopOnlineApp.Application.ViewModels.User;
using ShopOnlineApp.Data.EF.Common;

namespace ShopOnlineApp.Application.Interfaces
{
    public interface IBusinessService
    {
        Task<List<BusinessViewModel>> GetAll();
        Task<BaseReponse<ModelListResult<BusinessViewModel>>> GetAllPagingAsync(BusinessRequest request);
        BusinessViewModel GetByIdAsync(string id);

        void Update(BusinessViewModel businessVm);
        void Delete(string id);

    }
}
