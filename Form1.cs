using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AddDebug
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string path = "";
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择.cs所在文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.SelectedPath;
                this.textBox1.Text = path;

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(path);
            FileInfo[] fileInfo = theFolder.GetFiles("*.cs");

            string functionName = "";
            string fileName = "";
            foreach (FileInfo fi in fileInfo)
            {
                if (!fi.FullName.Contains("Designer"))
                {
                    textBox2.AppendText("正在处理" + fi.FullName + "\r\n");
                    StreamReader sr = new StreamReader(fi.FullName,Encoding.Default);
                    StreamWriter sw = new StreamWriter(fi.FullName + "1",false,Encoding.Default);
                    fileName = Path.GetFileName(fi.FullName);
                    string oneLine = sr.ReadLine();
                    while (oneLine != null)
                    {
                        if (!oneLine.Contains("System.Diagnostics.Debug.WriteLine"))
                        {
                            functionName = "";
                            string[] splitOne = oneLine.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            sw.WriteLine(oneLine);
                            if (splitOne.Length >= 3 && oneLine.Contains('(') && oneLine.Contains(')') && (oneLine.Contains("public") || oneLine.Contains("private") || oneLine.Contains("protected")) && !oneLine.Contains(":") && !oneLine.Contains(";"))//判断函数名
                            {
                                for (int ii = 2; ii < splitOne.Length; ii++)
                                    functionName = functionName + splitOne[ii] + " ";
                                sw.WriteLine(sr.ReadLine());
                                sw.WriteLine("\tSystem.Diagnostics.Debug.WriteLine(\"" + functionName + "\", \"" + fileName + "\");");
                            }
                        }
                        oneLine = sr.ReadLine();
                    }
                    sr.Close();
                    sw.Close();
                    FileInfo fiModify = new FileInfo(fi.FullName + "1");
                    fi.Delete();
                    fiModify.MoveTo(fiModify.FullName.Substring(0, fiModify.FullName.Length - 1));
                    textBox2.AppendText("OK\r\n");
                }
            }
        }
    }
}
