using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace smr_read_sn {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void ctms_1time_Click(object sender, EventArgs e) {
            ctms_1time.Checked = true;
            ctms_2time.Checked = false;
            File.WriteAllText("../../config/smr_read_sn_1time.txt", ctms_1time.Checked.ToString());
            File.WriteAllText("../../config/smr_read_sn_2time.txt", ctms_2time.Checked.ToString());
        }

        private void ctms_2time_Click(object sender, EventArgs e) {
            ctms_2time.Checked = true;
            ctms_1time.Checked = false;
            File.WriteAllText("../../config/smr_read_sn_1time.txt", ctms_1time.Checked.ToString());
            File.WriteAllText("../../config/smr_read_sn_2time.txt", ctms_2time.Checked.ToString());
        }

        private void Form1_Load(object sender, EventArgs e) {
            Process[] pname = Process.GetProcessesByName("smr_read_sn");
            if (pname.Length == 2) {
                Application.Exit();
                return;
            }
            try { ctms_1time.Checked = Convert.ToBoolean(File.ReadAllText("../../config/smr_read_sn_1time.txt")); } catch { }
            try { ctms_2time.Checked = Convert.ToBoolean(File.ReadAllText("../../config/smr_read_sn_2time.txt")); } catch { }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            timer1.Enabled = false;
            if (ctms_1time.Checked) textBox2.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (textBox1.Text.Count() < 13) return;
            timer2.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text.Count() < 13) return;
            timer3.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e) {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Focus();
        }

        private int run1 = 0;
        private string head = "";
        private void timer2_Tick(object sender, EventArgs e) {
            timer2.Enabled = false;
            string sn1 = textBox1.Text;
            if (sn1.Length != 13) {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ครบ 13 digit");
                textBox1.Text = "";
                textBox1.Focus();
                return;
            }
            head = sn1.Substring(0, sn1.Length - 6);
            string num1 = sn1.Substring(sn1.Length - 6, 6);
            try { run1 = Convert.ToInt32(num1); } catch {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ตรง Format");
                textBox1.Text = "";
                textBox1.Focus();
                return;
            }
            if (ctms_2time.Checked) { textBox2.Focus(); return; }
            File.WriteAllText("smr_sn_strat.txt", textBox1.Text);
            this.WindowState = FormWindowState.Minimized;
        }

        private void timer3_Tick(object sender, EventArgs e) {
            timer3.Enabled = false;
            string sn1 = textBox2.Text;
            if (sn1.Length != 13) {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ครบ 13 digit");
                textBox2.Text = "";
                textBox2.Focus();
                return;
            }
            int run2 = 0;
            string head2 = sn1.Substring(0, sn1.Length - 6);
            string num1 = sn1.Substring(sn1.Length - 6, 6);
            if(head != head2) {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ตรง Format");
                return;
            }
            try { run2 = Convert.ToInt32(num1); } catch {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ตรง Format");
                textBox2.Text = "";
                textBox2.Focus();
                return;
            }
            if((run2 - run1) != 35 && (run1 - run2) != 35) {
                File.Delete("smr_sn_strat.txt");
                show_form_err("SN ไม่ได้เรียงจาก 1 - 36");
                return;
            }
            if((run2 - run1) == 35) File.WriteAllText("smr_sn_strat.txt", textBox1.Text);
            else File.WriteAllText("smr_sn_strat.txt", textBox2.Text);
            this.WindowState = FormWindowState.Minimized;
        }
        Form form_err;
        private void show_form_err(string s = "") {
            form_err = new Form();
            form_err.Size = new Size(1000, 600);
            form_err.BackColor = Color.Red;
            form_err.StartPosition = FormStartPosition.CenterScreen;
            form_err.KeyDown += F_KeyDown;
            FontFamily fontFamily = new FontFamily("Arial");
            Label l1 = new Label();
            l1.Font = new Font(fontFamily, 50, FontStyle.Bold, GraphicsUnit.Pixel);
            l1.ForeColor = Color.White;
            l1.Location = new Point(50, 50);
            l1.Size = new Size(500, 50);
            l1.Text = s;
            Label l2 = new Label();
            l2.Font = new Font(fontFamily, 50, FontStyle.Bold, GraphicsUnit.Pixel);
            l2.ForeColor = Color.White;
            l2.Location = new Point(100, 125);
            l2.Size = new Size(850, 400);
            l2.Text = mes;

            form_err.Controls.Add(l1);
            form_err.Controls.Add(l2);
            form_err.ShowDialog();
        }
        private void F_KeyDown(object sender, KeyEventArgs e) {
            form_err.Close();
        }
        private string mes = "รูปแบบที่ตรวจสอบคือ...\r\n1.เลข SN 7 หลักแรก ของหัวท้ายจะต้องเหมือนกัน" +
                             "\r\n2.เลข SN 6 หลักหลัง เอามาลบกันจะเท่ากับ 35" +
                             "\r\n3.เลข SN ต้องมีจำนวน 13 digit";

        private void timer4_Tick(object sender, EventArgs e) {
            timer4.Enabled = false;
            try {
                string s = File.ReadAllText("smr_read_sn_show.txt");
                File.Delete("smr_read_sn_show.txt");
                if (s.Contains("L")) test_lift();
                if (s.Contains("R")) test_rigth();
            } catch { timer4.Enabled = true; return; }
            this.WindowState = FormWindowState.Normal;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Focus();
            timer4.Enabled = true;
        }
        private void test_lift() {
            pictureBox1.Image = Properties.Resources.smr_non_;
            pictureBox2.Image = Properties.Resources.smr_non_;
            pictureBox3.Image = Properties.Resources.smr_non_;
            pictureBox4.Image = Properties.Resources.smr_non_;
            pictureBox5.Image = Properties.Resources.smr_non_;
            pictureBox6.Image = Properties.Resources.smr_pass_;
            pictureBox7.Image = Properties.Resources.smr_non_;
            pictureBox8.Image = Properties.Resources.smr_non_;
            pictureBox9.Image = Properties.Resources.smr_non_;
            pictureBox10.Image = Properties.Resources.smr_non_;
            pictureBox11.Image = Properties.Resources.smr_non_;
            pictureBox12.Image = Properties.Resources.smr_non_;
            pictureBox13.Image = Properties.Resources.smr_non_;
            pictureBox14.Image = Properties.Resources.smr_non_;
            pictureBox15.Image = Properties.Resources.smr_non_;
            pictureBox16.Image = Properties.Resources.smr_non_;
            pictureBox17.Image = Properties.Resources.smr_non_;
            pictureBox18.Image = Properties.Resources.smr_non_;
            pictureBox19.Image = Properties.Resources.smr_non_;
            pictureBox20.Image = Properties.Resources.smr_non_;
            pictureBox21.Image = Properties.Resources.smr_non_;
            pictureBox22.Image = Properties.Resources.smr_non_;
            pictureBox23.Image = Properties.Resources.smr_non_;
            pictureBox24.Image = Properties.Resources.smr_non_;
            pictureBox25.Image = Properties.Resources.smr_non_;
            pictureBox26.Image = Properties.Resources.smr_non_;
            pictureBox27.Image = Properties.Resources.smr_non_;
            pictureBox28.Image = Properties.Resources.smr_non_;
            pictureBox29.Image = Properties.Resources.smr_non_;
            pictureBox30.Image = Properties.Resources.smr_non_;
            pictureBox31.Image = Properties.Resources.smr_non_;
            pictureBox32.Image = Properties.Resources.smr_non_;
            pictureBox33.Image = Properties.Resources.smr_non_;
            pictureBox34.Image = Properties.Resources.smr_non_;
            pictureBox35.Image = Properties.Resources.smr_non_;
            pictureBox36.Image = Properties.Resources.smr_pass_;
        }
        private void test_rigth() {
            pictureBox1.Image = Properties.Resources.smr_non;
            pictureBox2.Image = Properties.Resources.smr_non;
            pictureBox3.Image = Properties.Resources.smr_non;
            pictureBox4.Image = Properties.Resources.smr_non;
            pictureBox5.Image = Properties.Resources.smr_non;
            pictureBox6.Image = Properties.Resources.smr_pass;
            pictureBox7.Image = Properties.Resources.smr_non;
            pictureBox8.Image = Properties.Resources.smr_non;
            pictureBox9.Image = Properties.Resources.smr_non;
            pictureBox10.Image = Properties.Resources.smr_non;
            pictureBox11.Image = Properties.Resources.smr_non;
            pictureBox12.Image = Properties.Resources.smr_non;
            pictureBox13.Image = Properties.Resources.smr_non;
            pictureBox14.Image = Properties.Resources.smr_non;
            pictureBox15.Image = Properties.Resources.smr_non;
            pictureBox16.Image = Properties.Resources.smr_non;
            pictureBox17.Image = Properties.Resources.smr_non;
            pictureBox18.Image = Properties.Resources.smr_non;
            pictureBox19.Image = Properties.Resources.smr_non;
            pictureBox20.Image = Properties.Resources.smr_non;
            pictureBox21.Image = Properties.Resources.smr_non;
            pictureBox22.Image = Properties.Resources.smr_non;
            pictureBox23.Image = Properties.Resources.smr_non;
            pictureBox24.Image = Properties.Resources.smr_non;
            pictureBox25.Image = Properties.Resources.smr_non;
            pictureBox26.Image = Properties.Resources.smr_non;
            pictureBox27.Image = Properties.Resources.smr_non;
            pictureBox28.Image = Properties.Resources.smr_non;
            pictureBox29.Image = Properties.Resources.smr_non;
            pictureBox30.Image = Properties.Resources.smr_non;
            pictureBox31.Image = Properties.Resources.smr_non;
            pictureBox32.Image = Properties.Resources.smr_non;
            pictureBox33.Image = Properties.Resources.smr_non;
            pictureBox34.Image = Properties.Resources.smr_non;
            pictureBox35.Image = Properties.Resources.smr_non;
            pictureBox36.Image = Properties.Resources.smr_pass;
        }
    }
}
