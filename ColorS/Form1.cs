using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using System.Diagnostics;

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
                dataGridView1.Columns.Add("Column_" + i + 1, (i + 1).ToString());
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


            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {

                    Color color = CreateRandomColor();

                    dataGridView1[i, j].Style.BackColor = color;
                }

            }
        }

        private Color CreateRandomColor()
        {
            Random randonGen = new Random();
            Color randomColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255), randonGen.Next(255));
            return randomColor;
        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm set_frm = new SettingsForm();

            set_frm.ShowDialog();
        }

        private void save_button_Click(object sender, EventArgs e)
        {

            List<string> lines = new List<string>();

            int n = 0;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    lines.Add(("R="+dataGridView1[i,j].Style.BackColor.R).ToString()+" "+ "G=" + (dataGridView1[i, j].Style.BackColor.G).ToString() + " " + "B=" + (dataGridView1[i, j].Style.BackColor.B).ToString() + " "+"A=" + (dataGridView1[i, j].Style.BackColor.A).ToString());

                    Trace.WriteLine(lines[n]);
                }

                n = n + 1;
            }


             

            using (StreamWriter file = new StreamWriter(Properties.Settings.Default.WorkPath+"\\" + Properties.Settings.Default.ColorTableFileName))
            {
                foreach (string line in lines)
                {
                     file.WriteLine(line);
                }
            }

        }
    }
}
