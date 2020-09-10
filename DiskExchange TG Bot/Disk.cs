using System;
using System.Collections.Generic;
using System.Text;

namespace DiskExchange_TG_Bot
{
    class Disk
    {
        string photoId; //file_id for disk's photo
        string game; //Name of the game
        double price; //Game's price. If set to 0, price will not display in the message
        int diskId; //Id of the disk for the database
        int userId; //User's id for the database
        string exchange;

    }
}
