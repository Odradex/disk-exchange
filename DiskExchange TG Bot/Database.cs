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
            string path1 = @"URI=file:X:\Programs\SQLite\DiskExchangeDB.db";
            string path2 = @"URI=file:D:\DataBase\DiskExchangeDB.db";
            connection = new SQLiteConnection(path1);
            Console.Write("1/2: Connecting to Database... ".Pastel(System.Drawing.Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");


            //cmd = new SQLiteCommand("INSERT INTO disks(name) VALUES('test');", connection);
            //cmd.ExecuteNonQuery();
            //rdr = cmd.ExecuteReader();
        }
        public int NewDisc(int userId, int EditMessageId)
        {
            return 0;
        }
        public void SetPhoto(int UserId, string fileId)
        {

        }
        public void SetPrice(int UserId, double price)
        {

        }
        public void SetExchange(int UserId, string e)
        {

        }
        public void SetPlatform(int UserId, byte b)
        {

        }
        public void SetName(int UserId, string n)
        {

        }
        public int GetEditMessageId(int UserId)
        {
            return 0;
        }
        public string GetCaption(int UserId)
        {
            return "temp";
        }
    }
}