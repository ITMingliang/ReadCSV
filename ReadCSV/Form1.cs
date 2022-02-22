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

namespace ReadCSV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //扫描路径
        DirectoryInfo TheFolder = new DirectoryInfo($"{System.Environment.CurrentDirectory}" + "\\实时数据");
        string filePath = $"{System.Environment.CurrentDirectory}" + "\\实时数据\\";
        private void bt_Scan_Click(object sender, EventArgs e)
        {
            comboxFiles.Items.Clear();
            comboxFiles.SelectedIndex = comboxFiles.Items.Count > 0 ? 0 : -1;

            //遍历目录下的文件名
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                this.comboxFiles.Items.Add(NextFile);
            }

            //选择默认
            comboxFiles.SelectedIndex = comboxFiles.Items.Count > 0 ? 0 : -1;
        }

        private void bt_Open_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            var file = File.Open(filePath + comboxFiles.Text, FileMode.Open);
            List<string> list = new List<string>();

            using (var stream = new StreamReader(file))
            {
                while (!stream.EndOfStream)
                {
                    list.Add(stream.ReadLine());
                }
            }

            var array = new string[list.Count, 3];
            var line = 0;
            list.ForEach(item => {
                var row1 = 0;
                item.Split(',').ToList().ForEach(p =>
                {
                    array.SetValue(p, line, row1);
                    row1++;
                });
                line++;
            });
            file.Close();

            //去除标题行，从1行开始
            for (int i = 1; i < list.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                int index = dataGridView1.Rows.Add(row);
                dataGridView1.Rows[index].Cells[0].Value = array[i , 0];
                dataGridView1.Rows[index].Cells[1].Value = array[i , 1];
                dataGridView1.Rows[index].Cells[2].Value = array[i , 2];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboxFiles.SelectedIndex = comboxFiles.Items.Count > 0 ? 0 : -1;
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                this.comboxFiles.Items.Add(NextFile);
            }
            comboxFiles.SelectedIndex = comboxFiles.Items.Count > 0 ? 0 : -1;

            //添加列
            DataGridViewTextBoxColumn col0 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn col1 = new DataGridViewTextBoxColumn();
            DataGridViewTextBoxColumn col2 = new DataGridViewTextBoxColumn();
            col0.HeaderText = "序号";
            col1.HeaderText = "姓名";
            col2.HeaderText = "年龄";
            dataGridView1.Columns.Add(col0);
            dataGridView1.Columns.Add(col1);
            dataGridView1.Columns.Add(col2);

            DataGridViewCheckBoxCell cellText = new DataGridViewCheckBoxCell();
            DataGridViewImageCell cellImage = new DataGridViewImageCell();

        }

        private void bt_Save_Click(object sender, EventArgs e)
        {
            bool f = dataGridViewToCSV(dataGridView1);
        }

  
        public static bool dataGridViewToCSV(DataGridView dataGridView)
        {
            if (dataGridView.Rows.Count == 0)
            {
                MessageBox.Show("没有数据可导出!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AddExtension = true;//设置自动添加文件拓展名
            saveFileDialog.CreatePrompt = true;//创建提示
            saveFileDialog.FileName = null;//默认文件名
            saveFileDialog.Title = "保存";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.GetEncoding(-0));
                string strLine = "";
                try
                {
                    //表头
                    for (int i = 0; i < dataGridView.ColumnCount; i++)
                    {
                        if (i > 0)
                            strLine += ",";
                        strLine += dataGridView.Columns[i].HeaderText;
                    }
                    strLine.Remove(strLine.Length - 1);
                    sw.WriteLine(strLine);
                    strLine = "";

                    //表的内容
                    for (int j = 0; j < dataGridView.Rows.Count; j++)
                    {
                        strLine = "";
                        int colCount = dataGridView.Columns.Count;
                        for (int k = 0; k < colCount; k++)
                        {
                            //以逗号分隔
                            if (k > 0 && k < colCount)
                                strLine += ",";

                            if (dataGridView.Rows[j].Cells[k].Value == null)
                                strLine += "";
                            else
                            {
                                string cell = dataGridView.Rows[j].Cells[k].Value.ToString().Trim();
                                //防止里面含有特殊符号
                                cell = cell.Replace("\"", "\"\"");
                                //cell = "\"" + cell + "\""; //每个元素值用引号包括
                                strLine += cell;
                            }
                        }
                        sw.WriteLine(strLine);
                    }

                    sw.Close();
                    stream.Close();
                    MessageBox.Show("数据被导出到：" + saveFileDialog.FileName.ToString(), "导出完毕", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "导出错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }
    }
}
