using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Runtime.CompilerServices;

namespace Ciruit_board_editor_Framework_version
{
    public partial class Form1 : Form
    {

        //private Graphics g;
        //private Pen p;
        //private Point cursor;

        private int x = 0;
        private int y = 0;

        DatabaseControl dc = new DatabaseControl();
        private string pictureName = "";       

        public Form1()
        {
            InitializeComponent();
            //g = this.CreateGraphics();
            //p = new Pen(Color.Black, 3);           
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {                                  
            CreateImageBoxes();
            /*
            foreach(Element t in dc.GetElements())
            {
                MessageBox.Show(t.GetType());
            }
            */                                  
        }

        private void CreateImageBoxes()
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
            //MessageBox.Show(i.Tag.ToString());
            pictureName = i.Tag.ToString();
        }             

        private void Form1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(cursor.X + " AND " + cursor.Y);
            //g.DrawEllipse(p, cursor.X - 10, cursor.Y - 10, 20, 20);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            /*Color lightGray = Color.FromArgb(224, 227, 213);            
            Pen myPen = new Pen(Color.Black, 3);
            Brush myBrush = new SolidBrush(lightGray);
            Rectangle rect = new Rectangle(12, 27, 740, 500);
            g.FillRectangle(myBrush, rect);
            DrawGrids();
            */
            //g.DrawEllipse(p, cursor.X - 10, cursor.Y - 10, 200, 200);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //g.DrawEllipse(p, e.X - 10, e.Y - 10, 20, 20);
            CreateNewImageOnPanel(e);
        }

        private void CreateNewImageOnPanel(MouseEventArgs e)
        {
            if (pictureName != ""&& e.Y < 500)
            {
                PictureBox pb = new PictureBox();
                pb.Location = new Point(e.X - 25, e.Y - 25);
                pb.Size = new Size(50, 50);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = Image.FromFile(@"Cirquit element images/" + pictureName + ".png");
                this.Controls.Add(pb);
            }
        }

        private void DrawGrids()
        {
            /*
            Pen myPen = new Pen(Color.LightGray);
            for(int i=0;i<100;i++)
            {
                g.DrawLine(myPen, 12*i, 525, 12*i, 27);                
            }
            for (int i = 0; i < 45; i++)
            {               
                g.DrawLine(myPen, 755, 12 * i, 12, 12 * i);
            }
            */
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
           
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            
        }        
      
    }
}
