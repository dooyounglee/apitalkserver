using rest1.Controllers;
using rest1.Models;
using rest1.Services;
using System.Data;

namespace rest1.Repositories
{
    public interface IRoomRepository
    {
        Room getRoom(int roomNo, int usrNo);
        List<Room> getRoomList(int usrNo);
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly DbHelper _db;

        public RoomRepository(DbHelper db)
        {
            _db = db;
        }

        public Room getRoom(int roomNo, int usrNo)
        {
            string sql = @"SELECT a.room_no
                                 , b.title
                              FROM talk.room a 
                                 , talk.roomuser b
                             where a.room_no = b.room_no
                               and a.room_no = @roomNo
                               and b.usr_no = @usrNo
                               and b.del_yn = 'N'";
            var param = new Dictionary<string, object>
            {
                { "roomNo", roomNo },
                { "usrNo", usrNo },
            };

            var dt = _db.ExecuteSelect(sql, param);

            var roomlist = dt.AsEnumerable().Select(row => new Room()
            {
                RoomNo = (int)(long)dt.Rows[0]["room_no"],
                Title = (string)dt.Rows[0]["title"],
            });

            return roomlist.ToList<Room>()[0];
        }

        public List<Room> getRoomList(int usrNo)
        {
            string sql = @"SELECT a.room_no
                                 , b.title
                                 , c.chat
                                 , coalesce(c.rgt_dtm, a.rgt_dtm) as rgt_dtm
                                 , (SELECT COUNT(*)
                                      FROM talk.chatuser
                                     WHERE ROOM_NO = a.room_no
                                       AND USR_NO = b.usr_no) as cnt_unread
                              FROM talk.room a
                             inner join talk.roomuser b on (a.room_no = b.room_no)
                              left join (select room_no
                                              , chat
                                              , rgt_dtm
                                           from talk.chat
                                          where (room_no, chat_no) in (select room_no, max(chat_no) as chat_no
                                                                         from talk.chat
                                                                        group by room_no)
                                        ) c
                                on (a.room_no = c.room_no)
                             where b.usr_no = @usrNo
                               and b.del_yn = 'N'
                             order by rgt_dtm desc";

            var param = new Dictionary<string, object>
            {
                { "usrNo", 1 },
            };

            var dt = _db.ExecuteSelect(sql, param);

            // if (dt == null)
            // return StatusCode(500, "DB 조회 실패");

            var roomlist = dt.AsEnumerable().Select(row => new Room()
            {
                RoomNo = (int)(long)row["room_no"],
                Title = (string)row["title"],
                Chat = (string)row["chat"],
                RgtDtm = (string)row["rgt_dtm"],
                CntUnread = (int)(long)row["cnt_unread"]
            });

            return roomlist.ToList<Room>();
        }
    }
}
