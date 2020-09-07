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

namespace CircuitBoardDiagram.GUIControls
{
    class ImageGUIControl
    {
        private int queue = 0;

        protected bool isDragging;

        private string elementBehaviour = "alwaysGrid";
        public string elementName { get; set; } = "";

        private Point clickPosition;
        private TranslateTransform originTT;
        private Canvas canvas;
        private Grid grid;
        private MainWindow form;
        private Rectangle highlighter;

        private DotGUIControl dgc;
        private HighlighterGUIControl hgc;
        private WireGUIControl wgc;
        private MessageGUIControl mgc;
        private ListContainer lc;
        public ImageGUIControl(MainWindow form, Canvas canvas, Grid grid, DotGUIControl dgc, HighlighterGUIControl hgc, WireGUIControl wgc, MessageGUIControl mgc, ListContainer lc)
        {            
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.dgc = dgc;
            this.hgc = hgc;
            this.wgc = wgc;
            this.mgc = mgc;
            this.lc = lc;            
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image draggableControl = sender as Image;
            if (!Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.C) && !Keyboard.IsKeyDown(Key.X))
            {
                dgc.UpadateDotsLocation(draggableControl, lc.ec);
                //highlighting_rectangle.Visibility = Visibility.Hidden;
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(form);
                draggableControl.CaptureMouse();               
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                DeleteElement(draggableControl);
            }
            else if (Keyboard.IsKeyDown(Key.C))
            {
                mgc.ShowPopupMessage(draggableControl);
            }


        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();
            //if(elementBehaviour!="neverGrid")
            SnapToClosestCell(draggable);
            hgc.Highlight_cell(draggable);                      
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
                //if(elementBehaviour=="alwaysGrid")
                SnapToClosestCell(draggableControl);
                dgc.UpadateDotsLocation(draggableControl, lc.ec);
                hgc.Highlight_cell(draggableControl);
                //ec.UpdatePostitionValues(draggableControl.Tag.ToString());
                wgc.FindWireConnectedDots(draggableControl.Tag.ToString());
            }

        }
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //isOnImage = true;
            Image draggableControl = sender as Image;
            hgc.Highlight_cell(draggableControl);
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            hgc.highlighter.Visibility = Visibility.Hidden;
        }

        public void CreateElement(string currentImageName)
        {
            Image r = new Image();
            r.Height = 50;
            r.Width = 50;
            r.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + currentImageName + ".png"));
            r.Tag = currentImageName;           

            r.Tag = currentImageName + queue;

            elementName = r.Tag.ToString();

            r.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
            r.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
            r.MouseMove += new MouseEventHandler(Image_MouseMove);
            r.MouseEnter += new MouseEventHandler(Image_MouseEnter);
            r.MouseLeave += new MouseEventHandler(Image_MouseLeave);

            canvas.Children.Add(r);

            lc.ec.AddElementToList(r.Tag.ToString(), r);

            Canvas.SetTop(r, Mouse.GetPosition(canvas).Y - r.Width / 2);
            Canvas.SetLeft(r, Mouse.GetPosition(canvas).X - r.Height / 2);

            Panel.SetZIndex(r, 1);            
            queue++;
        }
        
        public void DeleteElement(Image draggableControl)
        {
            foreach (Polyline pl in lc.ec.GetLineListFromElement(draggableControl.Tag.ToString()))
            {
                wgc.DeleteWires(pl);
            }
            foreach (Dot d in lc.ec.GetDots(draggableControl.Tag.ToString()))
            {
                grid.Children.Remove(d.GetDot());
            }
            lc.ec.RemoveElementFromList(draggableControl.Tag.ToString());
            canvas.Children.Remove(draggableControl);
        }

        /*
        public void ResizeBasedElementsArangements()
        {
            maxX = -99999;
            minX = 99999;

            maxY = -99999;
            minY = 99999;


            foreach (SpecificElement se in ec.GetAllElements())
            {
                if (minX > se.GetElement().RenderTransform.Value.OffsetX)
                {
                    minX = se.GetElement().RenderTransform.Value.OffsetX;
                }

                if (maxX < se.GetElement().RenderTransform.Value.OffsetX)
                {
                    maxX = se.GetElement().RenderTransform.Value.OffsetX;
                }

                if (minY > se.GetElement().RenderTransform.Value.OffsetY)
                {
                    minY = se.GetElement().RenderTransform.Value.OffsetY;
                }

                if (maxY < se.GetElement().RenderTransform.Value.OffsetY)
                {
                    maxY = se.GetElement().RenderTransform.Value.OffsetY;
                }
            }
        }
        */
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

            draggableControl.RenderTransform = new TranslateTransform(cellWidth * cellX, cellHeight * cellY);

            //Grid.SetRow(draggableControl, 0);
            //Grid.SetColumn(draggableControl, 0);
        }
        /*
        public void RecreateElementsFromSave()
        {
            foreach (SpecificElement se in ec.GetAllElements())
            {
                Image r = new Image();
                r.Height = 50;
                r.Width = 50;
                r.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + se.GetName().Remove(se.GetName().Length - 1) + ".png"));
                r.Tag = se.GetName();
                //AddImageToCommon(r.Tag.ToString());               

                r.MouseLeftButtonDown += new MouseButtonEventHandler(Canvas_MouseLeftButtonDown);
                r.MouseLeftButtonUp += new MouseButtonEventHandler(Canvas_MouseLeftButtonUp);
                r.MouseMove += new MouseEventHandler(Canvas_MouseMove);
                r.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                r.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                canvas.Children.Add(r);

                se.SetImage(r);

                Canvas.SetTop(r, se.GetPositionX());
                Canvas.SetLeft(r, se.GetPositionY());

                Panel.SetZIndex(r, 1);

                RecreateDot(se, 4);
            }
        }
        */
    }
}
