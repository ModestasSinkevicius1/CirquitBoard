using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CircuitBoardDiagram
{
    /// <summary>
    /// Interaction logic for OptionWindow.xaml
    /// </summary>
    public partial class OptionWindow : Window
    {
        private List<RowDefinition> rList = new List<RowDefinition>();
        private List<ColumnDefinition> cList = new List<ColumnDefinition>();
        private Window w;

        public bool isPressedOk = false;
        private string elementBehaviour = "alwaysGrid";
        private string wireBehaviour = "alwaysGrid";
        private bool isGrid = true;

        public OptionWindow(double column, double row, string elementBehaviour, string wireBehaviour, bool isGrid)
        {            
            InitializeComponent();
            AddRow();
            AddColumn();
            ChangeSliderValue(column, row);
            ChangeBehaviour(elementBehaviour, wireBehaviour);
            ChangeShowGrid(isGrid);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (grid_preview != null)
            {
                foreach (ColumnDefinition column in grid_preview.ColumnDefinitions)
                {
                    column.Width = new GridLength(slider.Value);
                }
                //RemoveColumns();                
            }
        }

        private void slider_Copy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (grid_preview != null)
            {
                foreach (RowDefinition row in grid_preview.RowDefinitions)
                {
                    row.Height = new GridLength(slider_Copy.Value);
                }
                //RemoveRows();                
            }
        }

        private void ChangeBehaviour(string elementBehaviour, string wireBehaviour)
        {
            if (elementBehaviour == "alwaysGrid")
                radioButton.IsChecked = true;
            else if (elementBehaviour == "endGrid")
                radioButton_Copy.IsChecked = true;
            else if (elementBehaviour == "neverGrid")
                radioButton_Copy1.IsChecked = true;

            if (wireBehaviour == "alwaysGrid")
                radioButton1.IsChecked = true;
            else if (wireBehaviour == "endGrid")
                radioButton1_Copy.IsChecked = true;
            else if (wireBehaviour == "neverGrid")
                radioButton1_Copy1.IsChecked = true;
        }

        private void ChangeShowGrid(bool isGrid)
        {
            if (isGrid)
                checkBox.IsChecked = true;
            else
                checkBox.IsChecked = false;
        }

        private void ChangeSliderValue(double column, double row)
        {
            slider.Value = column;
            slider_Copy.Value = row;
        }

        private void AddColumn()
        {
            double originalValue = 50;
            int i = 0;

            while (i < 32)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(slider.Value);
                grid_preview.ColumnDefinitions.Add(c);

                i++;
            }                    
        }

        private void AddRow()
        {         
            double originalValue = 50;
            int i = 0;

            while (i<32)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(slider_Copy.Value);
                grid_preview.RowDefinitions.Add(r);

                i++;
            }                      
        }

        private void RemoveColumns()
        {
            foreach(ColumnDefinition c in cList)
            {
                grid_preview.ColumnDefinitions.Remove(c);              
            }      
            rList.Clear();
        }

        private void RemoveRows()
        {            
            foreach (RowDefinition r in rList)
            {
                grid_preview.RowDefinitions.Remove(r);               
            }
            cList.Clear();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            isPressedOk = true;
            Close();
        }

        public double GetColumn()
        {
            return slider.Value;
        }

        public double GetRow()
        {
            return slider_Copy.Value;
        }

        public string GetElementBehaviour()
        {
            return elementBehaviour;
        }

        public string GetWireBehaviour()
        {
            return wireBehaviour;
        }

        public bool GetIsShowGrid()
        {
            return checkBox.IsChecked.Value;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if(radioButton.IsChecked==true)
            {
                elementBehaviour = "alwaysGrid";               
            }
        }

        private void radioButton_Checked2(object sender, RoutedEventArgs e)
        {
            if (radioButton_Copy.IsChecked == true)
            {
                elementBehaviour = "endGrid";
            }
        }

        private void radioButton_Checked3(object sender, RoutedEventArgs e)
        {
            if (radioButton_Copy1.IsChecked == true)
            {
                elementBehaviour = "neverGrid";
            }
        }

        private void radioButton_wire_Checked(object sender, RoutedEventArgs e)
        {
            if (radioButton1.IsChecked == true)
            {
                wireBehaviour = "alwaysGrid";
            }
        }

        private void radioButton_wire_Checked2(object sender, RoutedEventArgs e)
        {
            if (radioButton1_Copy.IsChecked == true)
            {
                wireBehaviour = "endGrid";
            }
        }

        private void radioButton_wire_Checked3(object sender, RoutedEventArgs e)
        {
            if (radioButton1_Copy1.IsChecked == true)
            {
                wireBehaviour = "neverGrid";
            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {        
            if (MessageBox.Show("Are you sure want to reset settings?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ChangeBehaviour("alwaysGrid", "alwaysGrid");
                ChangeSliderValue(50, 50);                
            }            
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
