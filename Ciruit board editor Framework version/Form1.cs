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

        private Graphics g;
        Pen p = new Pen(Color.Red, 3);

        private int x = 0;
        private int y = 0;

        int previousX = 0;
        int previousY = 0;

        private DatabaseControl dc = new DatabaseControl();
        private string pictureName = "";

        private List<WindowGridPosition> wgpList = new List<WindowGridPosition>();

        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();                   
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
            pictureName = i.Tag.ToString();
        }             

        private void Form1_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {           
            DrawDots();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {           
            CreateNewImageOnPanel(e);
        }

        private void CreateNewImageOnPanel(MouseEventArgs e)
        {
            if (pictureName != ""&& e.Y < 500)
            {
                SnapToClosestDot(e);
                PictureBox pb = new PictureBox();              
                pb.Location = new Point(x-14, y-13);
                pb.Size = new Size(50, 50);
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Image = Image.FromFile(@"Cirquit element images/" + pictureName + ".png");
                this.Controls.Add(pb);
            }
        }       

        private void SnapToClosestDot(MouseEventArgs cursor)
        {
            int distanceX=0;
            int distanceY=0;

            int oldDistanceX = 9999;
            int oldDistanceY = 9999;            

            foreach(WindowGridPosition pos in wgpList)
            {
                distanceX = Math.Abs(pos.GetX() - cursor.X);
                distanceY = Math.Abs(pos.GetY() - cursor.Y);
                if (oldDistanceX > distanceX)
                {
                    oldDistanceX = distanceX;
                    x = pos.GetX();
                }
                if (oldDistanceY > distanceY)
                {
                    oldDistanceY = distanceY;
                    y = pos.GetY();
                }

            }
            //MessageBox.Show(oldDistanceX + " and " + oldDistanceY);
        }

        private void DrawDots()
        {            
            int posX;
            int posY;

            Pen myPen = new Pen(Color.Black, 3);
            for (int j = 0; j < 18; j++)
            {
                posY = 30 * j;
                
                for (int i = 0; i < 18; i++)
                {
                    posX = 12 * i * 4;                    
                    g.DrawEllipse(myPen, posX, posY, 20, 20);

                    WindowGridPosition wgp = new WindowGridPosition(posX, posY);
                    wgpList.Add(wgp);
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {            
            SnapToClosestDot(e);            
            
            if(previousX!=x || previousY!=y)
            {
                p.Color = Color.Black;
                g.DrawEllipse(p, previousX, previousY, 20, 20);
            }
            p.Color = Color.Red;

            g.DrawEllipse(p, x, y, 20, 20);
            
            previousX = x;
            previousY = y;
        }
    }
}
