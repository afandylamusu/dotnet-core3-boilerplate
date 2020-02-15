using Moonlay.Core.Models;

namespace Moonlay.MasterData.Domain.UnitTests
{
    public class MyConnection : IDbxConnection
    {
        public MyConnection(System.Data.Common.DbConnection connection)
        {
            Connection = connection;
        }

        public System.Data.Common.DbConnection Connection { get; }
    }
}
