namespace whateverthefuck.src.network
{
    using whateverthefuck.src.network.messages;

    public class UserLogin
    {
        public static void Login(string username)
        {
            LoginCredentials loginCreds = new LoginCredentials(username);
            Program.ServerConnection.SendMessage(new LoginMessage(loginCreds));
        }
    }
}
