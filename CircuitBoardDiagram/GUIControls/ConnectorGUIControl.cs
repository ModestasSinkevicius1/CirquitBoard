using CircuitBoardDiagram.GUIControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram.GUIControls
{
    class ConnectorGUIControl
    {
        private Point clickPosition;

        private TranslateTransform originTT;

        private int queue = 0;

        protected bool isDragging;

        private Wire w;

        private MainWindow form;
        private Canvas canvas;
        private Grid grid;
        private MessageGUIControl mgc;
        private WireGUIControl wgc;
        private MenuGUIControl mngc;
        private ListContainer lc;

        public ConnectorGUIControl(MainWindow form, Canvas canvas, Grid grid, MessageGUIControl mgc, WireGUIControl wgc, MenuGUIControl mngc, ListContainer lc)
        {
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.mgc = mgc;
            this.wgc = wgc;
            this.mngc = mngc;
            this.lc = lc;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image draggableControl = sender as Image;

            if (!Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.X))
            {
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(form);
                draggableControl.CaptureMouse();
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                DeleteElement(draggableControl);
            }
            else
            {
                wgc.DrawWireBetweenElements(draggableControl, draggableControl.Tag.ToString(), lc.ec, lc.dList);
            }


        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();
            if (mngc.elementBehaviour != "neverGrid")
                SnapToClosestCell(draggable);

            lc.ec.UpdatePostitionValues(draggable.Tag.ToString());
            //wgc.FindWireConnectedDots(draggable.Tag.ToString());
            //hgc.IndicateCell(highlighting_rectangle);           
            //indicating_rectangle.Visibility = Visibility.Hidden;

            //ec.UpdatePostitionValues(draggable.Tag.ToString());
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Image draggableControl = sender as Image;

            if (isDragging && draggableControl != null)
            {
                Point currentPosition = e.GetPosition(form);
                TranslateTransform transform = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                transform.X = originTT.X + (currentPosition.X - clickPosition.X);
                transform.Y = originTT.Y + (currentPosition.Y - clickPosition.Y);
                draggableControl.RenderTransform = new TranslateTransform(transform.X, transform.Y);
                if(mngc.elementBehaviour == "alwaysGrid")
                    SnapToClosestCell(draggableControl);
                wgc.FindWireConnectedDots(draggableControl.Tag.ToString());
                //dgc.UpadateDotsLocation(draggableControl, lc.ec);
                //hgc.Highlight_cell(draggableControl);

                //wgc.FindWireConnectedDots(draggableControl.Tag.ToString());
            }

        }
        public Image CreateConnector()
        {
            Image img = new Image();

            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireConnectors/wire_connector.png"));
            img.Width = 10;
            img.Height = 10;
            img.Tag = "wire_connector" + queue;

            img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
            img.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
            img.MouseMove += new MouseEventHandler(Image_MouseMove);

            Panel.SetZIndex(img, 2);

            lc.ec.AddElementToList(img.Tag.ToString(), img);

            queue++;

            return img;
        }

        public void ConnectWireToConnector(Polyline pl, bool singleLine)
        {
            int space = 3;

            if (singleLine)
                space = 2;

            string[] nameABC = new string[space];
            string[] dotABC = new string[space];

            string name;
            string dot;

            wgc.turn = false;

            wgc.previousLine.Name = pl.Name.ToString();

            foreach (Wire w in lc.wList)
            {
                if (pl.Name == w.GetName())
                {
                    nameABC[0] = w.elementA;
                    dotABC[0] = w.dotA;

                    nameABC[1] = w.elementB;
                    dotABC[1] = w.dotB;
                }
            }
            if (!singleLine)
            {
                nameABC[2] = wgc.previousElementName;
                dotABC[2] = wgc.previousDotName;
            }

            Image img = CreateConnector();
            SnapToClosestCell(img);
            canvas.Children.Add(img);

            lc.ec.UpdatePostitionValues(img.Tag.ToString());

            wgc.DeleteWires(pl);
            if(!singleLine)
                canvas.Children.Remove(wgc.previousLine);

            for (int i = 0; i < space; i++)
            {
                name = nameABC[i];
                dot = dotABC[i];

                Polyline newPl = wgc.CreatePolyline();

                w = new Wire(dot + img.Tag.ToString());

                w.elementA = name;
                w.elementB = img.Tag.ToString();

                newPl.Name = dot + img.Tag.ToString();

                w.dotA = dot;
                w.dotB = img.Tag.ToString();

                w.AddPolyline(newPl);
                lc.wList.Add(w);

                lc.ec.AddLineForElement(name, newPl);
                lc.ec.AddLineForElement(img.Tag.ToString(), newPl);

                lc.ec.AddConnectionCountToSpecificElement(name);
                lc.ec.AddConnectionCountToSpecificElement(img.Tag.ToString());

                wgc.previousElementName = "";

                wgc.UpdateWireLocation(w.dotA, w.dotB, newPl);
            }
        }

        public void SnapToClosestCell(Image draggableControl)
        {
            double distanceX;
            double distanceY;

            double oldDistanceX = 9999;
            double oldDistanceY = 9999;

            double i = 0;
            double cellY = 0;
            double cellX = 0;

            double cellWidth;
            double cellHeight;

            int offCell = 0;

            double length = 50;

            double widthLength = grid.ColumnDefinitions[0].Width.Value;
            double heightLength = grid.RowDefinitions[0].Height.Value;

            if (widthLength < length)
            {
                if (widthLength < 25)
                {
                    if (widthLength < 12.5)
                    {
                        if (widthLength < 6.25)
                            if (widthLength < 3.125)
                            {
                                offCell = 7;
                            }
                            else
                                offCell = 15;
                        else
                            offCell = 7;
                    }
                    else
                        offCell = 3;
                }
                else
                    offCell = 1;
            }

            foreach (ColumnDefinition column in grid.ColumnDefinitions)
            {
                distanceX = (Math.Abs((Mouse.GetPosition(canvas).X - (column.Width.Value / 2)) - (column.Width.Value * i))) - column.Width.Value / 2;

                if (oldDistanceX > distanceX && i < grid.ColumnDefinitions.Count - offCell)
                {
                    oldDistanceX = distanceX;
                    cellX = i;
                }
                i++;
            }

            i = 0;
            length = 50;
            offCell = 0;

            if (heightLength < length)
            {
                if (heightLength < 25)
                {
                    if (heightLength < 12.5)
                    {
                        if (heightLength < 6.25)
                            if (heightLength < 3.125)
                            {
                                offCell = 7;
                            }
                            else
                                offCell = 15;
                        else
                            offCell = 7;
                    }
                    else
                        offCell = 3;
                }
                else
                    offCell = 1;
            }

            foreach (RowDefinition row in grid.RowDefinitions)
            {
                distanceY = (Math.Abs((Mouse.GetPosition(canvas).Y - (row.Height.Value / 2)) - (row.Height.Value * i))) - row.Height.Value / 2;

                if (oldDistanceY > distanceY && i < grid.RowDefinitions.Count - offCell)
                {
                    oldDistanceY = distanceY;
                    cellY = i;
                }
                i++;
            }

            cellWidth = grid.ColumnDefinitions[(int)cellX].Width.Value;
            cellHeight = grid.RowDefinitions[(int)cellY].Height.Value;

            Canvas.SetLeft(draggableControl, 0);
            Canvas.SetTop(draggableControl, 0);

            draggableControl.RenderTransform = new TranslateTransform((cellWidth * cellX) + 19, (cellHeight * cellY) + 19);

            //Grid.SetRow(draggableControl, 0);
            //Grid.SetColumn(draggableControl, 0);
        }
        public void DeleteElement(Image draggableControl)
        {
            foreach (Polyline pl in lc.ec.GetLineListFromElement(draggableControl.Tag.ToString()))
            {
                wgc.DeleteWires(pl);
            }
            foreach (Dot d in lc.ec.GetDots(draggableControl.Tag.ToString()))
            {
                canvas.Children.Remove(d.GetDot());
            }
            lc.ec.RemoveElementFromList(draggableControl.Tag.ToString());
            canvas.Children.Remove(draggableControl);
        }

        public void RecreateElementsFromSave(ListContainer lc)
        {
            this.lc = lc;           

            foreach (SpecificElement se in lc.ec.GetAllElements())
            {
                if (RemoveNumbers(se.GetName()) == "wire_connector")
                {
                    Image img = new Image();

                    img.Height = 10;
                    img.Width = 10;
                    img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "WireConnectors/" + RemoveNumbers(se.GetName()) + ".png"));
                    img.Tag = se.GetName();

                    img.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
                    img.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
                    img.MouseMove += new MouseEventHandler(Image_MouseMove);

                    canvas.Children.Add(img);

                    se.SetImage(img);

                    Canvas.SetTop(img, 0);
                    Canvas.SetLeft(img, 0);

                    img.RenderTransform = new TranslateTransform(se.GetPositionX(), se.GetPositionY());

                    queue++;

                    Panel.SetZIndex(img, 1);
                }
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
