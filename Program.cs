using System;
using System.Threading;
using System.Windows.Forms;

namespace AracKiralamaSistemi
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.Run(new AnaForm());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                e.Exception.Message,
                "Beklenmeyen Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string mesaj = e.ExceptionObject is Exception ex ? ex.Message : "Bilinmeyen hata";
            MessageBox.Show(mesaj, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
