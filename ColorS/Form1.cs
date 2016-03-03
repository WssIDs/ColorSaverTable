
#define DEBUG

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
    /// <summary>
    /// Main Form
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Template Menu Item
        /// </summary>
        ToolStripMenuItem fileItem = new ToolStripMenuItem("Templates");

        List<string> UsedColors = new List<string>();

        /// <summary>
        /// count templates(XML files) in "data"
        /// </summary>
        int TemplateCounts = 0;

        /// <summary>
        /// color_table.xml location
        /// </summary>
        string pathToXml = Properties.Settings.Default.WorkPath + "\\" + Properties.Settings.Default.ColorTableFileName;

        public Form1()
        {
            InitializeComponent();

#if (DEBUG)
            Trace.WriteLine("DEBUG version");
            Trace.WriteLine("main Xml ="+ pathToXml);
#else
        Trace.WriteLine("DEBUG is not defined");
#endif
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

            ColorItems(Color.White);

            string apppath = AppDomain.CurrentDomain.BaseDirectory;

            string path = apppath + @"\data\";
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*.xml");

            TemplateCounts = Files.Count();

#if (DEBUG)
            Trace.WriteLine("TemplateCount = " + TemplateCounts);
#endif

            GenerateTemplatesMenuItems();


            if (File.Exists(Properties.Settings.Default.WorkPath + "\\" + Properties.Settings.Default.ColorTableFileName))
            {
                if(Properties.Settings.Default.LoadColorTable)
                {
                    Get_Data();
                }
            }

        }

        /// <summary>
        /// Generate Random Color
        /// </summary>
        /// <returns>Color</returns>
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
#if (DEBUG)
                    Trace.WriteLine("RandColorGenerate = " + r+","+","+g+","+b);
#endif
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

#if (DEBUG)
            Trace.WriteLine("Settings");
#endif

            set_frm.ShowDialog();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            Save_Data();

        }

        /// <summary>
        /// Save Data to XML file from datagridview
        /// </summary>
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
#if (DEBUG)
                    Trace.WriteLine(n + 1);
#endif

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


            MessageBox.Show("Saved", this.Text, MessageBoxButtons.OK,MessageBoxIcon.Information);
#if (DEBUG)
            Trace.WriteLine("Saved");
#endif
        }

        /// <summary>
        /// Save Data to XML file from datagridview
        /// </summary>
        /// <param name="templatepath">full path template file</param>
        private void Save_Data(string templatepath)
        {

            XmlTextWriter textWritter = new XmlTextWriter(templatepath, Encoding.UTF8);
            textWritter.WriteStartDocument();
            textWritter.WriteStartElement("head");
            textWritter.WriteEndElement();
            textWritter.Close();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(templatepath);
            XmlElement xRoot = xDoc.DocumentElement;


            int n = 0;

            for (int j = 0; j < dataGridView1.RowCount; j++)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {

#if (DEBUG)
                    Trace.WriteLine(n + 1);
#endif

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
                    xDoc.Save(templatepath);

                    n = n + 1;
                }

            }


#if (DEBUG)
            Trace.WriteLine("Saved Template");
#endif
        }

        private void findfile_timer_Tick(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.WorkPath + "\\" + Properties.Settings.Default.ColorTableFileName))
            {
                get_data_button.Enabled = true;
                openToolStripMenuItem.Enabled = true;
            }
            else
            {
#if (DEBUG)
                Trace.WriteLine("Color Table File is not found");
#endif

                get_data_button.Enabled = false;
                openToolStripMenuItem.Enabled = false;
            }



            string apppath = AppDomain.CurrentDomain.BaseDirectory;

            string path = apppath + @"\data\";
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files

            if (Files.Count() != TemplateCounts)
            {
#if (DEBUG)
                Trace.WriteLine("Template File Count =" +Files.Count().ToString());
#endif


                TemplateCounts = Files.Count();
                //MessageBox.Show("Need Generate files");



                fileItem.DropDownItems.Clear();
                GenerateTemplatesMenuItems();


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

        /// <summary>
        /// Get Data from XML file and generate to datagridview
        /// </summary>
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

        /// <summary>
        /// Get All Data from XML file and generate to datagridview
        /// </summary>
        /// <param name="path">full path filename</param>
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
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {
            }
            else
            {

                label2.Text = e.ColumnIndex + ":" + e.RowIndex+" "+ dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;

                pictureBox1.BackColor = dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;

                dataGridView1.Refresh();
            }
        }

        /// <summary>
        /// Clear datagridview cells
        /// </summary>
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

            if (e.ColumnIndex < 0 || e.RowIndex < 0)
            {

            }

            else
            {
                cdlg.Color = dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;

                if (cdlg.ShowDialog() == DialogResult.OK)
                {

                    dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = cdlg.Color;
                    label2.Text = e.ColumnIndex + ":" + e.RowIndex + "  " + dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;
                    pictureBox1.BackColor = dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;
                }
            }
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
            //MessageBox.Show(@"[" + (dataGridView1.CurrentCell.ColumnIndex).ToString() + "," + (dataGridView1.CurrentCell.RowIndex).ToString() + "]");


            //Проверка имени нажатой клавиши
            //MessageBox.Show(e.KeyCode.ToString());

            if (e.KeyCode == Keys.Oemplus)
            {

                if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R < 255)
                {
                    if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G < 255)
                    {
                        if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B < 255)
                        {
                            Color newcolor = Color.FromArgb(dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R + 1, dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G + 1, dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B + 1);

                            dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor = newcolor;

                            label2.Text = dataGridView1.CurrentCell.ColumnIndex + ":" + dataGridView1.CurrentCell.RowIndex + " " + dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor;

                            pictureBox1.BackColor = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor;
                        }

                        else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B >= 255)
                        {
                            MessageBox.Show("Maximum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G >= 255)
                    {
                        MessageBox.Show("Maximum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R >= 255)
                {
                    MessageBox.Show("Maximum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            if (e.KeyCode == Keys.OemMinus)
            {

                if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R > 0)
                {
                    if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G > 0)
                    {
                        if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B > 0)
                        {
                            Color newcolor = Color.FromArgb(dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R - 1, dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G - 1, dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B - 1);

                            dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor = newcolor;

                            label2.Text = dataGridView1.CurrentCell.ColumnIndex + ":" + dataGridView1.CurrentCell.RowIndex + " " + dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor;

                            pictureBox1.BackColor = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor;
                        }

                        else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.B <= 0)
                        {
                            MessageBox.Show("Minimum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.G <= 0)
                    {
                        MessageBox.Show("Minimum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                else if (dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex].Style.BackColor.R <= 0)
                {
                    MessageBox.Show("Minimum color value", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
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
            }

            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Sort array colors (1<->last)(2<->last-1) etc
        /// </summary>
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

        /// <summary>
        /// Generate Menu Items  
        /// </summary>
        void GenerateTemplatesMenuItems()
        {


            string apppath = AppDomain.CurrentDomain.BaseDirectory;

            string path = apppath + @"\data\";
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*.xml"); //Getting Text files

            List<ToolStripMenuItem> TemplateItems = new List<ToolStripMenuItem>();

            foreach (FileInfo file in Files)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(file.Name));
                item.Click += Item_Click;

                TemplateItems.Add(item);
            }



            foreach (ToolStripMenuItem items in TemplateItems)
            {
                fileItem.DropDownItems.Add(items);

            }


            fileItem.DropDownItemClicked += DropDownItemClicked;

            menuStrip1.Items.Add(fileItem);
        }

        private void Item_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < fileItem.DropDownItems.Count; i++)
            {
                ToolStripMenuItem item =  (ToolStripMenuItem)fileItem.DropDownItems[i];

                item.Checked = false;
            }



            ToolStripMenuItem senderButton = (ToolStripMenuItem)sender;

            senderButton.Checked = true;
        }

        private void DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string apppath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

            string pathfile = apppath + "\\" + "data\\"+ e.ClickedItem.Text+".xml";


            Get_Data_FromPreset(pathfile);
        }

        /// <summary>
        /// Set Custom color for datagrid cells
        /// </summary>
        /// <param name="InColor"></param>
        void ColorItems(Color InColor)
        {
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    dataGridView1[i, j].Style.BackColor = InColor;

#if (DEBUG)
                    Trace.WriteLine("Colors["+i+","+j+"]"+"= " + InColor);
#endif
                }
            }
        }

        private void openToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Get_Data();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ColorItems(Color.White);
        }

        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveTempdlg = new SaveFileDialog();
            saveTempdlg.Title = "Save Template";
            saveTempdlg.Filter = "Template Files(XML)|*.xml";
            saveTempdlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory +"data";

            if(saveTempdlg.ShowDialog() == DialogResult.OK)
            {
                Save_Data(saveTempdlg.FileName);
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SortMassive();
        }
    }
}
