using rest1.Repositories;
using rest1.Models;
using rest1.Attibutes;

namespace rest1.Services
{
    public interface IRoomService
    {
        Room getRoom(int roomNo, int usrNo);
        List<Room> getRoomList(int usrNo);
        Room createRoom(List<User> userList, User me);
        int EditTitle(int roomNo, int usrNo, string title);
        string Leave(int roomNo, int usrNo, string msg);
        List<User> RoomUserList(int roomNo);
        int CountRoomWithMe(int meNo, int usrNo);
    }

    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IChatService _chatService;

        public RoomService(IRoomRepository roomRepository, IChatService chatService)
        {
            _roomRepository = roomRepository;
            _chatService = chatService;
        }

        [Transaction]
        public Room getRoom(int roomNo, int usrNo)
        {
            return _roomRepository.getRoom(roomNo, usrNo);
        }

        [Transaction]
        public List<Room> getRoomList(int usrNo)
        {
            return _roomRepository.getRoomList(usrNo);
        }

        [Transaction]
        public Room createRoom(List<User> userList, User me)
        {
            int newRoomNo = _roomRepository.getRoomNo();

            string title = me.UsrNm;
            foreach (User u in userList)
            {
                if (me.UsrNo != u.UsrNo)
                {
                    title += "," + u.UsrNm;
                }
            }
            
            userList.Add(me);

            // 방만들기
            _roomRepository.AddRoom(new Room()
            {
                RoomNo = newRoomNo,
                UsrNo = me.UsrNo,
                Title = title,
            });
            
            // 방-유저 연결하기
            foreach (User user in userList)
            {
                _roomRepository.AddRoomUser(new Room()
                {
                    RoomNo = newRoomNo,
                    UsrNo = user.UsrNo,
                    Title = title,
                });
            }

            // 방만들었따는 채팅
            _chatService.InsertChat(newRoomNo, me.UsrNo, "B", me.UsrNo, $"{me.UsrNm}님이 방을 만들었다");

            var newRoom = _roomRepository.getRoom(newRoomNo, me.UsrNo);
            newRoom.Chat = $"{me.UsrNm}님이 방을 만들었다";
            return newRoom;
        }

        [Transaction]
        public int EditTitle(int roomNo, int usrNo, string title)
        {
            return _roomRepository.UpdateTitle(roomNo, usrNo, title);
        }

        [Transaction]
        public string Leave(int roomNo, int usrNo, string msg)
        {
            _roomRepository.LeaveRoom(roomNo, usrNo);

            _chatService.InsertChat(roomNo, usrNo, "D", usrNo, msg);

            return msg;
        }

        [Transaction]
        public List<User> RoomUserList(int roomNo)
        {
            return _roomRepository.SelectRoomUserList(roomNo);
        }

        [Transaction]
        public int CountRoomWithMe(int meNo, int usrNo)
        {
            return _roomRepository.CountRoomWithMe(meNo, usrNo);
        }
    }
}
