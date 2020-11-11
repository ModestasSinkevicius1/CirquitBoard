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
    /// Interaction logic for Element_features.xaml
    /// </summary>
    public partial class Element_features : Window
    {
        SpecificElement se;
       
        string oldValue_double = "0";

        string oldValue_int = "0";

        public Element_features(SpecificElement se)
        {
            InitializeComponent();
            this.se = se;
            UpdateTextBoxes();
        }

        private void UpdateTextBoxes()
        {
            label_element.Content = se.GetName();

            textBox_current.Text = se.current.ToString();
            textBox_voltage.Text = se.voltage.ToString();
            textBox_resistance.Text = se.resistance.ToString();
            textBox_power.Text = se.power.ToString();

            textBox_max_connection.Text = se.maxCount.ToString();
            textBox_required_connection.Text = se.requiredCount.ToString();

            checkBox_custom.IsChecked = se.isCustom;

            isCustomChecked();
        }

        private void UpdateElement()
        {           
            se.current = Convert.ToDouble(textBox_current.Text);
            se.voltage = Convert.ToDouble(textBox_voltage.Text);
            se.resistance = Convert.ToDouble(textBox_resistance.Text);
            se.power = Convert.ToDouble(textBox_power.Text);

            se.maxCount = Convert.ToInt32(textBox_max_connection.Text);
            se.requiredCount = Convert.ToInt32(textBox_required_connection.Text);

            se.isCustom = checkBox_custom.IsChecked.Value;

            if(checkBox_custom.IsChecked.Value)
            {
                se.SetConnection(true, se.requiredCount);
            }
        }

        private void CheckTextBoxesWhiteSpace()
        {
            if (textBox_current.Text == "")
            {
                textBox_current.Text = "0";
            }
            if (textBox_power.Text == "")
            {
                textBox_power.Text = "0";
            }
            if (textBox_voltage.Text == "")
            {
                textBox_voltage.Text = "0";
            }
            if (textBox_resistance.Text == "")
            {
                textBox_resistance.Text = "0";
            }
            if (textBox_max_connection.Text == "")
            {
                textBox_max_connection.Text = "0";
            }
            if (textBox_required_connection.Text == "")
            {
                textBox_required_connection.Text = "0";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckTextBoxesWhiteSpace();
            UpdateElement();
            Close();
        }
       
        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;           

            string value = text.Text;
            
            if("" == value)
            {
                value = "0";
            }

            if (!double.TryParse(value, out _))
            {
                text.Text = oldValue_double;
                /*
                if(text.Name != "textBox_current")
                {                   
                   textBox_current.Text = UseAltOhmsLawEquation(Convert.ToDouble(textBox_voltage.Text), Convert.ToDouble(textBox_resistance.Text));
                }
                if(text.Name != "textBox_power")
                {
                    
                }
                if(text.Name != "textBox_voltage")
                {
                    textBox_voltage.Text = UseOhmsLawEquation(Convert.ToDouble(textBox_current.Text), Convert.ToDouble(textBox_resistance.Text));
                }
                if(text.Name != "textBox_resistance")
                {
                    textBox_voltage.Text = UseReistanceEquation(Convert.ToDouble(textBox_voltage.Text), Convert.ToDouble(textBox_current.Text));
                }
                */
            }
            else
            {
                oldValue_double = value;
            }
        }

        private void textBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBox text = sender as TextBox;
            oldValue_double = text.Text;
        }

        private string UseOhmsLawEquation(double i, double r)
        {
            double v;
            
            v = i * r;            

            return Convert.ToString(v);
        }

        private string UseAltOhmsLawEquation(double v, double r)
        {
            double i;

            i = v / r;

            return Convert.ToString(i);
        }

        private string UseReistanceEquation(double v, double i)
        {
            double r;

            r = v / i;

            return Convert.ToString(r);
        }

        private void textBox_TextChanged_int(object sender, TextChangedEventArgs e)
        {
            TextBox text = sender as TextBox;

            string value = text.Text;

            if ("" == value)
            {
                value = "0";
            }

            if (!int.TryParse(value, out _))
            {               
                text.Text = oldValue_int;
            }
            else
            {
                if (Convert.ToInt32(value) <= 4)
                {
                    oldValue_int = value;
                }
                else
                {
                    text.Text = "4";
                    value = "4";
                    oldValue_int = value;                    
                }
            }
        }

        private void checkBox_custom_Checked(object sender, RoutedEventArgs e)
        {
            isCustomChecked();            
        }

        private void checkBox_custom_Unchecked(object sender, RoutedEventArgs e)
        {
            isCustomChecked();
        }

        private void isCustomChecked()
        {
            if (checkBox_custom.IsChecked == false)
            {
                textBox_max_connection.IsEnabled = false;
                textBox_required_connection.IsEnabled = false;
            }
            else
            {
                textBox_max_connection.IsEnabled = true;
                textBox_required_connection.IsEnabled = true;
            }
        }        
    }
}
