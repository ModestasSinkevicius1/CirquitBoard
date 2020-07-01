using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Ciruit_board_editor_Framework_version
{
    class DatabaseControl
    {
        private List<Element> eList = new List<Element>();        

        public List<Element> GetElements()
        {
            DatabaseConnection DBc = new DatabaseConnection();
            string query = "SELECT * FROM ElementType";
            SQLiteCommand sqlcommand = new SQLiteCommand(query, DBc.sqlc);
            DBc.OpenConnection();
            SQLiteDataReader result = sqlcommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    Element e = new Element(Convert.ToInt32(result["ID"]), Convert.ToString(result["Type"]), Convert.ToInt32(result["ConnectionCount"]));
                    eList.Add(e);
                }
            }            
            DBc.CloseConnection();
            return eList;
        }
    }
}
