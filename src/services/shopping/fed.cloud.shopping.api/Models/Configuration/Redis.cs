namespace fed.cloud.shopping.api.Models.Configuration;

public class Redis
{
    public Redis()
    {
        User = new User
        {
            Username = "user",
            Password = "password"
        };
        Database = new Database
        {
            Host = "localhost",
            Name = "default",
            Port = 6379
        };
    }

    public User User { get; set; }

    public Database Database { get; set; }
}

public class User
{
    public string Username { get; set; }

    public string Password { get; set; }
}

public class Database
{
    public string Host { get; set; }

    public int Port { get; set; }

    public string Name { get; set; }
}