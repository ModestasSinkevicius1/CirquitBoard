using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Ciruit_board_editor_Framework_version
{
    class DatabaseConnection
    {
        public SQLiteConnection sqlc;

        public DatabaseConnection()
        {
            sqlc = new SQLiteConnection("Data Source=Element.db");
            if (!File.Exists("./Element.db"))
            {
                SQLiteConnection.CreateFile("Element.db");
            }
        }

        public void OpenConnection()
        {
            if (sqlc.State != System.Data.ConnectionState.Open)
            {
                sqlc.Open();
            }
        }

        public void CloseConnection()
        {
            if (sqlc.State != System.Data.ConnectionState.Closed)
            {
                sqlc.Close();
            }
        }

    }
}
