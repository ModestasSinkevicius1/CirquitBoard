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

namespace CircuitBoardDiagram.GUIControls
{
    class ListImageGUIControl
    {
        public string currentImageName { get; set; }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            currentImageName = img.Tag.ToString();            
        }

        public void LoadImages(Grid grid_expander)
        {
            DatabaseControl dc = new DatabaseControl();

            Image img;

            int i = 0;
            int y = 0;

            foreach (DatabaseElement e in dc.GetElements())
            {
                if (i > 7)
                {
                    i = 0;
                    y++;
                }
                img = new Image();
                img.Width = 22;
                img.Height = 22;
                img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + e.GetElementType() + ".png"));
                img.Tag = e.GetElementType();
                img.Stretch = Stretch.Fill;

                img.MouseLeftButtonDown += new MouseButtonEventHandler(image_MouseLeftButtonDown);

                Grid.SetColumn(img, i);
                Grid.SetRow(img, y);
                grid_expander.Children.Add(img);
                i++;
            }
        }
        /*
        public void AddImageToCommon(string imageName)
        {
            int i = 0;
            bool allowAdd = true;

            if (next > 5)
                next = 0;

            foreach (Image img in dock_bottom.Children)
            {
                if (img.Tag != null)
                    if (img.Tag.ToString() == imageName)
                    {
                        allowAdd = false;
                    }
            }

            if (allowAdd)
            {
                foreach (Image img in dock_bottom.Children)
                {
                    if (img.Tag == null)
                    {
                        img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + imageName + ".png"));
                        img.Tag = imageName;
                        break;
                    }
                    else if (i > 4)
                    {
                        Image img2 = dock_bottom.Children[next] as Image;
                        img2.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + imageName + ".png"));
                        img2.Tag = imageName;
                        next++;
                    }
                    i++;
                }
            }
        }*/
    }
}
