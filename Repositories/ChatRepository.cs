using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Models;
//using OTILib.DB;
//using System.Windows.Interop;
using rest1.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace rest1.Repositories
{
    public interface IChatRepository
    {
        //public Room GetRoom(int roomNo, int usrNo);
        public List<Chat>? getChatList(int roomNo, int usrNo);
        public List<Chat>? getChatList(int roomNo, int usrNo, int page);
        //public int GetRoomNo();
        //public void AddRoom(Room room);
        public void AddRoomUser(Room room);
        //public void LeaveRoom(int roomNo, int usrNo);
        //public void UpdateTitle(int? roomNo, int usrNo, string title);
        public int getNewChatNo();
        public void InsertChat(int newChatNo, int roomNo, int usrNo, string type, string msg);
        public void InsertChat(int newChatNo, int roomNo, int usrNo, string type, string msg, int fileNo);
        //public List<Chat> SelectChats(int roomNo, int usrNo);
        //public List<Chat> SelectChats(int roomNo, int usrNo, int page);
        public int CountChats(int roomNo);
        //public List<User> SelectRoomUserList(int roomNo);
        //public int CountRoomWithMe(int myUsrNo, int usrNo);
        public int CountHeinRoom(int roomNo, int usrNo);
        //public int UpdateTitle(int roomNo, int usrNo, string title);
        public int InsertChatUserExceptMe(int roomNo, int myUsrNo, int chatNo);
        public int ReadChat(int roomNo, int usrNo);
    }

    internal class ChatRepository : IChatRepository
    {
        private readonly DbHelper _db;
        private readonly int pageSize = 10;

        public ChatRepository(DbHelper db)
        {
            _db = db;
        }


        public Room GetRoom(int roomNo, int usrNo)
        {
            string sql = @"SELECT a.room_no
                                 , b.title
                              FROM talk.room a 
                                 , talk.roomuser b
                             where a.room_no = b.room_no
                               and a.room_no = @roomNo
                               and b.usr_no = @usrNo
                               and b.del_yn = 'N'";
            var param = new
            {
                roomNo = roomNo ,
                usrNo = usrNo ,
            };

            var dt = _db.ExecuteSelect(sql, param);

            var roomlist = dt.AsEnumerable().Select(row => new Room()
            {
                RoomNo = (int)(long)dt.Rows[0]["room_no"],
                Title = (string)dt.Rows[0]["title"],
            });

            return roomlist.ToList<Room>()[0];
        }

        public List<Chat> getChatList(int roomNo, int usrNo)
        {
            string sql = @"SELECT a.chat_no
                                 , a.chat
                                 , a.usr_no
                                 , a.chat_fg
                                 , coalesce(a.file_no, 0) as file_no
                              FROM talk.chat a
                             where a.room_no = @roomNo
                               and a.chat_no > (select chat_no
                                                  from talk.roomuser
                                                 where room_no = @roomNo
                                                   and usr_no = @usrNo
                                                   and del_yn = 'N')
                             order by chat_no desc
                             limit 10";
            var param = new
            {
                roomNo = roomNo,
                usrNo = usrNo,
            };

            var dt = _db.ExecuteSelect(sql, param);

            var chats = new List<Chat>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                chats.Add(new Chat()
                {
                    ChatNo = (int)(long)dt.Rows[i]["chat_no"],
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    chat = dt.Rows[i].IsNull("chat") ? "" : (string)dt.Rows[i]["chat"],
                    ChatFg = (string)dt.Rows[i]["chat_fg"],
                    FileNo = (int)(long)dt.Rows[i]["file_no"],
                });
            };

            return chats;
        }

        public List<Chat> getChatList(int roomNo, int usrNo, int page)
        {
            string sql = @"SELECT * FROM (
                        SELECT ROW_NUMBER() OVER(ORDER BY a.chat_no desc) as rownum
                                 , a.chat_no
                                 , a.chat
                                 , a.usr_no
                                 , a.chat_fg
                              FROM talk.chat a
                             where a.room_no = @roomNo
                               and a.chat_no > (select chat_no
                                                  from talk.roomuser
                                                 where room_no = @roomNo
                                                   and usr_no = @usrNo
                                                   and del_yn = 'N')
                             order by chat_no desc
                        )
                     WHERE rownum between @from and @to";
            var param = new
            {
                roomNo = roomNo,
                usrNo = usrNo,
                from = (page - 1) * pageSize + 1,
                to = page * pageSize,
            };

            var dt = _db.ExecuteSelect(sql, param);

            var chats = new List<Chat>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                chats.Add(new Chat()
                {
                    ChatNo = (int)(long)dt.Rows[i]["chat_no"],
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    chat = dt.Rows[i].IsNull("chat") ? "" : (string)dt.Rows[i]["chat"],
                    ChatFg = (string)dt.Rows[i]["chat_fg"],
                });
            }
            ;

            return chats;
        }

        public int GetRoomNo()
        {
            string sql = @$"SELECT COALESCE(MAX(room_no),0)+1 as room_no
                              FROM talk.room";
            var param = new Dictionary<string, object>
            {
            };

            var dt = _db.ExecuteSelect(sql, param);

            var roomNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roomNo = (int)(long)dt.Rows[0]["room_no"];
            };

            return roomNo;
        }

        public void AddRoom(Room room)
        {
            string sql = @"INSERT INTO talk.room (ROOM_NO,USR_NO,TITLE,RGT_DTM) VALUES
                           (@roomNo,@usrNo,@title,to_char(now(),'YYYYMMDDHH24MISS'))";
            var param = new Dictionary<string, object>
            {
                {"roomNo", room.RoomNo},
                {"usrNo", room.UsrNo},
                {"title", room.Title},
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }

        public void AddRoomUser(Room room)
        {
            string sql = @"INSERT INTO talk.roomuser (ROOM_NO,USR_NO,TITLE,CHAT_NO,DEL_YN) VALUES
                           (@roomNo,@usrNo,(SELECT TITLE FROM talk.room where ROOM_NO = @roomNo),
                            (SELECT coalesce(MAX(CHAT_NO),0) FROM talk.chat WHERE ROOM_NO = @roomNo),'N')";
            var param = new Dictionary<string, object>
            {
                {"roomNo", room.RoomNo},
                {"usrNo", room.UsrNo},
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }

        public void LeaveRoom(int roomNo, int usrNo)
        {
            string sql = @$"UPDATE talk.roomuser
                               SET DEL_YN = 'Y'
                             WHERE ROOM_NO = @roomNo
                               AND USR_NO = @usrNo"
                         ;
            var param = new Dictionary<string, object>
            {
                {"roomNo", roomNo},
                {"usrNo", usrNo},
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }

        public void UpdateTitle(int? roomNo, int usrNo, string title)
        {
            string sql = @$"UPDATE talk.room
                               SET TITLE = @Title'
                             WHERE ROOM_NO = @RoomNo
                               AND USR_NO = @UsrNo"
                         ;
            var param = new
            {
                Title = title,
                RoomNo = roomNo,
                UsrNo = usrNo,
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }

        public int getNewChatNo()
        {
            string sql = @$"SELECT COALESCE(MAX(chat_no),0)+1 as chat_no FROM talk.chat";
            var dt = _db.ExecuteSelect(sql, new {});

            var chatNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                chatNo = (int)(long)dt.Rows[0]["chat_no"];
            };

            return chatNo;
        }

        public void InsertChat(int newChatNo, int roomNo, int usrNo, string type, string msg)
        {
            string sql = @"INSERT INTO talk.chat
                            (CHAT_NO,ROOM_NO,USR_NO,CHAT_FG,CHAT,RGT_DTM) VALUES
                            (@newChatNo,@roomNo,@usrNo,@type,@msg,to_char(now(),'YYYYMMDDHH24MISS'))"
                         ;
            var param = new
            {
                newChatNo = newChatNo,
                RoomNo = roomNo,
                UsrNo = usrNo,
                type = type,
                msg = msg,
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }
        public void InsertChat(int newChatNo, int roomNo, int usrNo, string type, string msg, int fileNo)
        {
            string sql = @"INSERT INTO talk.chat
                            (CHAT_NO,ROOM_NO,USR_NO,CHAT_FG,CHAT, FILE_NO ,RGT_DTM) VALUES
                            (@newChatNo,@roomNo,@usrNo,@type,@msg,@fileNo, to_char(now(),'YYYYMMDDHH24MISS'))"
                         ;
            var param = new
            {
                newChatNo = newChatNo,
                RoomNo = roomNo,
                UsrNo = usrNo,
                type = type,
                fileNo = fileNo,
                msg = msg,
            };

            int result = _db.ExecuteNonQuery(sql, param);
        }


        public int CountChats(int roomNo)
        {
            string sql = @"SELECT COUNT(*) as cnt
                              FROM talk.chat
                             where room_no = @roomNo";
            var param = new
            {
                roomNo = roomNo,
            };

            var dt = _db.ExecuteSelect(sql, param);

            return (int)(long)dt.Rows[0]["cnt"];
        }

        public List<User> SelectRoomUserList(int roomNo)
        {
            string sql = @"SELECT b.usr_no
                                 , b.usr_nm
                              FROM talk.roomuser a
                                 , talk.""user"" b
                             where a.room_no = @roomNo
                               and a.usr_no = b.usr_no
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

        public int CountHeinRoom(int roomNo, int usrNo)
        {
            string sql = @"SELECT count(*) as cnt
                              FROM talk.roomuser
                             where room_no = @roomNo
                               and usr_no = @usrNo
                               and del_yn = 'N'";
            var param = new
            {
                roomNo = roomNo,
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

        public int UpdateTitle(int roomNo, int usrNo, string title)
        {
            string sql = @"UPDATE talk.roomuser
                               SET TITLE = @title
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

        public int InsertChatUserExceptMe(int roomNo, int myUsrNo, int chatNo)
        {
            string sql = @"INSERT INTO talk.chatuser (ROOM_NO,CHAT_NO,USR_NO)
                            SELECT @roomNo, @chatNo, USR_NO
                              FROM talk.roomuser
                             WHERE ROOM_NO = @roomNo
                               AND USR_NO != @myUsrNo
                               AND DEL_YN = 'N'"
                         ;
            var param = new
            {
                roomNo = roomNo,
                chatNo = chatNo,
                myUsrNo = myUsrNo,
            };

            return _db.ExecuteNonQuery(sql, param);
        }

        public int ReadChat(int roomNo, int usrNo)
        {
            string sql = @"DELETE FROM talk.chatuser
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
    }
}
