﻿using CircuitBoardDiagram.GUIControls;
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
        private MenuGUIControl mngc;
        private ListContainer lc;
        private ShortcutGUIControl sgc;

        private Point startPosition;
        private Image tmpImage;
        public string modeTool { get; set; } = "";

        public ImageGUIControl(MainWindow form, Canvas canvas, Grid grid, DotGUIControl dgc, HighlighterGUIControl hgc, WireGUIControl wgc, MessageGUIControl mgc, MenuGUIControl mngc, ListContainer lc, ShortcutGUIControl sgc)
        {            
            this.form = form;
            this.canvas = canvas;
            this.grid = grid;
            this.dgc = dgc;
            this.hgc = hgc;
            this.wgc = wgc;
            this.mgc = mgc;
            this.mngc = mngc;
            this.lc = lc;
            this.sgc = sgc;
        }       

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image draggableControl = sender as Image;           
            if (!Keyboard.IsKeyDown(Key.W) && !Keyboard.IsKeyDown(Key.C) && !Keyboard.IsKeyDown(Key.X) && modeTool != "delete")
            {
                dgc.UpadateDotsLocation(draggableControl, lc.ec);
                //highlighting_rectangle.Visibility = Visibility.Hidden;
                if (mngc.elementBehaviour != "neverGrid")
                    SnapToClosestCell(draggableControl);
                originTT = draggableControl.RenderTransform as TranslateTransform ?? new TranslateTransform();
                isDragging = true;
                clickPosition = e.GetPosition(form);
                draggableControl.CaptureMouse();
                                
                startPosition = Mouse.GetPosition(form);
                dgc.BeginHide(startPosition, lc.ec.GetDots(draggableControl.Tag.ToString()));
            }
            if (Keyboard.IsKeyDown(Key.X) || modeTool == "delete")
            {
                if (!wgc.turn)
                {
                    DeleteElement(draggableControl);
                }
                else
                    mgc.ShowWarningMessage(draggableControl, "Wiring process \n detected!");
            }
            if (Keyboard.IsKeyDown(Key.C) || modeTool == "info")
            {
                mgc.ShowPopupMessage(draggableControl);
            }


        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            Image draggable = sender as Image;
            draggable.ReleaseMouseCapture();
            if (mngc.elementBehaviour != "neverGrid")
                SnapToClosestCell(draggable);
            else
            {
                Canvas.SetLeft(draggable, 0);
                Canvas.SetTop(draggable, 0);
                draggable.RenderTransform = new TranslateTransform(Mouse.GetPosition(canvas).X - 25, Mouse.GetPosition(canvas).Y - 25);
            }
            hgc.Highlight_cell(draggable);
            dgc.UpadateDotsLocation(draggable, lc.ec);
            lc.ec.UpdatePostitionValues(draggable.Tag.ToString());
            wgc.FindWireConnectedDots(draggable.Tag.ToString());
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
                if(mngc.elementBehaviour=="alwaysGrid")
                    SnapToClosestCell(draggableControl);
                dgc.UpadateDotsLocation(draggableControl, lc.ec);
                hgc.Highlight_cell(draggableControl);                

                wgc.FindWireConnectedDots(draggableControl.Tag.ToString());
                foreach(SpecificElement se in lc.ec.GetAllElements())
                {
                    if(se.GetName()==draggableControl.Tag.ToString())
                    {
                        mgc.ShowStatusBox(draggableControl, se.voltage);
                        break;
                    }
                }               
            }

        }
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //isOnImage = true;
            sgc.UpdateText("OnControl");
            Image draggableControl = sender as Image;
            hgc.Highlight_cell(draggableControl);

            SpecificElement se = null;

            foreach(SpecificElement se2 in lc.ec.GetAllElements())
            {
                if(se2.GetName()==draggableControl.Tag.ToString())
                {
                    se = se2;
                    break;
                }
            }

            if (!wgc.turn && se != null)
                mgc.ShowStatusBox(draggableControl, se.voltage);
            hgc.RemoveCheckCircuitBox();
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            sgc.UpdateText("OnCanvas");
            Image img = sender as Image;
            hgc.highlighter.Visibility = Visibility.Hidden;          
            mgc.HideBox("status");
            startPosition = Mouse.GetPosition(form);
            dgc.BeginHide(startPosition, lc.ec.GetDots(img.Tag.ToString()));
        }

        public void AddContextHandler(Image r)
        {
            ContextMenu newContext = new ContextMenu();

            MenuItem newItem1 = new MenuItem();
            newItem1.Click += new RoutedEventHandler(ContextProperties_Click);
            newItem1.Header = "Properties";

            MenuItem newItem2 = new MenuItem();
            newItem2.Click += new RoutedEventHandler(ContextDelete_Click);
            newItem2.Header = "Delete";

            r.ContextMenu = newContext;
            r.ContextMenu.Items.Add(newItem1);
            r.ContextMenu.Items.Add(newItem2);
        }

        public void CreateElement(string currentImageName)
        {
            mngc.isProgress = true;

            Image r = new Image();
            if (currentImageName != "transformer")
            {
                r.Height = 50;
                r.Width = 50;
            }
            else
            {
                r.Height = 100;
                r.Width = 100;
            }
            r.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + currentImageName + ".png"));
            r.Tag = currentImageName;           

            r.Tag = currentImageName + queue;

            elementName = r.Tag.ToString();

            r.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
            r.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
            r.MouseMove += new MouseEventHandler(Image_MouseMove);
            r.MouseEnter += new MouseEventHandler(Image_MouseEnter);
            r.MouseLeave += new MouseEventHandler(Image_MouseLeave);

            AddContextHandler(r);

            canvas.Children.Add(r);

            lc.ec.AddElementToList(r.Tag.ToString(), r);

            tmpImage = r;

            Canvas.SetTop(r, Mouse.GetPosition(canvas).Y - r.Width / 2);
            Canvas.SetLeft(r, Mouse.GetPosition(canvas).X - r.Height / 2);            

            Panel.SetZIndex(r, 1);            
            queue++;
        }               

        private void ContextDelete_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            ContextMenu cm = item.Parent as ContextMenu;

            Image img = cm.PlacementTarget as Image;

            DeleteElement(img);
        }

        private void ContextProperties_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            ContextMenu cm = item.Parent as ContextMenu;

            Image img = cm.PlacementTarget as Image;

            SpecificElement se = null;

            foreach(SpecificElement se2 in lc.ec.GetAllElements())
            {
                if(se2.GetName() == img.Tag.ToString())
                {
                    se = se2;
                    break;
                }
            }
            if (se != null)
            {
                Element_features ef = new Element_features(se);
                ef.Show();
            }
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
            foreach(SpecificElement se in lc.ec.GetAllElements())
            {
                lc.ec.RemoveChildElementFromElement(se.GetName() ,draggableControl.Tag.ToString());
            }
            lc.ec.RemoveElementFromList(draggableControl.Tag.ToString());
            canvas.Children.Remove(draggableControl);
        }
        
        public void UpdateImage()
        {                              
            SnapToClosestCell(tmpImage);
            lc.ec.UpdatePostitionValues(tmpImage.Tag.ToString());
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

            draggableControl.RenderTransform = new TranslateTransform(cellWidth * cellX, cellHeight * cellY);

            //Grid.SetRow(draggableControl, 0);
            //Grid.SetColumn(draggableControl, 0);
        }
        public void RecreateElementsFromSave(ListContainer lc)
        {
            mngc.isProgress = true;

            this.lc = lc;           
            this.lc.dList.Clear();
            
            foreach (SpecificElement se in lc.ec.GetAllElements())
            {
                if (RemoveNumbers(se.GetName()) != "wire_connector")
                {
                    Image r = new Image();

                    r.Height = 50;
                    r.Width = 50;
                    r.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Circuit element images/" + RemoveNumbers(se.GetName()) + ".png"));
                    r.Tag = se.GetName();

                    r.MouseLeftButtonDown += new MouseButtonEventHandler(Image_MouseLeftButtonDown);
                    r.MouseLeftButtonUp += new MouseButtonEventHandler(Image_MouseLeftButtonUp);
                    r.MouseMove += new MouseEventHandler(Image_MouseMove);
                    r.MouseEnter += new MouseEventHandler(Image_MouseEnter);
                    r.MouseLeave += new MouseEventHandler(Image_MouseLeave);

                    AddContextHandler(r);
                    
                    canvas.Children.Add(r);

                    se.SetImage(r);

                    Canvas.SetTop(r, 0);
                    Canvas.SetLeft(r, 0);

                    r.RenderTransform = new TranslateTransform(se.GetPositionX(), se.GetPositionY());

                    queue++;

                    Panel.SetZIndex(r, 1);
                    
                    dgc.RecreateDot(se.GetName(), 4, this.lc);
                    dgc.UpadateDotsLocation(se.GetElement(), this.lc.ec);

                    foreach (Dot d in this.lc.dList)
                    {
                        d.GetDot().Visibility = Visibility.Hidden;
                    }                
                }
            }
        }       

        private string RemoveNumbers(string name)
        {            
            foreach(char w in name)
            {
                if(Char.IsNumber(w))
                {                    
                    name = name.Remove(name.Length - 1);
                }               
            }

            return name;
        }

        public void ResetQueue()
        {
            queue = 0;
        }
    }
}
