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
    public partial class FrmClose : Form
    {
        public bool Keluar = false;
        public FrmClose()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string kode_close = ConfigurationManager.AppSettings.Get("kode_close");
            Keluar = false;
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                if (textBox1.Text == kode_close)
                {
                    Keluar = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kode yang dimasukkan salah, Silakan ulang atau tekan tombol escape untuk keluar");
                    this.Close();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
