using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Models;
using rest1.Repositories;

namespace rest1.Services
{
    public interface IFileService
    {
        public int saveFile(Models.File file);
        rest1.Models.File getFile(int fileNo);
    }

    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public int saveFile(Models.File file)
        {
            int fileNo = _fileRepository.GetNewFileNo();
            file.FileNo = fileNo;
            _fileRepository.saveFile(file);

            // 파일실물 생성
            if (!System.IO.Directory.Exists(file.FilePath))
            {
                System.IO.Directory.CreateDirectory(file.FilePath);
            }
            System.IO.File.WriteAllBytes(file.FilePath + file.FileName, file.Buffer);

            return fileNo;
        }

        public rest1.Models.File getFile(int fileNo)
        {
            return _fileRepository.getFile(fileNo);
        }
    }
}
