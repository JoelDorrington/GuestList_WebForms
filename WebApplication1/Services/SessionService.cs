using System.Web;
using WebApplication1.Services;

public class SessionService : ISessionService
{
    public string Get(string key)
    {
        return HttpContext.Current.Session[key] as string;
    }

    public void Set(string key, string value)
    {
        HttpContext.Current.Session[key] = value;
    }
}
