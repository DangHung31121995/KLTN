using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using ShopOnlineApp.Application.Interfaces;
using ShopOnlineApp.Application.ViewModels.BusinessAction;
using ShopOnlineApp.Data.IRepositories;

namespace ShopOnlineApp.Application.Implementation
{
    public class BusinessActionService : IBusinessActionService
    {
        private readonly IBusinessActionRepository _repository;
        public BusinessActionService(IBusinessActionRepository repository)
        {
            _repository = repository;
        }
        public List<BusinessActionViewModel>  GetByBusinessIds(string businessId)
        {
            var items = _repository.FindAll(x => x.BusinessId == businessId).GroupBy(x=>x.Name).Select(x=>x.FirstOrDefault());

            return new BusinessActionViewModel().Map(items).ToList();

        }
    }
}
