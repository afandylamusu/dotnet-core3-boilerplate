using Moonlay.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moonlay.MasterData.Domain.Customers
{
    public class CustomerRepository : RepoEntity<Customer, CustomerTrail>, ICustomerRepository
    {
        public CustomerRepository(IDbContext dbContext, IDbTrailContext dbTrailContext, ISignInService signIn) : base(dbContext, dbTrailContext, signIn)
        {
        }
    }

    internal static class RepositoryHelpers
    {
        /// <summary>
        /// Save new Customer
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="id"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static async Task<Customer> StoreAsync(this ICustomerRepository repo, Guid id, string firstName, string lastName)
        {
            var record = repo.Create(id, firstName, lastName);

            await repo.DbSet.AddAsync(record);

            return record;
        }

        public static Customer Create(this ICustomerRepository repo, Guid id, string firstName, string lastName)
        {
            return new Customer(id)
            {
                FirstName = firstName,
                LastName = lastName,
            };
        }

        public static async Task StoreAsync(this ICustomerRepository repo, IEnumerable<Customer> list)
        {
            await repo.DbSet.AddRangeAsync(list);
        }

        public static Task<Customer> UpdateAsync(this ICustomerRepository repo, Customer record)
        {
            repo.DbSet.Update(record);

            return Task.FromResult(record);
        }

        public static async Task RemoveAsync(this ICustomerRepository repo, Guid id)
        {
            var record = await repo.With(id);

            repo.DbSet.Remove(record);
        }
    }
}