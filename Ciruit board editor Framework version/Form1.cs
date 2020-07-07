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
        private Pen p = new Pen(Color.Red, 3);

        private List<ConnectionDotInfo> cdiList = new List<ConnectionDotInfo>();

        private int x = 0;
        private int y = 0;

        private int queue = 0;

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
                pb.MouseEnter += new EventHandler(PictureBoxDynamic_MouseEnter);
                pb.MouseLeave += new EventHandler(PictureBoxDynamic_MouseLeave);
                pb.Tag = pictureName + queue;

                ConnectionDotInfo cdi = new ConnectionDotInfo(pb.Tag.ToString());

                this.Controls.Add(pb);
                for(int i=0;i<4;i++)
                    CreateConnectionDots(i, cdi);                
                cdiList.Add(cdi);
                HideConnectionDots(pb);
                queue++;
            }
        }       

        private void PictureBoxDynamic_MouseEnter(object sender, EventArgs e)
        {
            PictureBox pbElement = (PictureBox)sender;
            ShowConnectionDots(pbElement);
        }

        private void PictureBoxDynamic_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pbElement = (PictureBox)sender;
            HideConnectionDots(pbElement);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            SnapToClosestDot(e);

            if (previousX != x || previousY != y)
            {
                p.Color = Color.Black;
                g.DrawEllipse(p, previousX, previousY, 20, 20);
            }
            p.Color = Color.Red;

            g.DrawEllipse(p, x, y, 20, 20);

            previousX = x;
            previousY = y;
        }

        private void PictureBoxDot_MouseEnter(object sender, EventArgs e)
        {            
            PictureBox pb = (PictureBox)sender;
            pb.Image = Image.FromFile(@"WireDots/dotRed.png");           
        }

        private void PictureBoxDot_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.Image = Image.FromFile(@"WireDots/dot.png");         
        }

        private void CreateConnectionDots(int i, ConnectionDotInfo cdi)
        {
            int sign=1;

            if (i > 1)
                sign = -1;

            PictureBox pb = new PictureBox();
            if(i==0||i==2)
                pb.Location = new Point(x-15*sign, y);
            else
                pb.Location = new Point(x, y-15*sign);
            pb.Size = new Size(15, 15);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Image = Image.FromFile(@"WireDots/dot.png");

            pb.MouseEnter += new EventHandler(PictureBoxDot_MouseEnter);
            pb.MouseLeave += new EventHandler(PictureBoxDot_MouseLeave);

            this.Controls.Add(pb);

            cdi.AddDotToList(pb);

            pb.BringToFront();          
        }

        private void HideConnectionDots(PictureBox pbElement)
        {
            foreach (ConnectionDotInfo cdi in cdiList)
            {
                if (cdi.GetTag() == pbElement.Tag.ToString())
                {
                    foreach (PictureBox pb in cdi.GetPictureBoxList())
                    {
                        pb.Hide();
                    }
                }
            }
        }

        private void ShowConnectionDots(PictureBox pbElement)
        {
            foreach(ConnectionDotInfo cdi in cdiList)
            {
                if(cdi.GetTag()==pbElement.Tag.ToString())
                {
                    foreach (PictureBox pb in cdi.GetPictureBoxList())
                    {
                        pb.Show();
                    }                    
                }
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
    }
}
