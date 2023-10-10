using Maletkunst.DAL.RestClient;
using MaletKunst.WinApp;

namespace Maletkunst.WinApp;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var ApiClient = new PaintingsRestClientDao();

		Application.Run(new MainForm(ApiClient));
    }
}