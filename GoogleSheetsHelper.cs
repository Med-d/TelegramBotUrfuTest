using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace TGBot;

public class GoogleSheetsHelper
{
    public SheetsService Service { get; set; } = null!;
    private const string ApplicationName = "UrfuTestTask";
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    public GoogleSheetsHelper()
    {
        InitializeService();
    }
    private void InitializeService()
    {
        var credential = GetCredentialsFromFile();
        Service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        });
    }
    private GoogleCredential GetCredentialsFromFile()
    {
        using var stream = new FileStream("secrets.json", FileMode.Open, FileAccess.Read);
        var credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        return credential;
    }
}