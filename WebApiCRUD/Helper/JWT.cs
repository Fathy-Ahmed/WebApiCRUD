namespace WebApiCRUD.Helper;

public class JWT
{
    public string SecritKey { get; set; }
    public string AudienceIP { get; set; }
    public string IssuerIP { get; set; }
    public double DurationInDays { get; set; }
}
