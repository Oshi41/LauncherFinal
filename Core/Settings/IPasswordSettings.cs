namespace Core.Settings
{
    public interface IPasswordSettings
    {
        string Salt { get; set; }
        string Password { get; set; }
        string Encrypted { get; set; }
    }
}
