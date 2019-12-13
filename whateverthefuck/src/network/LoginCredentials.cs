namespace whateverthefuck.src.network
{
    public class LoginCredentials
    {
        public LoginCredentials()
        {
        }

        public LoginCredentials(string username)
        {
            this.Username = username;
        }

        public string Username { get; private set; }
    }
}
