using rest1.Repositories;
using rest1.Models;

namespace rest1.Services
{
    public interface IRoomService
    {
        Room getRoom(int roomNo, int usrNo);
        List<Room> getRoomList(int usrNo);
    }

    public class RoomService : IRoomService
    {
        private readonly IRoomRespository _roomRepository;

        public RoomService(IRoomRespository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public Room getRoom(int roomNo, int usrNo)
        {
            return _roomRepository.getRoom(roomNo, usrNo);
        }

        public List<Room> getRoomList(int usrNo)
        {
            return _roomRepository.getRoomList(usrNo);
        }
    }
}
