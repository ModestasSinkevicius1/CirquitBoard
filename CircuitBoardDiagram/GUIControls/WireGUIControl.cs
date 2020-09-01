using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram.GUIControls
{
    class WireGUIControl
    {
        /*public Polyline CreatePolyline(bool seperateLine)
        {
            Polyline pl = new Polyline();
            SolidColorBrush bc = new SolidColorBrush();
            bc.Color = Colors.Black;

            pl.StrokeThickness = 2;
            pl.Stroke = bc;

            if (seperateLine)
            {
                pl.MouseLeftButtonDown += new MouseButtonEventHandler(Polyline_mouseLeftButtonDown);
                pl.MouseEnter += new MouseEventHandler(Polyline_mouseEnter);
                pl.MouseLeave += new MouseEventHandler(Polyline_mouseLeave);
                pl.MouseMove += new MouseEventHandler(Polyline_mouseMove);
                pl.MouseLeftButtonUp += new MouseButtonEventHandler(Polyline_mouseLeftButtonUp);
            }

            canvas.Children.Add(pl);

            return pl;
        }
        public void UpdateWireLocation(string dotNameA, string dotNameB, Polyline pl)
        {
            Point startLine1;
            Point startline2;
            Point line1;
            Point line2;
            Point endLine2;
            Point endLine1;

            PointCollection polylinePoints = new PointCollection();

            Image dotA = FindDot(dotNameA);
            Image dotB = FindDot(dotNameB);

            bool direction = DetermineDirection(dotNameA);
            int n = DetermineDirection2(dotNameA);

            startLine1 = new Point(dotA.RenderTransform.Value.OffsetX + 25, dotA.RenderTransform.Value.OffsetY + 25);
            endLine1 = new Point(dotB.RenderTransform.Value.OffsetX + 25, dotB.RenderTransform.Value.OffsetY + 25);

            if (!direction)
            {
                startline2 = new Point(startLine1.X + 25 * n, startLine1.Y);
            }
            else
            {
                startline2 = new Point(startLine1.X, startLine1.Y + 25 * n);
            }

            direction = DetermineDirection(dotNameB);
            n = DetermineDirection2(dotNameB);

            if (!direction)
            {
                endLine2 = new Point(endLine1.X + 25 * n, endLine1.Y);
            }
            else
            {
                endLine2 = new Point(endLine1.X, endLine1.Y + 25 * n);
            }

            direction = DetermineDirection(dotNameA);

            if (!direction)
            {
                line1 = new Point(startline2.X, endLine2.Y);
                line2 = new Point(endLine2.X, endLine2.Y);
            }
            else
            {
                line1 = new Point(endLine2.X, startline2.Y);
                line2 = new Point(endLine2.X, endLine2.Y);
            }


            polylinePoints.Add(startLine1);
            polylinePoints.Add(startline2);
            polylinePoints.Add(line1);
            polylinePoints.Add(line2);
            polylinePoints.Add(endLine2);
            polylinePoints.Add(endLine1);

            pl.Points = polylinePoints;
        }
        public void DeleteWires(Polyline l)
        {
            foreach (Wire w2 in wList)
            {
                if (w2.GetName() == l.Name)
                {
                    foreach (Dot d in dList)
                    {
                        if (w2.dotA == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                        }
                        if (w2.dotB == d.GetName())
                        {
                            d.GetDot().Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireDots/dotGreen.png"));
                        }
                    }

                    canvas.Children.Remove(w2.GetPolyline());
                    ec.RemoveConnectionCountFromSpecificElement(w2.elementA);
                    ec.RemoveConnectionCountFromSpecificElement(w2.elementB);

                    ec.EnableConnectionAvailability(w2.elementA);
                    ec.EnableConnectionAvailability(w2.elementB);

                    wList.Remove(w2);
                    break;
                }
            }
        }
        public void ChangeLineStyle(Polyline pl, SolidColorBrush bc, double thickness)
        {
            bool singleLine = true;
            foreach (Wire w2 in wList)
            {
                if (w2.GetName() == pl.Name)
                {

                    pl.Stroke = bc;
                    pl.StrokeThickness = thickness;
                    singleLine = false;
                    break;
                }
            }
            if (singleLine)
            {
                pl.Stroke = bc;
                pl.StrokeThickness = thickness;
            }
        }

        public void DrawWireBetweenElements()
        {

        }
        */
    }
}
