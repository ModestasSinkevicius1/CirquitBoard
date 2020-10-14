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
    class ShortcutGUIControl
    {
        private Label shortcut;
        public ShortcutGUIControl(Label shortcut)
        {
            this.shortcut = shortcut;
        }

        public void UpdateText(string keys)
        {
            if (keys == "OnControl")
            {
                shortcut.Content = "X" + " - " + "Delete" + "  ";
                shortcut.Content += "W" + " - " + "Wire";
            }
            if(keys == "OnCanvas")
            {
                shortcut.Content = "SHIFT" + " - " + "Pan canvas" + "  ";
                shortcut.Content += "E" + " - " + "Create";
            }
            if (keys == "")
                shortcut.Content = "";
        }
    }
}
