using Moonlay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.Customers
{
    public interface ICustomerService
    {
        Task<Customer> NewCustomerAsync(string firstName, string lastName, Action<Customer> beforeSave = null);

        Task<PaginatedList<Customer>> SearchAsync(Expression<Func<Customer, bool>> criteria = null, int page = 0, int size = 25);

        Task<List<CustomerTrail>> LogsAsync(Guid id);

        Task<Customer> UpdateProfileAsync(Guid id, string firstName, string lastName, Action<Customer> beforeSave = null);
    }

}