using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
//Working On It 
//Osama Zubair
namespace MIBAgent
{
    public class DBConnection
    {
        private DBConnection(string dbname,string server,string username,string password)
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", server, db_name, uname, pwd);
            connection = new MySqlConnection(connstring);
            connection.Open();
        }
        
        private static DBConnection _instance = null;
        
        private string db_name = "mib";
        private string pwd = "admin";
        private string uname = "admin";
        private string server = "localhost";
        
        private MySqlConnection connection = null;
        public MySqlConnection Connection()
        {
            return connection;
        }

        public bool IsConnect()
        {
            if (connection == null)
            {
                if (String.IsNullOrEmpty(db_name))
                    return false;
            }
            return true;
        }
        public MySqlConnection GetConnection(){
            
            return connection;
        }

        public void Close()
        {
            connection.Close();
        }

        public string GetDatabaseName()
        {
            return db_name;
        }
        public void SetDatabaseName(string value)
        {
            db_name = value;
        }
        public void SetPassword(string value) 
        {
         pwd = value;
        }
    }
}

/*
    var dbCon = DBConnection.Instance();
    dbCon.DatabaseName = "YourDatabase";
    if (dbCon.IsConnect())
    {
        //suppose col0 and col1 are defined as VARCHAR in the DB
        string query = "SELECT col0,col1 FROM YourTable";
        var cmd = new MySqlCommand(query, dbCon.Connection);
        var reader = cmd.ExecuteReader();
        while(reader.Read())
        {
            string someStringFromColumnZero = reader.GetString(0);
            string someStringFromColumnOne = reader.GetString(1);
            Console.WriteLine(someStringFromColumnZero + "," + someStringFromColumnOne);
        }
        dbCon.Close();
    }
*/