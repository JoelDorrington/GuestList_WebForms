using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication1.Services;
using System.Web;
using System.Web.SessionState;
using Microsoft.ApplicationInsights.DataContracts;
using System.IO;
using System.Reflection;

namespace WebApplication1.Tests.Services
{

    [TestClass]
    public class SessionService_Tests
    {
        SessionService _sessionService;
        Mock<HttpSessionStateBase> _sessionMock;

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://example.com/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }

        [TestInitialize]
        public void SetUp()
        {
            _sessionMock = new Mock<HttpSessionStateBase>();
            HttpContext.Current = FakeHttpContext();
            _sessionService = new SessionService();
        }

        [TestMethod]
        public void Get_ReturnsValue()
        {
            // Arrange
            HttpContext.Current.Session["key"] = "value";
            // Act
            var result = _sessionService.Get("key");
            // Assert
            Assert.AreEqual("value", result);
        }
        [TestMethod]
        public void Get_ReturnsNull()
        {
            HttpContext.Current.Session["key"] = null;
            var result = _sessionService.Get("key");
            Assert.IsNull(result);
        }
        [TestMethod]
        public void Set_WritesValue()
        {
            _sessionService.Set("key", "value");
            Assert.AreEqual("value", HttpContext.Current.Session["key"]);
        }
    }
}
