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
        private int next = 0;
        public string currentImageName { get; set; }
        public HighlighterGUIControl hgc { get; set; }
        public Grid grid_expander { get; set; }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
            Image img = sender as Image;           

            currentImageName = img.Tag.ToString();

            HighlightSelectedElement(img);
        }

        public void LoadImages()
        {
            DatabaseControl dc = new DatabaseControl();

            Image img;

            int i = 0;
            int y = 0;

            foreach (DatabaseElement e in dc.GetElements())
            {
                if (i > 6)
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


                Border b = new Border();
                b.BorderBrush = Brushes.Black;               
                b.Child = img;


                Grid.SetColumn(b, i);
                Grid.SetRow(b, y);

                grid_expander.Children.Add(b);                

                i++;
            }
        }
        
        private void CommonImage_MouseDown(object sender, MouseButtonEventArgs e)
        {           
            Image img = sender as Image;
            currentImageName = img.Tag.ToString();            
        }

        public void AddImageToCommon(string imageName, Panel dock_bottom)
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
                        img.MouseDown += new MouseButtonEventHandler(CommonImage_MouseDown);
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
        }

        public void HighlightSelectedElement(Image img)
        {
            DeselectOtherElements(img);

            Border b = img.Parent as Border;

            b.BorderThickness = new Thickness(1);
        }

        public void DeselectOtherElements(Image img)
        {
            foreach(Border b in grid_expander.Children)
            {
                if(b.Child != img)
                {
                    b.BorderThickness = new Thickness(0);
                }
            }
        }
    }
}
