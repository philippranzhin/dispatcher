namespace DispatcherDesktop.Domain.Web
{
    using System;

    public class ConnectInfo
    {
        public ConnectInfo(Uri serverUrl, string login, string password)
        {
            this.ServerUrl = serverUrl;
            this.Login = login;
            this.Password = password;
        }

        public Uri ServerUrl { get; }
        
        public string Login { get; } 
        
        public string Password { get; }
    }
}