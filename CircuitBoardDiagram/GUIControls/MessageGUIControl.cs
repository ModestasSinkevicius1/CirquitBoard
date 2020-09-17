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

        private Canvas canvas;
        private ListContainer lc;
        public MessageGUIControl(Canvas canvas, ListContainer lc)
        {
            this.canvas = canvas;
            this.lc = lc;

            LoadPopupMessage();
            LoadWarningMessage();
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
    }
}
