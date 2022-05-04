using System;
using System.Drawing;
using System.Windows.Forms;
using BasketballModel;
using BasketballModel.validator;
using BasketballServices;

namespace BasketballClient
{
    public partial class SellerForm : Form
    {
        private BasketballClientCtrl ctrl;
        
        public SellerForm(BasketballClientCtrl ctrl)
        {
            InitializeComponent();
            this.ctrl = ctrl;
            ctrl.updateEvent += userUpdate;
        }

        private void SellerForm_Load(object sender, EventArgs e)
        {
            dataGridView1Refresh();
            dataGridView2Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ctrl.saveTicket(textBox1.Text, textBox2.Text, textBox3.Text);
            }
            catch (ServiceException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (ValidatorException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            long match_id;

            if (dataGridView1.SelectedCells.Count > 0)
            {
                match_id = Int64.Parse(dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                textBox3.Text = match_id.ToString();
            }
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            long match_id;

            if (dataGridView2.SelectedCells.Count > 0)
            {
                match_id = Int64.Parse(dataGridView2.Rows[dataGridView2.SelectedCells[0].RowIndex].Cells[0].Value.ToString());
                textBox3.Text = match_id.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void userUpdate(object sender, BasketballUserEventArgs e)
        {
            if (e.UserEventType == BasketballUserEvent.TicketSold)
            {
                Console.WriteLine("[ChatWindow] TicketSold ");
                dataGridView1.BeginInvoke(new UpdateDataGridViewCallback(this.dataGridView1Refresh));
                dataGridView2.BeginInvoke(new UpdateDataGridViewCallback(this.dataGridView2Refresh));
            }
        }

        private void dataGridView1Refresh()
        {
            dataGridView1.Rows.Clear();
            int i = 0;
            foreach (Match m in ctrl.findAll())
            {
                if (m.NoAvailableSeats > 0)
                {
                    dataGridView1.Rows.Insert(i, m.MatchId.ToString(), m.Name, m.TicketPrice.ToString(), m.NoAvailableSeats.ToString(), "Available");
                    dataGridView1.Rows[i].Cells[4].Style = new DataGridViewCellStyle { ForeColor = Color.Green, BackColor = Color.LightGreen };
                }
                else
                {
                    dataGridView1.Rows.Insert(i, m.MatchId.ToString(), m.Name, m.TicketPrice.ToString(), m.NoAvailableSeats.ToString(), "SOLD OUT");
                    dataGridView1.Rows[i].Cells[4].Style = new DataGridViewCellStyle { ForeColor = Color.Red, BackColor = Color.PaleVioletRed };
                }
                i++;
            }
        }

        private void dataGridView2Refresh()
        {
            int i = 0;
            dataGridView2.Rows.Clear();
            i = 0;
            foreach (Match m in ctrl.availableMatchesDescending())
            {
                dataGridView2.Rows.Insert(i, m.MatchId.ToString(), m.Name, m.TicketPrice.ToString(), m.NoAvailableSeats.ToString());
                i++;
            }
        }
        
        public delegate void UpdateDataGridViewCallback();

        private void SellerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ctrl.logout();
            ctrl.updateEvent -= userUpdate;
        }
    }
}
