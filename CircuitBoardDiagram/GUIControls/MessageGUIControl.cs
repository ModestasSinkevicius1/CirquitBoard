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
    class MessageGUIControl
    {
        private TextBlock tb = new TextBlock();
        private TextBlock wtb = new TextBlock();
        private TextBlock stb = new TextBlock();

        private Canvas canvas;
        private ListContainer lc;
        public MessageGUIControl(Canvas canvas, ListContainer lc)
        {
            this.canvas = canvas;
            this.lc = lc;

            LoadPopupMessage();
            LoadWarningMessage();
            LoadStatusMessage();
        }

        public void UpdateContainer(ListContainer lc)
        {
            this.lc = lc;
        }

        private void Textbox_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock t = sender as TextBlock;
            t.Visibility = Visibility.Hidden;
        }
        public void LoadPopupMessage()
        {
            tb.Visibility = Visibility.Hidden;
            SolidColorBrush scb = Brushes.LightBlue;

            tb.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);

            tb.Width = 130;
            tb.Height = 100;
            tb.Background = scb;
            tb.Opacity = 0.90;
            Panel.SetZIndex(tb, 3);
            canvas.Children.Add(tb);
        }

        public void LoadWarningMessage()
        {
            wtb.Visibility = Visibility.Hidden;
            SolidColorBrush scb = Brushes.IndianRed;

            wtb.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);

            wtb.Width = 130;
            wtb.Height = 60;
            wtb.Background = scb;
            wtb.Opacity = 0.90;
            Panel.SetZIndex(wtb, 3);
            canvas.Children.Add(wtb);
        }

        public void LoadStatusMessage()
        {
            stb.Visibility = Visibility.Hidden;
            SolidColorBrush scb = Brushes.Yellow;

            //wtb.MouseLeave += new MouseEventHandler(Textbox_MouseLeave);

            stb.Width = 60;
            stb.Height = 20;
            stb.Background = scb;
            stb.Opacity = 0.90;
            Panel.SetZIndex(stb, 3);
            canvas.Children.Add(stb);
        }

        public void ShowPopupMessage(Image draggableControl)
        {
            tb.Visibility = Visibility.Visible;
            tb.Text = draggableControl.Tag.ToString() + "\n";
            tb.Text += "Connections: " + lc.ec.GetConnectionCount(draggableControl.Tag.ToString()) + "\n";
            tb.Text += "Connection state: " + lc.ec.GetConnectionAvailability(draggableControl.Tag.ToString()) + "\n";
            tb.RenderTransform = draggableControl.RenderTransform;
        }
        
        public void ShowWarningMessage(Image draggableControl, string message)
        {
            wtb.Visibility = Visibility.Visible;
            wtb.Text = "";

            string[] word=message.Split(' ');
            int row = 0;
            foreach(string w in word)
            {
                wtb.Text += w + " ";
                if(row>2)
                {
                    wtb.Text += "\n";
                    row = 0;
                }
                row++;
            }                          
            wtb.RenderTransform = draggableControl.RenderTransform;
        }

        public void ShowStatusBox(Image draggableControl, double value)
        {           
            if (RemoveNumbers(draggableControl.Tag.ToString()) == "AC")
            {
                stb.Background = Brushes.DarkBlue;
                stb.Foreground = Brushes.White;
            }
            else
            {
                stb.Background = Brushes.Yellow;
                stb.Foreground = Brushes.Black;
            }
            stb.Visibility = Visibility.Visible;
           
            stb.RenderTransform = draggableControl.RenderTransform;

            stb.RenderTransform = new TranslateTransform(draggableControl.RenderTransform.Value.OffsetX+-5, draggableControl.RenderTransform.Value.OffsetY+-25);

            stb.HorizontalAlignment = HorizontalAlignment.Center;

            stb.TextAlignment = TextAlignment.Center;            

            stb.Text = value.ToString()+'V';
        }

        public void HideBox(string type)
        {
            if (type == "status")
            {
                stb.Visibility = Visibility.Hidden;
            }
            else if (type == "warning")
            {
                wtb.Visibility = Visibility.Hidden;
            }
            else if(type =="popup")
            {
                tb.Visibility = Visibility.Hidden;
            }
        }
        private string RemoveNumbers(string name)
        {
            foreach (char w in name)
            {
                if (Char.IsNumber(w))
                {
                    name = name.Remove(name.Length - 1);
                }
            }

            return name;
        }
    }
}
