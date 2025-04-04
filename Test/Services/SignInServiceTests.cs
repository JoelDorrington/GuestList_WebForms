using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Services;

namespace WebApplication1.Tests.Services
{
    [TestClass]
    public class SignInService_Tests
    {
        private SignInService _signInService;
        private Mock<ISessionService> _sessionMock;

        [TestInitialize]
        public void SetUp()
        {
            _sessionMock = new Mock<ISessionService>();
            _signInService = new SignInService(_sessionMock.Object);
        }

        [TestMethod]
        public void GetLastLog_ReturnsLastLog()
        {
            // Arrange
            _sessionMock.Setup(s => s.Get("lastLog")).Returns("TestUser");

            // Act
            var result = _signInService.GetLastLog();

            // Assert
            Assert.AreEqual("TestUser", result);
        }

        [TestMethod]
        public void SetLastLog_SetsLastLog()
        {
            // Act
            _signInService.SetLastLog("TestUser");

            // Assert
            _sessionMock.Verify(s => s.Set("lastLog", "TestUser"), Times.Once);
        }
    }

}