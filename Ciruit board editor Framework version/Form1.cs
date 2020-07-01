using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ciruit_board_editor_Framework_version
{
    public partial class Form1 : Form
    {

        private Graphics g;
        private Pen p;
        private Point cursor;

        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            p = new Pen(Color.Black, 3);           
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DatabaseControl dc = new DatabaseControl();
            CreateImageBoxes(dc);                   
            /*
            foreach(Element t in dc.GetElements())
            {
                MessageBox.Show(t.GetType());
            }
            */

        }

        private void CreateImageBoxes(DatabaseControl dc)
        {
            PictureBox pb;

            int i = 0;

            foreach(Element t in dc.GetElements())
            {
                int posX = 12 * i * 11;

                pb = new PictureBox();
                pb.Location = new Point(posX, 557);
                pb.Size = new Size(100, 100);
                pb.Image = Image.FromFile(@"Cirquit element images/"+t.GetType()+".png");
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Click += new EventHandler(Image_Click);
                pb.Tag = t.GetType();
                this.Controls.Add(pb);
                i++;
            }
        }

        public void Image_Click(object sender, EventArgs e)
        {           
            PictureBox i = (PictureBox)sender;           
            MessageBox.Show(i.Tag.ToString());
        }             

        private void Form1_Click(object sender, EventArgs e)
        {         
            g.DrawEllipse(p, cursor.X - 10, cursor.Y - 10, 20, 20);
        }
    }
}
