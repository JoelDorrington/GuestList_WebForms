using System.Web.SessionState;

namespace WebApplication1.Services
{
    public interface ISessionService
    {
        string Get(string key);
        void Set(string key, string value);
    }
    public interface ISignInService
    {
        string GetLastLog();
        void SetLastLog(string name);
    }
    public class SignInService
    {
        private readonly ISessionService _sessionService;

        public SignInService(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public string GetLastLog()
        {
            return _sessionService.Get("lastLog");
        }

        public void SetLastLog(string name)
        {
            _sessionService.Set("lastLog", name);
        }
    }
}
