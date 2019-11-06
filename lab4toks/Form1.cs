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
        const double TIME_SLOT = /*0.0512*/3;
        const int COLLISION_WINDOW_TIME = 2;

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

        private bool LateCollisionAnalysis()
        {
            int timeMillisecond = DateTime.Now.Millisecond;
            if (timeMillisecond % 10 == 0)
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

        private String send(String msg) { return msg; }

        private void InputTextBox_GotFocus(object sender, EventArgs e) => OldText = new StringBuilder(inputTextBox.Text);

        private void collisionWindow()
        {
            System.Threading.Thread.Sleep(COLLISION_WINDOW_TIME * 300);//окно коллизий
        }
        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            if (switchPackageMode.Value == true)
            {
                if (OldText.Length <= inputTextBox.Text.Length)
                {
                    inputTextBoxStrToPackage += inputTextBox.Text.Substring(OldText.Length);
                    int j = 0;
                    if (inputTextBoxStrToPackage.Length == 4)
                    {
                        debugTextBox.AppendText(">>");
                        if (j == 0)
                        {
                            do
                            {
                                String symbolSend = send(inputTextBox.Text.Substring(inputTextBox.Text.Length - 1));//отправка символа
                                collisionWindow();
                                if (CollisionAnalysis())
                                {
                                    i++;
                                    if (i > 10)
                                    {
                                        i = 0;
                                        debugTextBox.AppendText(Environment.NewLine);
                                        return;
                                    }
                                    debugTextBox.AppendText("x");
                                    System.Threading.Thread.Sleep((int)GetDelay(i));//вычисление случайной задержки
                                }
                                else break;
                            } while (true);
                            outputTextBox.AppendText(inputTextBoxStrToPackage[j].ToString());
                            j++;
                        }
                        while(j < 4)
                        {
                            if(LateCollisionAnalysis())
                            {
                                debugTextBox.AppendText(Environment.NewLine);
                                debugTextBox.AppendText(">>LateCollision");
                                debugTextBox.AppendText(Environment.NewLine);
                                inputTextBoxStrToPackage = string.Empty;
                                OldText.Length = inputTextBox.Text.Length;
                                return;
                            }                                                      
                            outputTextBox.AppendText(inputTextBoxStrToPackage[j].ToString());
                            j++;
                        }
                        inputTextBoxStrToPackage = string.Empty;
                        debugTextBox.AppendText(Environment.NewLine);
                    }
                }
            }
            else
            {
                debugTextBox.AppendText(">>");
                do
                {
                    while (ListeningCarrierFrequency()) { }//канал свободен или нет?
                    String symbolSend = send(inputTextBox.Text.Substring(inputTextBox.Text.Length - 1));//отправка символа
                    collisionWindow();
                    if (CollisionAnalysis())
                    {
                        i++;
                        if (i > 10)
                        {
                            debugTextBox.AppendText(Environment.NewLine);
                            return;
                        }
                        debugTextBox.AppendText("x");
                        System.Threading.Thread.Sleep((int)GetDelay(i));//вычисление случайной задержки
                    }
                    else break;
                } while (true);
                outputTextBox.AppendText(inputTextBox.Text.Substring(inputTextBox.Text.Length - 1));
                debugTextBox.AppendText(Environment.NewLine);
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
