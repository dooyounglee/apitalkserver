//using OTILib.DB;
// using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Controllers;
using rest1.Models;

namespace rest1.Repositories
{
    public interface IFileRepository
    {
        int GetNewFileNo();
        //void saveFile(File file);
    }

    public class FileRepository : IFileRepository
    {
        private readonly DbHelper _db;

        public FileRepository(DbHelper db)
        {
            _db = db;
        }

        public int GetNewFileNo()
        {
            string sql = @$"SELECT COALESCE(MAX(file_no),0)+1 as file_no
                              FROM talk.chatfile";
            var param = new {};

            var dt = _db.ExecuteSelect(sql, param);

            var roomNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roomNo = (int)(long)dt.Rows[0]["file_no"];
            }
            ;

            return roomNo;
        }

        public int saveFile(rest1.Models.File file)
        {
            string sql = @"INSERT INTO talk.chatfile
                           (FILE_NO,FILE_PATH,FILE_NAME,FILE_EXT,ORIGIN_NAME) VALUES
                           (@fileNo,@filePath,@fileName,@fileExt,@originName)";
            var param = new
            {
                fileNo = file.FileNo,
                filepath = file.FilePath,
                filename = file.FileName,
                fileExt = file.FileExt,
                originName = file.OriginName,
            };

            return _db.ExecuteNonQuery(sql, param);
        }
    }
}
