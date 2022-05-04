using System;
using System.Windows.Forms;
using BasketballServices;

namespace BasketballClient
{
    public partial class LoginForm : Form
    {
        private BasketballClientCtrl ctrl;
        public LoginForm(BasketballClientCtrl ctrl)
        {
            InitializeComponent();
            this.ctrl = ctrl;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            
        }
        
        private void buttonLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                ctrl.login(textBox1.Text, textBox2.Text);
                
                SellerForm sellerForm = new SellerForm(ctrl);
                textBox1.Clear();
                textBox2.Clear();
                this.Hide();
                sellerForm.ShowDialog();
                this.Show();
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}