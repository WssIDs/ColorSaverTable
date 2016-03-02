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
using System.Xml;
using System.Reflection;

using System.Diagnostics;

namespace ColorS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<string> UsedColors = new List<string>();

        string pathToXml = Properties.Settings.Default.WorkPath + "\\" + Properties.Settings.Default.ColorTableFileName;

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
        }

        private Color RandColor()
        {
            Random x = new Random();
            int r, g, b;
            Color myRgbColor = new Color();
            while (true)
            {
                r = x.Next(0, 255);
                g = x.Next(0, 255);
                b = x.Next(0, 255);
                if (!UsedColors.Contains(r + "," + g + "," + b))
                {
                    UsedColors.Add(r + "," + g + "," + b);
                    break;
                }
            }
            myRgbColor = Color.FromArgb(r, g, b);
            return myRgbColor;
        }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm set_frm = new SettingsForm();

            set_frm.ShowDialog();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            Save_Data();

        }

        private void Save_Data()
        {

            XmlTextWriter textWritter = new XmlTextWriter(pathToXml, Encoding.UTF8);
            textWritter.WriteStartDocument();
            textWritter.WriteStartElement("head");
            textWritter.WriteEndElement();
            textWritter.Close();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(pathToXml);
            XmlElement xRoot = xDoc.DocumentElement;


            int n = 0;

            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    //Вывод в OutPutWindow
                    Trace.WriteLine(n + 1);

                    // создаем новый элемент color
                    XmlElement colorElem = xDoc.CreateElement("color");
                    // создаем атрибут index
                    XmlAttribute indexAttr = xDoc.CreateAttribute("index");

                    // создаем элементы
                    XmlElement ColumnIndexElem = xDoc.CreateElement("ColumnIndex");
                    XmlElement RowIndexElem = xDoc.CreateElement("RowIndex");
                    XmlElement RChannelElem = xDoc.CreateElement("R");
                    XmlElement GChannelElem = xDoc.CreateElement("G");
                    XmlElement BChannelElem = xDoc.CreateElement("B");
                    XmlElement AChannelElem = xDoc.CreateElement("A");

                    // создаем текстовые значения для элементов и атрибута
                    XmlText indexText = xDoc.CreateTextNode(n.ToString());

                    XmlText ColumnIndex = xDoc.CreateTextNode(i.ToString());
                    XmlText RowIndex = xDoc.CreateTextNode(j.ToString());
                    XmlText RValue = xDoc.CreateTextNode((Convert.ToInt32(dataGridView1[i, j].Style.BackColor.R)).ToString());
                    XmlText GValue = xDoc.CreateTextNode((Convert.ToInt32(dataGridView1[i, j].Style.BackColor.G)).ToString());
                    XmlText BValue = xDoc.CreateTextNode((Convert.ToInt32(dataGridView1[i, j].Style.BackColor.B)).ToString());
                    XmlText AValue = xDoc.CreateTextNode((Convert.ToInt32(dataGridView1[i, j].Style.BackColor.A)).ToString());

                    //добавляем узлы
                    indexAttr.AppendChild(indexText);

                    ColumnIndexElem.AppendChild(ColumnIndex);
                    RowIndexElem.AppendChild(RowIndex);

                    RChannelElem.AppendChild(RValue);
                    GChannelElem.AppendChild(GValue);
                    BChannelElem.AppendChild(BValue);
                    AChannelElem.AppendChild(AValue);

                    // добавляем аттрибут
                    colorElem.Attributes.Append(indexAttr);

                    // добавляем узлы
                    colorElem.AppendChild(ColumnIndexElem);
                    colorElem.AppendChild(RowIndexElem);
                    colorElem.AppendChild(RChannelElem);
                    colorElem.AppendChild(GChannelElem);
                    colorElem.AppendChild(BChannelElem);
                    colorElem.AppendChild(AChannelElem);

                    xRoot.AppendChild(colorElem);
                    xDoc.Save(pathToXml);

                    n = n + 1;
                }

            }
        }


        private void findfile_timer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.WorkPath + "\\" + Properties.Settings.Default.ColorTableFileName))
            {
                get_data_button.Enabled = true;
                loadDataToolStripMenuItem.Enabled = true;
            }
            else
            {
                get_data_button.Enabled = false;
                loadDataToolStripMenuItem.Enabled = false;
            }
        }

        private void get_data_button_Click(object sender, EventArgs e)
        {
            Get_Data();
            
        }

        private void generateRandomColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
                for (int i = 0; i < 14; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        dataGridView1[i, j].Style.BackColor = RandColor();
                    }
                }
        }


        private void Get_Data()
        {

            List<ColorTable> colorTable = new List<ColorTable>();



            int Red = 0;
            int Green = 0;
            int Blue = 0;
            int Alpha = 0;



            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(pathToXml);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                ColorTable clrTable = new ColorTable();

                // получаем атрибут name
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("index");
                    //if (attr != null)
                    //    Trace.WriteLine(attr.Value);
                }
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {




                    if (childnode.Name == "ColumnIndex")
                    {
                        //Trace.WriteLine("Column: "+ childnode.InnerText);
                        clrTable.Column = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "RowIndex")
                    {
                        //Trace.WriteLine("Row: "+ childnode.InnerText);
                        clrTable.Row = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "R")
                    {
                        //Trace.WriteLine("R= "+ childnode.InnerText);
                        Red = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "G")
                    {
                        //Trace.WriteLine("G= "+ childnode.InnerText);
                        Green = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "B")
                    {
                        // Trace.WriteLine("B= "+ childnode.InnerText);
                        Blue = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "A")
                    {
                        //Trace.WriteLine("A= "+ childnode.InnerText);
                        Alpha = Convert.ToInt32(childnode.InnerText);
                    }

                }

                clrTable.color = Color.FromArgb(Alpha, Red, Green, Blue);
                colorTable.Add(clrTable);
            }

            for (int i = 0; i < colorTable.Count; i++)
            {
                dataGridView1[colorTable[i].Column, colorTable[i].Row].Style.BackColor = colorTable[i].color;
            }
        }


        private void Get_Data_FromPreset(string path)
        {

            List<ColorTable> colorTable = new List<ColorTable>();



            int Red = 0;
            int Green = 0;
            int Blue = 0;
            int Alpha = 0;



            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(path);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                ColorTable clrTable = new ColorTable();

                // получаем атрибут name
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("index");
                    //if (attr != null)
                    //    Trace.WriteLine(attr.Value);
                }
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {




                    if (childnode.Name == "ColumnIndex")
                    {
                        //Trace.WriteLine("Column: "+ childnode.InnerText);
                        clrTable.Column = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "RowIndex")
                    {
                        //Trace.WriteLine("Row: "+ childnode.InnerText);
                        clrTable.Row = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "R")
                    {
                        //Trace.WriteLine("R= "+ childnode.InnerText);
                        Red = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "G")
                    {
                        //Trace.WriteLine("G= "+ childnode.InnerText);
                        Green = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "B")
                    {
                        // Trace.WriteLine("B= "+ childnode.InnerText);
                        Blue = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "A")
                    {
                        //Trace.WriteLine("A= "+ childnode.InnerText);
                        Alpha = Convert.ToInt32(childnode.InnerText);
                    }

                }

                clrTable.color = Color.FromArgb(Alpha, Red, Green, Blue);
                colorTable.Add(clrTable);
            }

            for (int i = 0; i < colorTable.Count; i++)
            {
                dataGridView1[colorTable[i].Column, colorTable[i].Row].Style.BackColor = colorTable[i].color;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            label2.Text = e.ColumnIndex + ":" + e.RowIndex;

            dataGridView1.Refresh();
        }


        private void ClearDataGridView()
        {
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    dataGridView1[i, j].Style.BackColor = Color.White;
                }
            }
        }


        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ColorDialog cdlg = new ColorDialog();

            if (e.RowIndex >= 1 || e.ColumnIndex >=0)
            {
                cdlg.Color = dataGridView1[e.ColumnIndex, e.RowIndex-1].Style.BackColor;
            }

            else
            {
                cdlg.Color = dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;
            }

                if (cdlg.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = cdlg.Color;
                }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearDataGridView();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Get_Data();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_Data();
        }

        private void clearToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ClearDataGridView();
        }

        private void dataGridView1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void loadDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Get_Data();
        }

        private void loadPresetLightToDarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string apppath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            string pathfile = apppath + "\\" + "data\\template_ltd.xml";

            Get_Data_FromPreset(pathfile);
        }

        private void loadPresetDarkToLightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string apppath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            string pathfile = apppath + "\\" + "data\\template_dtl.xml";

            Get_Data_FromPreset(pathfile);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();

            ab.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {


            if (MessageBox.Show("Do you want to exit?", "Color Table Generator",MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                e.Cancel = false;
             //   Application.Exit();
            }

            else
            {
                e.Cancel = true;
            }
        }
           
        
        
        void SortMassive()
        {
            Color clr = new Color();


            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {

                for (int k = 0; k < dataGridView1.RowCount / 2; k++)
                {
                    int n = dataGridView1.RowCount - 1;

                    clr = dataGridView1[i, k].Style.BackColor;

                    dataGridView1[i, k].Style.BackColor = dataGridView1[i, n - k].Style.BackColor;

                    dataGridView1[i, n - k].Style.BackColor = clr;
                }

            }
        } 

            
    }
}
