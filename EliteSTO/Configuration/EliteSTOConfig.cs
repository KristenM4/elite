namespace EliteSTO.Configuration;

public class EliteSTOConfig
{
    public const string SectionName = "EliteSTO";
    public EmailSettings? EmailSettings { get; set; }
}

public class EmailSettings
{
    public string? From { get; set; }
    public string? To { get; set; }
    public string? ApiKey { get; set; }
    public string? MailgunEndpoint { get; set; }
}