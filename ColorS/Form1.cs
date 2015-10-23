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

using System.Diagnostics;

namespace ColorS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

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

        private Color CreateRandomColor()
        {
            Random randonGen = new Random();
            Color randomColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255), randonGen.Next(255));
            return randomColor;
        }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm set_frm = new SettingsForm();

            set_frm.ShowDialog();
        }

        private void save_button_Click(object sender, EventArgs e)
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

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    //Вывод в OutPutWindow
                    Trace.WriteLine(n+1); 
                               
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
            }
            else
            {
                get_data_button.Enabled = false;
            }
        }

        private void get_data_button_Click(object sender, EventArgs e)
        {

            List<ColorTable> colorTable = new List<ColorTable>();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(pathToXml);
            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            // обход всех узлов в корневом элементе
            foreach (XmlNode xnode in xRoot)
            {
                // получаем атрибут name
                if (xnode.Attributes.Count > 0)
                {
                    XmlNode attr = xnode.Attributes.GetNamedItem("index");
                    if (attr != null)
                        Trace.WriteLine(attr.Value);
                }
                // обходим все дочерние узлы элемента user
                foreach (XmlNode childnode in xnode.ChildNodes)
                {

                    ColorTable clrTable = new ColorTable();
                    int Red = 0;
                    int Green = 0;
                    int Blue = 0;
                    int Alpha = 0;

                    if (childnode.Name == "ColumnIndex")
                    {
                        Trace.WriteLine("Column: "+ childnode.InnerText);
                        clrTable.Column = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "RowIndex")
                    {
                        Trace.WriteLine("Row: "+ childnode.InnerText);
                        clrTable.Row = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "R")
                    {
                        Trace.WriteLine("R= "+ childnode.InnerText);
                        Red = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "G")
                    {
                        Trace.WriteLine("G= "+ childnode.InnerText);
                        Green = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "B")
                    {
                        Trace.WriteLine("B= "+ childnode.InnerText);
                        Blue = Convert.ToInt32(childnode.InnerText);
                    }

                    if (childnode.Name == "A")
                    {
                        Trace.WriteLine("A= "+ childnode.InnerText);
                        Alpha = Convert.ToInt32(childnode.InnerText);
                    }

                    clrTable.color =Color.FromArgb(Red, Green, Blue);


                    colorTable.Add(clrTable);
                }
            }


            for (int i = 0; i < colorTable.Count; i++)
            {
                Trace.WriteLine(colorTable[i].color.ToString());
            }
        }

        private void generateRandomColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    dataGridView1[i, j].Style.BackColor = CreateRandomColor();
                    Trace.WriteLine((dataGridView1[i, j].Style.BackColor).ToString());
                }
            }
        }
    }
}
