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
        int getRoomNo();
        void AddRoom(Room room);
        void AddRoomUser(Room room);
        int UpdateTitle(int roomNo, int usrNo, string title);
        int LeaveRoom(int roomNo, int usrNo);
        List<User> SelectRoomUserList(int roomNo);
        int CountRoomWithMe(int myUsrNo, int usrNo);
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
                                 , b.modify_yn
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
                ModifyYn = (string)dt.Rows[0]["modify_yn"],
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
                { "usrNo", usrNo },
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

        public int getRoomNo()
        {
            string sql = @$"SELECT COALESCE(MAX(room_no),0)+1 as room_no
                              FROM talk.room";
            var dt = _db.ExecuteSelect(sql, new { });

            var roomNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roomNo = (int)(long)dt.Rows[0]["room_no"];
            }
            ;

            return roomNo;
        }

        public void AddRoom(Room room)
        {
            string sql = @"INSERT INTO talk.room (ROOM_NO,USR_NO,RGT_DTM) VALUES
                           (@roomNo,@usrNo,to_char(now(),'YYYYMMDDHH24MISS'))";
            var param = new
            {
                roomNo = room.RoomNo,
                usrNo =  room.UsrNo,
            };
            int result = _db.ExecuteNonQuery(sql, param);
        }

        public void AddRoomUser(Room room)
        {
            string sql = @"INSERT INTO talk.roomuser (ROOM_NO,USR_NO,TITLE,CHAT_NO,DEL_YN,modify_yn) VALUES
                           (@roomNo,@usrNo,@title,
                            (SELECT coalesce(MAX(CHAT_NO),0) FROM talk.chat WHERE ROOM_NO = @roomNo),'N','N')";
            var param = new 
            {
                roomNo = room.RoomNo,
                usrNo = room.UsrNo,
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }

        public int UpdateTitle(int roomNo, int usrNo, string title)
        {
            string sql = @"UPDATE talk.roomuser
                               SET TITLE = @title
                                 , modify_yn = 'Y'
                             WHERE ROOM_NO = @roomNo
                               AND USR_NO = @usrNo
                               AND DEL_YN = 'N'";
            var param = new
            {
                title = title,
                roomNo = roomNo,
                usrNo = usrNo,
            };

            return _db.ExecuteNonQuery(sql, param);
        }

        public int LeaveRoom(int roomNo, int usrNo)
        {
            string sql = @$"UPDATE talk.roomuser
                               SET DEL_YN = 'Y'
                             WHERE ROOM_NO = @roomNo
                               AND USR_NO = @usrNo"
                         ;
            var param = new
            {
                roomNo = roomNo,
                usrNo = usrNo,
            };

            return _db.ExecuteNonQuery(sql, param);
        }

        public List<User> SelectRoomUserList(int roomNo)
        {
            string sql = @"SELECT b.usr_no
                                 , b.usr_nm
                                 , c.div_nm
                              FROM talk.roomuser a
                                 , talk.""user"" b
                                 , talk.div c
                             where a.room_no = @roomNo
                               and a.usr_no = b.usr_no
                               and b.div_no = c.div_no
                               and a.del_yn = 'N'
                             order by usr_nm";
            var param = new
            {
                roomNo = roomNo,
            };

            var dt = _db.ExecuteSelect(sql, param);

            var userList = new List<User>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                userList.Add(new User()
                {
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    UsrNm = (string)dt.Rows[i]["usr_nm"],
                    DivNm = (string)dt.Rows[i]["div_nm"],
                });
            }
            ;

            return userList;
        }

        public int CountRoomWithMe(int myUsrNo, int usrNo)
        {
            string sql = @"SELECT count(*) as cnt
                              FROM (SELECT room_no
                                         , count(*) as cnt
                                      FROM talk.roomuser
                                     WHERE DEL_YN = 'N'
                                     GROUP BY room_no
                                    HAVING count(*) = 2) A
                                 , (SELECT room_no
                                         , count(*) as cnt
                                      FROM talk.roomuser
                                     WHERE usr_no in (@myUsrNo,@usrNo)
                                       AND DEL_YN = 'N'
                                     GROUP BY room_no) B
                             where A.room_no = B.room_no
                               and A.cnt = B.cnt";
            var param = new
            {
                myUsrNo = myUsrNo,
                usrNo = usrNo,
            };

            var dt = _db.ExecuteSelect(sql, param);

            int result = -1;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                result = (int)(long)dt.Rows[i]["cnt"];
            }
            return result;
        }
    }
}
