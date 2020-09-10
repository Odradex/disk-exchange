using System;
using System.Collections.Generic;
using System.Text;

namespace DiskExchange_TG_Bot
{
    class Disc
    {
        string game; //Name of the game
        double price; //Game's price. If set to 0, price will not display in the message
        int discId; //Id of the disc for the database
        public int userId;//User's id for the database
        string exchange; //Games that seller wants to exchange for. If set to null, will not display
        short platform; //Game platform (1-PS4 2-XONE 3-SWITCH)

        public string message;
        string photoId 
        {
            get { return photoId; } 
            set
            {
                photoId = value;
            }
        } //file_id for disk's photo
        string gameName
        {
            get { return gameName; }
            set
            {
                gameName = value;
            }
        }//name of the selling game
        public Disc(int user)
        {
            userId = user;
            message = "[Фото]\n💿:[Название] | [Платформа]\n💵Цена:[Цена]\n🔄Обмен на:[Обмен]\n📍Расположение:[Местоположение]";

        }
    }
}