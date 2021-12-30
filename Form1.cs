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
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
namespace Renamer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            resultLabel.Text = "";
        }

        private static Regex r = new Regex(":");

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    pathEdt.Text = fbd.SelectedPath;
                    string[] files = Directory.GetFiles(pathEdt.Text);
                    resultLabel.Text = "Found " + files.Length + " files.";

                }
            }
        }

      /*  class DateTimeCompare : IComparer<string>
        {
            public override int IComparer.Compare(string o1,string o2)
            {
                return 0;
            }
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string[] files = Directory.GetFiles(pathEdt.Text);
                Array.Sort(files, (emp1, emp2) => GetDateTakenFromImage(emp1).CompareTo(GetDateTakenFromImage(emp2)));
                string p = prefixEdt.Text;
                if (p.Length == 0)
                    throw new Exception("Prefix not defined");
                
                for (int i = 0; i < files.Length; i++)
                {
                    // extract extension
                    string filename = files[i];
                    string extension = Path.GetExtension(filename);
                    string baseName = Path.GetFileNameWithoutExtension(filename);
                    string baseDirectory = Path.GetDirectoryName(filename);
                    string target = baseDirectory + "\\" + p + "-" + (i + 1).ToString()  + extension;
                    File.Move(files[i], target);
                }
                resultLabel.Text = "Successfully renamed " + files.Length + " files.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
