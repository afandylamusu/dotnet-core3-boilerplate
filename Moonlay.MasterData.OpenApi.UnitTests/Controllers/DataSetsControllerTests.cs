using Microsoft.Extensions.Logging;
using Moonlay.MasterData.OpenApi.Clients;
using Moonlay.MasterData.OpenApi.Controllers;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Moonlay.MasterData.OpenApi.UnitTests.Controllers
{
    public class DataSetsControllerTests
    {
        private MockRepository mockRepository;

        private Mock<ILogger<DataSetsController>> mockLogger;
        private Mock<IManageDataSetClient> mockManageDataSetClient;

        public DataSetsControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockLogger = this.mockRepository.Create<ILogger<DataSetsController>>();
            this.mockManageDataSetClient = this.mockRepository.Create<IManageDataSetClient>();
        }

        private DataSetsController CreateDataSetsController()
        {
            return new DataSetsController(
                this.mockLogger.Object,
                this.mockManageDataSetClient.Object);
        }

        [Fact]
        public async Task Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dataSetsController = this.CreateDataSetsController();

            // Act
            var result = await dataSetsController.Get();

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }
    }
}
