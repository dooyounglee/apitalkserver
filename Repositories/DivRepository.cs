using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Models;
using rest1.Controllers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace rest1.Repositories
{
    public interface IDivRepository
    {
        public List<Div>? getDivList();
        public int InsertDiv(string divNm);
        public int EditDiv(int divNo, string divNm);
        public int DeleteDiv(int divNo);
    }

    internal class DivRepository : IDivRepository
    {
        private readonly DbHelper _db;
        // private readonly int pageSize = 10;

        public DivRepository(DbHelper db)
        {
            _db = db;
        }


        public List<Div> getDivList()
        {
            string sql = @"SELECT div_no
                                , div_nm
                             FROM talk.div
                            order by 1";
            var param = new { };

            var dt = _db.ExecuteSelect(sql, param);

            var divs = new List<Div>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                divs.Add(new Div()
                {
                    DivNo = Convert.ToInt16(dt.Rows[i]["div_no"]),
                    DivNm = Convert.ToString(dt.Rows[i]["div_nm"]),
                });
            }
            ;
            return divs;
        }

        public int InsertDiv(string divNm)
        {
            string sql = @"INSERT INTO talk.div (DIV_NO,DIV_NM) VALUES
                           ((SELECT COALESCE(MAX(div_no),0)+1 as div_no FROM talk.div),@divNm)";
            var param = new
            {
                divNm = divNm,
            };

            return _db.ExecuteNonQuery(sql, param);
        }

        public int EditDiv(int divNo, string divNm)
        {
            string sql = @"UPDATE talk.div
                              SET div_nm = @divNm
                            WHERE div_no = @divNo";
            var param = new
            {
                divNo = divNo,
                divNm = divNm,
            };

            return _db.ExecuteNonQuery(sql, param);
        }

        public int DeleteDiv(int divNo)
        {
            string sql = @"DELETE FROM talk.div
                             WHERE DIV_NO = @divNo"
                         ;
            var param = new
            {
                divNo = divNo,
            };

            return _db.ExecuteNonQuery(sql, param);
        }
    }
}
