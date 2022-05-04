using System;
using System.Windows.Forms;
using BasketballNetworking;
using BasketballServices;

namespace BasketballClient
{
    static class StartClient
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            IBasketballService server = new BasketballServerObjectProxy("127.0.0.1", 55556);
            BasketballClientCtrl ctrl = new BasketballClientCtrl(server);

            Application.Run(new LoginForm(ctrl));
        }
    }
}
