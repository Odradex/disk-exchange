using System;
using System.Data.SQLite;
using System.Diagnostics.Tracing;
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
            connection = new SQLiteConnection(path2);
            Console.Write("1/2: Connecting to Database... ".Pastel(Color.Yellow));
            Console.Beep();
            connection.Open();
            Console.WriteLine("[READY]");
        }
        #region AwaitInfo
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
            if (rdr.HasRows == false)
                return 0;
            return rdr.GetInt32(5);
        }
        #endregion
        #region Platform
        public string GetPlatform(int userId)
        {
            cmd = new SQLiteCommand($"SELECT platform FROM disks WHERE id = (SELECT editDiscId FROM users WHERE id = {userId})", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public void SetPlatform(byte b, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET platform = '{platformNames[b]}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #endregion
        #region Disc
        public int NewDisc(int Id)
        {
            cmd = new SQLiteCommand($"INSERT INTO disks(sellerId, location) VALUES({Id}, (SELECT location FROM users WHERE id = {Id}))", connection);
            cmd.ExecuteNonQueryAsync();
            cmd = new SQLiteCommand($"UPDATE users SET editDiscId = (SELECT last_insert_rowid() FROM disks) WHERE id = {Id}", connection);
            cmd.ExecuteNonQueryAsync();
            return 0;
        }
        public void DeleteDisc(int Id)
        {
            cmd = new SQLiteCommand($"DELETE FROM disks WHERE id = (SELECT editDiscId FROM users WHERE id = {Id})", connection);
            cmd.ExecuteNonQuery();
        }
        #endregion
        public void NewUser(int userId)
        {
            cmd = new SQLiteCommand($"INSERT INTO users(id) VALUES ({userId})", connection);
            cmd.ExecuteNonQueryAsync();
        }
        private int GetDiscId(int Id)
        {
            cmd = new SQLiteCommand($"SELECT * FROM users WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(4);
        }
        #region Photo
        public void SetPhoto(string fileId, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET photo = '{fileId}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        public bool IsUserHasDisk(int Id)
        {
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                counter++;
            }
            if (counter == 0)
                return false;
            return true;
        }
        public string GetPhoto(int Id,int num)
        {
            num -= 1;
            int counter = 0;
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if(counter == num)
                    return rdr.GetString(3);
                counter++;
            }
            return "AgACAgIAAxkBAAIGZF9aSti3CZNeKoW3AjRGDco3-45KAAL3rjEb0L7RSjbSrDV25SE0ECFzly4AAwEAAwIAA3gAA3CNAAIbBA";
        }
        #endregion
        #region Exchange
        internal string GetExchange(int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"SELECT exchange FROM disks WHERE id = {discId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetString(0);
        }
        public void SetExchange(string e, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET exchange = '{e}' WHERE id = {discId}", connection);
            cmd.ExecuteNonQueryAsync();
        }
        #endregion
        public void SetPrice(string price, int discId, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                discId = GetDiscId(discId);
            cmd = new SQLiteCommand($"UPDATE disks SET price = '{price}' WHERE id = {discId}", connection);
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
        public int GetEditMessageId(int userId)
        {
            cmd = new SQLiteCommand($"SELECT editMessageId FROM users WHERE id = {userId}", connection);
            rdr = cmd.ExecuteReader();
            rdr.ReadAsync();
            return rdr.GetInt32(0);
        }
        public string GetUserDisks(int userId)
        {
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {userId}",connection);
            rdr = cmd.ExecuteReader();
            string temp = "";
            int count = 0;
            while (rdr.Read())
            {
                count++;
                temp += $"{count}: {rdr.GetString(1)} | {rdr.GetString(2)} | {rdr.GetString(4)} BYN\n";
            }
            return temp + "\n\nЧтобы выбрать товар, отправьте его номер в следующем сообщении.";
        }
        public string GetSelectedDisk(int Id,int num)
        {
            num -= 1;
            int counter = 0;
            string str ="Диск под данным номером не найден...";
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE sellerId = {Id}", connection);
            rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                if(counter == num)
                {
                    cmd = new SQLiteCommand($"UPDATE users SET editDiscId = {rdr.GetInt32(0)} WHERE Id = {Id}", connection);
                    cmd.ExecuteReader();
                    str = GetCaption(rdr.GetInt32(0));
                }
                counter++;
            }
            return str;
        }

        internal void SetLocation(string text, int id)
        {
            cmd = new SQLiteCommand($"UPDATE users SET location = '{text}' WHERE Id = {id}", connection);
            cmd.ExecuteNonQuery();
        }

        public string GetCaption(int Id, bool getIdFromUser = false)
        {
            if (getIdFromUser)
                Id = GetDiscId(Id);
            cmd = new SQLiteCommand($"SELECT * FROM disks WHERE id = {Id}", connection);
            rdr = cmd.ExecuteReader();
            rdr.Read();
            return $"💿Игра: {rdr.GetString(1)} | {rdr.GetString(2)}\n" +
                    $"💵Цена: {((rdr.GetString(4) != "")  ? rdr.GetString(4) + " BYN": "Не указана")}\n" + (rdr.GetString(5) != "" ?
                    $"🔄Обмен на: {rdr.GetString(5)}\n" : "") +
                    $"📍Расположение: {rdr.GetString(7)}";
        }
    }
}