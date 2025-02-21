namespace waap.ServiceModels;

public class EmailSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FriendlyName { get; set; }
    public bool SendNotificationEmail { get; set; }

    
}
