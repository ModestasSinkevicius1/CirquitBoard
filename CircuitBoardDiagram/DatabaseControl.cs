using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;


namespace CircuitBoardDiagram
{
    class DatabaseControl
    {
        private List<DatabaseElement> eList = new List<DatabaseElement>();
        public List<DatabaseElement> GetElements()
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
                    DatabaseElement e = new DatabaseElement(Convert.ToInt32(result["ID"]), Convert.ToString(result["Type"]), Convert.ToInt32(result["ConnectionCount"]));
                    eList.Add(e);
                }
            }
            DBc.CloseConnection();
            return eList;
        }
    }
}
