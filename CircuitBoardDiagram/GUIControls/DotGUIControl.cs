using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CircuitBoardDiagram
{
    class DotGUIControl
    {
        private MainWindow form;
        
        private Canvas canvas;
        private Grid grid;
        
        private ListContainer lc;

        private WireGUIControl wgc;

        private DispatcherTimer t1;
        private int timer=5;

        private List<Dot> tmpList;        
        public DotGUIControl(MainWindow form, Canvas canvas, Grid grid, WireGUIControl wgc, ListContainer lc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.wgc = wgc;
            this.lc = lc;
        }

        public void BeginHide(Point startPosition, List<Dot> tmpList)
        {
            this.tmpList = tmpList;

            Thread th = new Thread(new ParameterizedThreadStart(UpdateCurrentDotVisibilityStatus));           
            th.IsBackground = true;
            th.Start(startPosition);           
            
            /*foreach(Dot d in tmpList)
            {
                d.GetDot().Visibility = Visibility.Hidden;
            }
            */
        }
        public void UpdateCurrentDotVisibilityStatus(object obj)
        {
            Point startPostition = (Point)obj;
            
            double distance = 50;
            while (distance<100)
            {
                Thread.Sleep(50);
                form.Dispatcher.BeginInvoke(new Action(() =>
                {
                    distance = CalculateDistance(startPostition,Mouse.GetPosition(form));                    
                }));
            }
            form.Dispatcher.Invoke(() =>
            {
                if (tmpList != null)
                {
                    foreach (Dot d in tmpList)
                    {
                        d.GetDot().Visibility = Visibility.Hidden;
                    }
                }
            });
            
        }

        private double CalculateDistance(Point a, Point b)
        {
            double result;           

            result = Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

            return result;
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            string name = "";            
            
            foreach (Dot d in lc.dList)
            {               
                if (img.Tag.ToString() == d.GetName())
                {
                    name = d.GetCore();                   
                    wgc.DrawWireBetweenElements(img, name, lc.ec, lc.dList);
                }
            }            
        }

        private void Dot_MouseLeave(object sender, MouseEventArgs e)
        {
            
        }

        public void CreateDot(string name, int count)
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

                lc.ec.AddDot(name, d);
                lc.dList.Add(d);

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
        */
        public void UpadateDotsLocation(Image draggableControl, ElementControl ec)
        {
            List<Dot> dList = ec.GetDots(draggableControl.Tag.ToString());

            //MessageBox.Show(dList.Count.ToString());

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
