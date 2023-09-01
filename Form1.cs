using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedOperasi
{
    public partial class Form1 : Form
    {
        private int counter_down = 0;
        private SerialPortService _serialPortService;
        private string key_config;
        private string key_save;
        private string key_run;
        private string key_close;
        private string key_stop;
        private string calibarate_temp;
        private string calibarate_humidity;
        private string count_down;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Form1()
        {
            InitializeComponent();
            _serialPortService = new SerialPortService();
            count_down = "00:00:00";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setDateTime();
            setPosition();
            setColor();
            setValue(0, 0);
            settingBackground();
            setKeyBoard();
            counter_down = setCountDown();
            this.textBox1.Hide();
            this.Focus();
            timer2.Enabled = false;
            //if something OK with COM Port, try to open port.
            if(_serialPortService.setSerialPort())
                timer3.Enabled = _serialPortService.openPort();
        }

        private void settingBackground()
        {
            Application.DoEvents();
            this.BackgroundImage = Image.FromFile(Application.StartupPath + "\\background.png");
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        }

        private void setPosition()
        {
            int topPosition = Convert.ToInt32(ConfigurationManager.AppSettings.Get("position_top"));
            int spaceBetwen = Convert.ToInt32(ConfigurationManager.AppSettings.Get("space_between"));
            int position_left = Convert.ToInt32(ConfigurationManager.AppSettings.Get("position_left"));

            lblTime.Left = position_left;
            
            lblDate.Left =  position_left;
            lblTemp.Left = position_left;
            lblHumidity.Left = position_left;
            lblCountdown.Left = position_left;

            lblTime.Top = topPosition;
            lblDate.Top = lblTime.Top + lblTime.Height ;
            lblTemp.Top = lblDate.Top + lblDate.Height ;
            lblHumidity.Top = lblTemp.Top + lblTemp.Height ;
            lblCountdown.Top = lblHumidity.Top + lblHumidity.Height ;

            label7.Top = lblTime.Top;
            label8.Top = lblDate.Top;
            lblTempValue.Top = lblTemp.Top;
            lblHumidityValue.Top = lblHumidity.Top;
            label6.Top = lblCountdown.Top;
            textBox1.Top = lblCountdown.Top;

            string sLeftAdjustment= ConfigurationManager.AppSettings.Get("position_left_adjustment");
            label7.Left = label7.Left + Convert.ToInt32(sLeftAdjustment);
            label8.Left = label7.Left;
            lblTempValue.Left = label7.Left;
            lblHumidityValue.Left = label7.Left;
            label6.Left = label7.Left;

            label7.AutoSize = false;
            label8.AutoSize = false;
            lblTempValue.AutoSize = false;
            lblHumidityValue.AutoSize = false;
            label6.AutoSize = false;

            label7.TextAlign= ContentAlignment.MiddleRight;
            label8.TextAlign = ContentAlignment.MiddleRight;
            lblTempValue.TextAlign = ContentAlignment.MiddleRight;
            lblHumidityValue.TextAlign = ContentAlignment.MiddleRight;
            label6.TextAlign = ContentAlignment.MiddleRight;

            label7.Width = label8.Width;
            lblTempValue.Width = label7.Width;
            lblHumidityValue.Width = label7.Width;
            label6.Width = label7.Width;
        }
        private void setColor()
        {
            string sColor = ConfigurationManager.AppSettings.Get("color");
            string sColorTemperatur = ConfigurationManager.AppSettings.Get("color_temperature");
            string sColorHumidity = ConfigurationManager.AppSettings.Get("color_humidity");
            string sFontName = ConfigurationManager.AppSettings.Get("font_name");
            string sFontLabelName = ConfigurationManager.AppSettings.Get("font_name_label");
            string sFontSize= ConfigurationManager.AppSettings.Get("font_size");
            Color c = (Color)((new ColorConverter()).ConvertFromString(sColor));
            Font myfont = new Font(sFontName, Convert.ToInt32(sFontSize));
            Font myfontlabel = new Font(sFontLabelName, Convert.ToInt32(sFontSize));

            Label mylabel;

            foreach (Control con in this.Controls)
            {
                if (con.GetType() == typeof(Label)) //or any other logic
                {
                    mylabel = (Label)con;
                    mylabel.ForeColor = c;
                    mylabel.Font = myfont;
                    mylabel.BackColor = Color.Transparent;
                }
            }

            c = (Color)((new ColorConverter()).ConvertFromString(sColorTemperatur));
            lblTemp.ForeColor = c;
            lblTemp.Font = myfont;
            c = (Color)((new ColorConverter()).ConvertFromString(sColorHumidity));
            lblHumidity.ForeColor = c;
            lblHumidity.Font = myfont;

            lblTime.Font = myfontlabel;
            lblDate.Font = myfontlabel;
            lblTemp.Font = myfontlabel;
            lblHumidity.Font = myfontlabel;
            lblCountdown.Font = myfontlabel;
        }

        private void setKeyBoard()
        {
            key_config = ConfigurationManager.AppSettings.Get("key_configure");
            key_save= ConfigurationManager.AppSettings.Get("key_save");
            key_run= ConfigurationManager.AppSettings.Get("key_run");
            key_close = ConfigurationManager.AppSettings.Get("key_close");
            key_stop = ConfigurationManager.AppSettings.Get("key_stop");
            calibarate_temp = ConfigurationManager.AppSettings.Get("calibarate_temp");
            calibarate_humidity = ConfigurationManager.AppSettings.Get("calibarate_humidity");

        }
        private void setValue(float degrees,float humidity)
        {
            lblTempValue.Text = string.Format("{0}\u00B0C", degrees + Convert.ToInt32(calibarate_temp));
            lblHumidityValue.Text = string.Format("{0}%", humidity + Convert.ToInt32(calibarate_humidity));
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            _log.Debug(String.Format("Form1_KeyUp:{0} {1}", e.KeyData,e.KeyValue));
            _log.Debug(String.Format("Form1_KeyUp:{0}", e.KeyCode));

            Keys config = (Keys)int.Parse(key_config);
            Keys run = (Keys)int.Parse(key_run);
            Keys close = (Keys)int.Parse(key_close);
            Keys stop = (Keys)int.Parse(key_stop);

            if (e.KeyData== close)
                this.Close();

            if (e.KeyData == config)
            {
                label6.Hide();
                textBox1.Text = count_down;
                textBox1.Show();
                textBox1.ReadOnly = false;
                textBox1.SelectionStart = 0;
                textBox1.SelectionLength = textBox1.TextLength;
                textBox1.Select();
            }
            else if (e.KeyData == stop)
            {
                timer2.Enabled = false;
                timer2.Stop();
            }
            else if (e.KeyData == run)
            {
                timer2.Enabled = true;
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            setDateTime();
        }

        private void setDateTime()
        {
            label7.Text = DateTime.Now.ToString("HH:mm:ss");
            label8.Text = DateTime.Now.ToString("dd-MM-yyyy");
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private int setCountDown()
        {
            var countdown = label6.Text.Split(':');
            int counter = Int32.Parse(countdown[0]) * 3600;
            counter += Int32.Parse(countdown[1]) * 60;
            counter += Int32.Parse(countdown[2]);
            return counter;
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            if (this.counter_down == 0)
            {
                //play sound beep
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Application.StartupPath +  @"\ledoperasi.wav");
                player.Play();
                return;
            }
            this.counter_down -= 1;
            convertToTime();
            timer2.Enabled = true;
        }

        private void convertToTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(this.counter_down);
            string str = time.ToString(@"hh\:mm\:ss");
            this.label6.Text = str;
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            //dfd
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            //if port is not open trial to open again and again...
            timer3.Enabled = false;
            setValue(_serialPortService.readTemperature(), _serialPortService.readHumidity());
            timer3.Enabled = true;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmClose frm = new FrmClose();
            frm.ShowDialog();
            if (frm.Keluar)
            {
                timer3.Enabled = false;
                timer3.Stop();
                if (_serialPortService != null)
                    _serialPortService.stopSerialService();
                _serialPortService = null;
            }
            else
                e.Cancel = true;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Keys save = (Keys)int.Parse(key_save);
            _log.Debug(String.Format("textBox1_KeyPress:{0}", e.KeyChar));
            
            if ((Keys)e.KeyChar == save)
            {
                textBox1.BorderStyle = BorderStyle.None;
                textBox1.TabStop = false;
                textBox1.ReadOnly = true;
                textBox1.Hide();
                string varTime = textBox1.Text;
                label6.Text = varTime.Substring(0,2) + ":" + varTime.Substring(2,2) + ":" + varTime.Substring(4,2);
                count_down = textBox1.Text;
                this.counter_down = setCountDown();
                label6.Show();
                //this.Hide();
                Application.DoEvents();
                this.Activate();
                this.Show();
                this.Focus();
                this.Select();
            }
            else
            {
                count_down = textBox1.Text;
            }
            
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {

        }

        private void label6_Click_1(object sender, EventArgs e)
        {

        }
    }
}
