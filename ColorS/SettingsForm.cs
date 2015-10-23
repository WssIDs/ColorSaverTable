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

namespace ColorS
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

            if (File.Exists(Properties.Settings.Default.WorkPath +"\\"+ Properties.Settings.Default.ColorTableFileName))
            {
                error_label.ForeColor = Color.LightGreen;
                error_label.Text = Properties.Settings.Default.ColorTableFileName + " is ready";

                error_label.Visible = true;
            }
            else
            {
                error_label.ForeColor = Color.Red;
                error_label.Text = "no files";

                error_label.Visible = true;
            }

            workpath_textbox.Text = Properties.Settings.Default.WorkPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkPath = workpath_textbox.Text;
            Properties.Settings.Default.Save();

            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.WorkPath = workpath_textbox.Text;
            Properties.Settings.Default.Save();
        }

        private void pickfolder_button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();

            fdlg.SelectedPath = Properties.Settings.Default.WorkPath;


            if (fdlg.ShowDialog() == DialogResult.OK)
            {


                //Properties.Settings.Default.WorkPath = fdlg.SelectedPath;
                //Properties.Settings.Default.Save();
                workpath_textbox.Text = fdlg.SelectedPath;


                if (File.Exists(fdlg.SelectedPath +"\\"+ Properties.Settings.Default.ColorTableFileName))
                {
                    error_label.ForeColor = Color.LightGreen;
                    error_label.Text = Properties.Settings.Default.ColorTableFileName + " is ready";

                    error_label.Visible = true;
                }
                else
                {
                    error_label.ForeColor = Color.Red;
                    error_label.Text = "no files";

                    error_label.Visible = true;
                }


            }


        }
    }
}
