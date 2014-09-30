using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.jmPicture1.Load(global::test.Properties.Resources.u45);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var imgofd = new OpenFileDialog();
            imgofd.Filter = "Image File|(*.jpg;*.png;*.bmp)";
            if (imgofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.jmPicture1.Load(imgofd.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.jmPicture1.Rotation(-90);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.jmPicture1.Rotation(90);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.jmPicture1.ScaleImage(1.5f, 1.5f);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.jmPicture1.ScaleImage(0.8f, 0.8f);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.jmPicture1.FullImage();
        }
    }
}
