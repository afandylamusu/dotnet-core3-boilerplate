using System.Data.Common;

namespace Moonlay.Core.Models
{
    public interface IDbxConnection
    {
        DbConnection Connection { get; }
    }
}