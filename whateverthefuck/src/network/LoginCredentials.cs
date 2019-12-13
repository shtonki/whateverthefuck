namespace whateverthefuck.src.network
{
    public class LoginCredentials
    {
        public string Username { get; private set; }

        public LoginCredentials()
        {
        }

        public LoginCredentials(string username)
        {
            this.Username = username;
        }
    }
}
