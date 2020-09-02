using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CircuitBoardDiagram
{
    class DotGUIControl
    {
        private Canvas canvas;
        private Grid grid;

        private List<Dot> dList = new List<Dot>();
        //private ElementControl ec { get; set}
        public DotGUIControl(Canvas canvas, Grid grid)
        {
            this.canvas = canvas;
            this.grid = grid;
        }
        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            string name = "";

            foreach (Dot d in dList)
            {
                if (img.Tag.ToString() == d.GetName())
                {
                    name = d.GetCore();
                    MessageBox.Show(name);
                    //wgc.DrawWireBetweenElements();
                }
            }            
        }

        private void Dot_MouseLeave(object sender, MouseEventArgs e)
        {
            /*Image img = sender as Image;
            foreach (Dot d in dList)
            {
                if (d.GetName() == img.Tag.ToString() && img != null)
                {
                    foreach (Dot d2 in ec.GetDots(d.GetCore()))
                    {
                        d2.GetDot().Visibility = Visibility.Hidden;
                    }
                }
            }*/
        }

        public void CreateDot(string name, ElementControl ec, int count)
        {
            bool direction = false;
            int oposite = 1;
            for (int i = 0; i < count; i++)
            {
                Image img = new Image();
                img.Visibility = Visibility.Hidden;
                img.Width = 15;
                img.Height = 15;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                img.Tag = name + "_" + i;

                img.MouseLeftButtonDown += new MouseButtonEventHandler(Dot_MouseLeftButtonDown);
                img.MouseLeave += new MouseEventHandler(Dot_MouseLeave);

                Panel.SetZIndex(img, 2);
                canvas.Children.Add(img);
                Dot d = new Dot(img.Tag.ToString(), name, img, direction, oposite);               

                ec.AddDot(name, d);
                dList.Add(d);

                direction = direction == true ? false : true;
                if (i < 1)
                {
                    oposite = 1;
                }
                else
                {
                    oposite = -1;
                }
            }
        }

        /*
        public void RecreateDot(SpecificElement se, int count)
        {
            bool direction = false;
            int oposite = 1;
            for (int i = 0; i < count; i++)
            {
                Image img = new Image();
                img.Visibility = Visibility.Hidden;
                img.Width = 15;
                img.Height = 15;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                img.Tag = se.GetDots()[i].GetName();

                img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown_2);
                img.MouseLeave += new MouseEventHandler(Image_MouseLeave_2);

                Panel.SetZIndex(img, 2);
                canvasGrid.Children.Add(img);
                Dot d = new Dot(img.Tag.ToString(), se.GetName(), img, direction, oposite);
                se.GetDots()[i].SetDot(img);
                dList.Add(d);
                direction = direction == true ? false : true;
                if (i < 1)
                {
                    oposite = 1;
                }
                else
                {
                    oposite = -1;
                }
            }
        }

        public Image FindDot(string dotName)
        {
            Image d = null;
            foreach (Dot d2 in dList)
            {
                if (d2.GetName() == dotName)
                {
                    d = d2.GetDot();
                }
            }
            return d;
        }
        public bool DetermineDirection(string dotName)
        {
            bool direction = false;
            foreach (Dot d in dList)
            {
                if (d.GetName() == dotName)
                {
                    direction = d.GetDirection();
                    break;
                }
            }
            return direction;
        }

        public int DetermineDirection2(string dotName)
        {
            int n = 0;
            foreach (Dot d in dList)
            {
                if (d.GetName() == dotName)
                {
                    n = d.GetOposite();

                }
            }
            return n;
        }
        */
        public void UpadateDotsLocation(Image draggableControl, ElementControl ec)
        {
            //List<Dot> dList = ec.GetDots(draggableControl.Tag.ToString());            

            double x = draggableControl.RenderTransform.Value.OffsetX+17;
            double y = draggableControl.RenderTransform.Value.OffsetY+17;           

            double distanceX = draggableControl.Width / 2;
            double distanceY = draggableControl.Height / 2;

            for (int i = 0; i < dList.Count; i++)
            {
                if (i <= 0)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x + distanceX, y);
                }
                else if (i <= 1)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x, y + distanceY);
                }
                else if (i <= 2)
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x - distanceX, y);
                }
                else
                {
                    dList[i].GetDot().RenderTransform = new TranslateTransform(x, y - distanceY);
                }
                dList[i].GetDot().Visibility = Visibility.Visible;
            }

        }
    }
}
