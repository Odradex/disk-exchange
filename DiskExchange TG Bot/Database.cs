using System;
using System.Data.SQLite;
using System.Drawing;
using Pastel;

/*
 cmd.ExecuteNonQueryAsync(); - выполнить команду
 
 */

namespace DiskExchange_TG_Bot
{
    class Database
    {
        SQLiteConnection connection;
        SQLiteCommand cmd;
        SQLiteDataReader rdr;
        public Database()
        {
            string filePath = @"URI=file:X:\Programs\SQLite\DiskExchangeDB.db";
            connection = new SQLiteConnection(filePath);
            Console.Write("1/2: Connecting to Database... ".Pastel(System.Drawing.Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");
           

            cmd = new SQLiteCommand("INSERT INTO disks(name) VALUES('test');", connection);
            cmd.ExecuteNonQuery();
            //rdr = cmd.ExecuteReader();
        }
    }
}
