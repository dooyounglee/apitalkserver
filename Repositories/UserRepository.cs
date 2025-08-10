//using OTILib.DB;
//using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Controllers;

//using System.Windows.Controls;
using rest1.Models;

namespace rest1.Repositories
{
    public interface IUserRepository
    {
        User login(string usrId, string usrPw);
        public List<User> getUserList();
        public User findById(int usrNo);
        public int save(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DbHelper _db;

        public UserRepository(DbHelper db)
        {
            _db = db;
        }

        public User login(string usrId, string usrPw)
        {
            string sql = @"SELECT a.usr_no
                                 , a.usr_id
                                 , a.usr_nm
                                 , a.div_no
                                 , b.div_nm
                              FROM talk.""user"" a
                                 , talk.div b
                             where a.div_no = b.div_no
                               and a.usr_id = @usrId"
                         //    and a.usr_pw = '{usrPw}'"
                         ;
            var param = new
            {
                usrId = usrId,
                // usrPw = usrPw,
            };

            var dt = _db.ExecuteSelect(sql, param);

            // 없으면 null
            if (dt.Rows.Count == 0) return null;

            return new User()
            {
                UsrNo = Convert.ToInt16(dt.Rows[0]["usr_no"]),
                UsrId = Convert.ToString(dt.Rows[0]["usr_id"]),
                UsrNm = Convert.ToString(dt.Rows[0]["usr_nm"]),
                DivNo = Convert.ToInt16(dt.Rows[0]["div_no"]),
                DivNm = Convert.ToString(dt.Rows[0]["div_nm"]),
            };
        }

        public List<User> getUserList()
        {
            string sql = @"SELECT a.usr_no
                                , a.usr_id
                                , a.usr_nm
                                , a.div_no
                                , b.div_nm
                             FROM talk.""user"" a
                                , talk.div b
                            where a.div_no = b.div_no
                              and a.usr_no > 0
                            order by 1";
            var param = new {};

            var dt = _db.ExecuteSelect(sql, param);

            var users = new List<User>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                users.Add(new User()
                {
                    UsrNo = Convert.ToInt16(dt.Rows[i]["usr_no"]),
                    UsrId = Convert.ToString(dt.Rows[i]["usr_id"]),
                    UsrNm = Convert.ToString(dt.Rows[i]["usr_nm"]),
                    DivNo = Convert.ToInt16(dt.Rows[i]["div_no"]),
                    DivNm = Convert.ToString(dt.Rows[i]["div_nm"]),
                });
            }
            ;
            return users;
        }

        public User findById(int usrNo)
        {
            string sql = @"SELECT a.usr_no
                                 , a.usr_id
                                 , a.usr_nm
                                 , a.div_no
                                 , b.div_nm
                              FROM talk.""user"" a
                                 , talk.div b
                             where a.div_no = b.div_no
                               and a.usr_no = @usrNo"
                         ;
            var param = new
            {
                usrNo = usrNo,
            };

            var dt = _db.ExecuteSelect(sql, param);

            // 없으면 null
            if (dt.Rows.Count == 0) return null;

            return new User()
            {
                UsrNo = Convert.ToInt16(dt.Rows[0]["usr_no"]),
                UsrId = Convert.ToString(dt.Rows[0]["usr_id"]),
                UsrNm = Convert.ToString(dt.Rows[0]["usr_nm"]),
                DivNo = Convert.ToInt16(dt.Rows[0]["div_no"]),
                DivNm = Convert.ToString(dt.Rows[0]["div_nm"]),
            };
        }

        public int save(User user)
        {
            string sql = @"INSERT INTO talk.""user"" (USR_NO,USR_NM,DIV_NO,USR_ID,USR_PW) VALUES
                           ((SELECT MAX(USR_NO)+1 FROM talk.""user"")
                           ,@usrNm,1,@usrId,@password1)";
            var param = new
            {
                usrNm = user.UsrNm,
                usrId = user.UsrId,
                password1 = user.Password1,
            };

            return _db.ExecuteNonQuery(sql, param);
        }
    }
}
