using Moonlay.MasterData.Protos;

namespace Moonlay.MasterData.OpenApi.Controllers
{
    public class DataSetDto
    {
        public DataSetDto(DataSetArg o)
        {
            Name = o.Name;
        }

        public string Name { get; }
    }
}