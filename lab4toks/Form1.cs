using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace lab4toks
{
    public partial class Form1 : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private String inputTextBoxStrToPackage;
        private StringBuilder OldText = new StringBuilder();
        const double TIME_SLOT = 0.0512;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            inputTextBoxStrToPackage = string.Empty;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void BunifuImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Regex.IsMatch(e.KeyChar.ToString(), "[A-Za-z0-9\b]"))
                e.Handled = false;
            else e.Handled = true;
        }
        
        private bool CollisionAnalysis()
        {
            int timeMillisecond = DateTime.Now.Millisecond;
            if (timeMillisecond % 2 == 0)
                return true;
            else return false;
        }

        private bool ListeningCarrierFrequency()
        {
            int timeMillisecond = DateTime.Now.Millisecond;
            if (timeMillisecond % 2 != 0)
                return true;
            else return false;
        }

        private double GetDelay(int count)
        {
            Random randomNumber = new Random();
            int rnd = randomNumber.Next(count, 10);
            return TIME_SLOT * Convert.ToDouble(randomNumber.Next(0, Convert.ToInt32(Math.Pow(2, randomNumber.Next(count, 10)))));
        }

        private void InputTextBox_GotFocus(object sender, EventArgs e) => OldText = new StringBuilder(inputTextBox.Text);

        private void allAnalysis()
        {
            int i = 0;
            debugTextBox.AppendText(">>");
            while (ListeningCarrierFrequency())
            {
                i++;
                if (i > 10)
                {
                    debugTextBox.AppendText(Environment.NewLine);
                    return;
                }
                debugTextBox.AppendText("X");
            }
            System.Threading.Thread.Sleep((int)GetDelay(i));//окно коллизий
            while (CollisionAnalysis())
            {
                i++;
                if (i > 10)
                {
                    debugTextBox.AppendText(Environment.NewLine);
                    System.Threading.Thread.Sleep((int)GetDelay(i));//вычисление случайной задержки
                    return;
                }
                debugTextBox.AppendText("x");
                System.Threading.Thread.Sleep((int)GetDelay(i));//вычисление случайной задержки
            }
            debugTextBox.AppendText(Environment.NewLine);
        }
        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            if (switchPackageMode.Value == true)
            {
                if (OldText.Length <= inputTextBox.Text.Length)
                {
                    inputTextBoxStrToPackage += inputTextBox.Text.Substring(OldText.Length);
                    if (inputTextBoxStrToPackage.Length == 4)
                    {
                        allAnalysis();
                        outputTextBox.AppendText(inputTextBoxStrToPackage);                        
                        inputTextBoxStrToPackage = string.Empty;
                    }
                }
            }
            else
            {
                allAnalysis();
                outputTextBox.AppendText(inputTextBox.Text.Substring(inputTextBox.Text.Length - 1));
            }
            OldText.Length = inputTextBox.Text.Length;
        }
        private void inputTextBox_Click(object sender, EventArgs e)
        {
            if (inputTextBox.SelectionStart != inputTextBox.Text.Length)
            {
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
        }
    }
}
