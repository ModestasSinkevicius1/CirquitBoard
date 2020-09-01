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
        /*public void CreateDot(string name, int count)
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

                img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown_2);
                img.MouseLeave += new MouseEventHandler(Image_MouseLeave_2);

                Panel.SetZIndex(img, 2);
                canvasGrid.Children.Add(img);
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

        public void UpadateDotsLocation(Image draggableControl)
        {
            List<Dot> dList = ec.GetDots(draggableControl.Tag.ToString());

            double x = draggableControl.RenderTransform.Value.OffsetX;
            double y = draggableControl.RenderTransform.Value.OffsetY;

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

        }*/
    }
}
