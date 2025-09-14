using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rest1.Attibutes;
using rest1.Models;
using rest1.Repositories;

namespace rest1.Services
{
    public interface IDivService
    {
        public List<Div> getDivList();
        public int InsertDiv(string divNm);
        public int EditDiv(int divNo, string divNm);
    }

    public class DivService : IDivService
    {
        private readonly IDivRepository? _divRepository;

        public DivService(IDivRepository? divRepository)
        {
            _divRepository = divRepository;
        }

        [Transaction]
        public List<Div> getDivList()
        {
            return _divRepository.getDivList();
        }

        [Transaction]
        public int InsertDiv(string divNm)
        {
            return _divRepository.InsertDiv(divNm);
        }

        [Transaction]
        public int EditDiv(int divNo, string divNm)
        {
            return _divRepository.EditDiv(divNo, divNm);
        }
    }
}
