using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            dataGridView1.RowHeadersWidth = 50;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToOrderColumns = false;

            for (int i = 0; i < 14; i++)
            {
                dataGridView1.Columns.Add("Column_"+i+1,(i+1).ToString());
            }

            for (int i = 0; i < 7; i++)
            {
                dataGridView1.Rows.Add();

                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }



            foreach (DataGridViewRow row in dataGridView1.Rows)
            {

                

                row.Height = 35;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settingForm set_frm = new settingForm();

            set_frm.ShowDialog();
        }
    }
}
