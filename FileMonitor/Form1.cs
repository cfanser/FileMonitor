using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace FileCopy
{
    public partial class Form1 : Form
    {
        DataSet ds = new DataSet();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetDataSet();
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                {
                    MessageBox.Show("Slect a Folder,please!");
                }
            }
            else
            {
                MessageBox.Show("Slect a Folder,please!");
                return;
            }
            textBox1.Text = folderBrowserDialog.SelectedPath;
            LoadData();
            dataGridView1.ColumnHeadersHeight = 30;
            dataGridView1.Columns[0].Width = 150;

            DataGridViewColumn dgvc = dataGridView1.Columns[0];
            
            dataGridView1.Sort(dgvc,ListSortDirection.Ascending);
        }

        private void SetDataSet()
        {
            if (ds.Tables.Contains("bak"))
            {
                ds.Tables.Remove("bak");
            }
            
            DataTable dt = new DataTable("Bak");
            
            
            dt.Columns.Add(new DataColumn("考室",typeof(string)));
           
            ds.Tables.Add(dt);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Select floder first,please.");
                return;
            }
            SetDataSet();
            LoadData();
           
        }

        private void LoadData()
        {
           
            DirectoryInfo info = new DirectoryInfo(textBox1.Text);
            foreach (DirectoryInfo d in info.GetDirectories())
            {
                DataRow dr = ds.Tables[0].NewRow();
                dr["考室"] = d.Name;
                DirectoryInfo sinfo = new DirectoryInfo(d.FullName);

                foreach (DirectoryInfo dd in sinfo.GetDirectories())
                {
                    if (!ds.Tables[0].Columns.Contains(dd.Name))
                    {
                        ds.Tables[0].Columns.Add(new DataColumn(dd.Name, typeof(string)));
                    }

                    FileInfo[] finfo = dd.GetFiles("*.hhrdc");
                    dr[dd.Name] = finfo.Length;
                }
                ds.Tables[0].Rows.Add(dr);
            }
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.DataSource = ds.Tables[0];
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                dr.Height = 30;
                foreach (DataGridViewCell cell in dr.Cells)
                {
                    int fileCoount;
                    if (int.TryParse(cell.Value.ToString(), out fileCoount))
                    {
                        if (fileCoount != 2)
                        {
                            cell.Style.ForeColor = Color.Red;
                            cell.Style.BackColor = Color.Aquamarine;
                            cell.Style.Font = new Font("黑体", 11);
                        }
                        this.dataGridView1.Columns[cell.ColumnIndex].Width = 40;
                        DataGridViewCellStyle cStyle = new DataGridViewCellStyle();
                        cStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        this.dataGridView1.Columns[cell.ColumnIndex].DefaultCellStyle = cStyle;                        
                    }
                }
            }
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Select floder first,please.");                
            }
            else
            {
                this.button2.PerformClick();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

       

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                SetTimer();
                this.timer1.Start();
            }
            else
            {
                this.timer1.Stop();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SetTimer();
        }

        private void SetTimer()
        {
            this.timer1.Interval = (int)this.numericUpDown1.Value * 60 * 1000;
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "考室")
            {
                int v1 =Convert.ToInt32(Regex.Match(e.CellValue1.ToString(), @"[\-\d\. ]+").ToString());
                int v2= Convert.ToInt32(Regex.Match(e.CellValue2.ToString(), @"[\-\d\. ]+").ToString());
                if (v1 > v2)
                {
                    e.SortResult = 1;
                }
                else if (v1==v2)
                {
                    e.SortResult = 0;
                }
                else
                {
                    e.SortResult = -1;
                }
                e.Handled = true;
            }
        }
    }
}
