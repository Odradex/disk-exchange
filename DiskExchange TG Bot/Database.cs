using System;
using System.Data.SQLite;
using System.Drawing;
using Pastel;

namespace DiskExchange_TG_Bot
{
    class Database
    {
        static SQLiteConnection connection;
        SQLiteCommand cmd;
        SQLiteDataReader rdr;
        string[] platformNames = { "PS4", "Xbox", "Switch" };
        public Database()
        {
            string path1 = @"URI=file:X:\Programs\SQLite\DiskExchangeDB.db";
            string path2 = @"URI=file:D:\DataBase\DiskExchangeDB.db";
            connection = new SQLiteConnection(path1);
            Console.Write("1/2: Connecting to Database... ".Pastel(Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");
        }
        public void SetAwaitInfoType(int userId, int type)
        {
            cmd = new SQLiteCommand($"UPDATE users SET awaitInfoType = {type} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public int GetAwaitInfoType(int userId)
        {
            cmd = new SQLiteCommand($"SELECT * FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(5);
        }
        public string GetPlatform(int userId)
        {
            cmd = new SQLiteCommand($"SELECT platform FROM disks WHERE id = (SELECT editDiscId FROM users WHERE id = {userId})", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public int NewDisc(int Id)
        {
            cmd = new SQLiteCommand($"INSERT INTO disks(sellerId, location) VALUES({Id}, (SELECT location FROM users WHERE id = {Id}))", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE users SET editDiscId = (SELECT last_insert_rowid() FROM disks) WHERE id = {Id}", connection);
            cmd.ExecuteNonQueryAsync();
            return 0;
        }
        public void NewUser(int userId, string loc)
        {
            cmd = new SQLiteCommand($"INSERT INTO users(id, location) VALUES {userId}, '{loc}'", connection);
            cmd.ExecuteNonQueryAsync();
        }
        private int GetDiscId(int Id)
        {
            cmd = new SQLiteCommand($"SELECT * FROM users WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(4);
        }
        public void SetPhoto(string fileId, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET photo = '{fileId}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetPrice(double price, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET price = {price} WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetExchange(string e, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET exchange = '{e}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetPlatform(byte b, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET platform = '{platformNames[b]}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetName(string n, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET name = '{n}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public void SetEditMessageId(int userId, int messageId)
        {
            cmd = new SQLiteCommand($"UPDATE users SET editMessageId = {messageId} WHERE id = {userId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public string GetCaption(int Id, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                Id = GetDiscId(Id);
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            return $"💿Игра: {rdr.GetString(1)} | {rdr.GetString(2)}\n" +
                    $"💵Цена: {((Convert.ToDouble(rdr.GetString(4)) > 0)  ? rdr.GetString(4) + " BYN": "Не указана")}\n" + (rdr.GetString(5) != "" ?
                    $"🔄Обмен на: {rdr.GetString(5)}\n" : "") +
                    $"📍Расположение: {rdr.GetString(7)}";
        }
    }
}