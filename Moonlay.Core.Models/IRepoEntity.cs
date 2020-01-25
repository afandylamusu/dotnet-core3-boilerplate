using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Moonlay.Core.Models
{
    public interface IRepoEntity<TModel, TModelTrail>
        where TModel : Entity
        where TModelTrail : EntityTrail
    {
        DbSet<TModel> DbSet { get; }

        DbSet<TModelTrail> DbSetTrail { get; }

        Task<TModel> With(Guid id);

        string CurrentUser { get; }

        bool IsCurrentUserDemo { get; }
    }
}
