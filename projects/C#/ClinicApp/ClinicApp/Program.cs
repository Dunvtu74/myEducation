using ClinicApp.Database;

namespace ClinicApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            DB.Init();
            Application.Run(new Form1());
        }
    }
}