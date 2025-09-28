using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTILib.Util;
using rest1.Attibutes;

//using System.Windows.Interop;
using rest1.Models;
using rest1.Repositories;

namespace rest1.Services
{
    public interface IChatService
    {
        //public Room getChat(int roomNo);
        // public List<Room> getChatList(int usrNo);
        public int InsertChat(int roomNo, int usrNo, string type, int meUsrNo, string msg);
        public int InsertChat(int roomNo, int usrNo, string type, int meUsrNo, Models.File file);
        //public List<Chat> SelectChats(int roomNo);
        public List<Chat> getChatList(int roomNo, int usrNo, int page);
        public int CountChats(int roomNo);
        //public int CreateRoom(List<User> userList);
        //[Obsolete] public string Invite(int roomNo, List<User> userList);
        public Room Invite(int roomNo, List<int> usrNos, string usrNms, int meNo, string moNm);
        //public string Leave(int roomNo, int usrNo);
        //public List<User> RoomUserList(int roomNo);
        //public int CountRoomWithMe(int usrNo);
        public bool IsThereSomeoneinRoom(int roomNo, List<int> usrNos);
        //public void EditTitle(int roomNo, int usrNo, string title);
        public int ReadChat(int roomNo, int usrNo);
    }

    public class ChatService : IChatService
    {
        private readonly IChatRepository? _chatRepository;
        private readonly IFileService _fileService;

        public ChatService(IChatRepository? chatRepository, IFileService fileService)
        {
            _chatRepository = chatRepository;
            _fileService = fileService;
        }

        //public Room getChat(int roomNo)
        //{
        //    return _chatRepository.GetRoom(roomNo, _userService.Me.UsrNo);
        //}

        [Transaction]
        public List<Chat> getChatList(int roomNo, int usrNo, int page)
        {
            if (page == 0)
            {
                return _chatRepository.getChatList(roomNo, usrNo);
            } else
            {
                return _chatRepository.getChatList(roomNo, usrNo, page);
            }
        }

        [Transaction]
        public int InsertChat(int roomNo, int usrNo, string type, int meUsrNo, string msg)
        {
            int newChatNo = _chatRepository.getNewChatNo();
            _chatRepository.InsertChat(newChatNo, roomNo, usrNo, type, msg);
            _chatRepository.InsertChatUserExceptMe(roomNo, meUsrNo, newChatNo);
            return newChatNo;
        }

        [Transaction]
        public int InsertChat(int roomNo, int usrNo, string type, int meUsrNo, Models.File file)
        {
            int fileNo = _fileService.saveFile(file);

            int newChatNo = _chatRepository.getNewChatNo();
            _chatRepository.InsertChat(newChatNo, roomNo, usrNo, type, file.OriginName, fileNo);
            _chatRepository.InsertChatUserExceptMe(roomNo, meUsrNo, newChatNo);
            return newChatNo;
        }

        //public List<Chat> SelectChats(int roomNo)
        //{
        //    return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo);
        //}
        //public List<Chat> SelectChats(int roomNo, int page)
        //{
        //    return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo, page);
        //}

        [Transaction]
        public int CountChats(int roomNo)
        {
            return _chatRepository.CountChats(roomNo);
        }

        //public int CreateRoom(List<User> userList)
        //{
        //    int newRoomNo = _chatRepository.GetRoomNo();

        //    string title = _userService.Me.UsrNm;
        //    foreach (User u in userList)
        //    {
        //        if (_userService.Me.UsrNo != u.UsrNo)
        //        {
        //            title += "," + u.UsrNm;
        //        }
        //    }

        //    userList.Add(_userService.Me);

        //    // 방만들기
        //    _chatRepository.AddRoom(new Room()
        //    {
        //        RoomNo = newRoomNo,
        //        UsrNo = _userService.Me.UsrNo,
        //        Title = title,
        //    });

        //    // 방-유저 연결하기
        //    foreach (User user in userList)
        //    {
        //        _chatRepository.AddRoomUser(new Room()
        //        {
        //            RoomNo = newRoomNo,
        //            UsrNo = user.UsrNo,
        //            Title = title,
        //        });
        //    }

        //    // 방만들었따는 채팅
        //    InsertChat(newRoomNo, _userService.Me.UsrNo, "B", $"{_userService.Me.UsrNm}님이 방을 만들었다");

        //    return newRoomNo;
        //}

        // [Obsolete]
        // public string Invite(int roomNo, List<User> userList)
        // {
        //     string invitedUsers = string.Join(",", userList.Select(u => u.UsrNm));
        // 
        //     // 방-유저 연결하기
        //     foreach (User user in userList)
        //     {
        //         _chatRepository.AddRoomUser(new Room()
        //         {
        //             RoomNo = roomNo,
        //             UsrNo = user.UsrNo,
        //             Title = $"{_userService.Me.UsrNm},{invitedUsers}",
        //         });
        //     }
        // 
        //     var msg = $"{_userService.Me.UsrNm}님이 {invitedUsers}님을 초대했다";
        // 
        //     // InsertChat(roomNo, _userService.Me.UsrNo, "C", msg);
        // 
        //     return msg;
        // }
        [Transaction]
        public Room Invite(int roomNo, List<int> usrNos, string usrNms, int meNo, string meNm)
        {
            // 초대한사람 방제목 가져오기
            string? title;
            var room = _chatRepository.GetRoom(roomNo, meNo);
            if (room.ModifyYn == "Y")
            {
                title = room.Title;
            }
            else
            {
                title = $"{room.Title},{usrNms}";
            }

            // 방-유저 연결하기
            foreach (int usrNo in usrNos)
            {
                _chatRepository.AddRoomUser(new Room()
                {
                    RoomNo = roomNo,
                    UsrNo = usrNo,
                    Title = title,
                });
            }

            var msg = $"{meNm}님이 {usrNms}님을 초대했다";

            InsertChat(roomNo, meNo, "C", meNo, msg);

            return new Room()
            {
                RoomNo = roomNo,
                Chat = msg,
                Title = title,
            };
        }

        //public string Leave(int roomNo, int usrNo)
        //{
        //    _chatRepository.LeaveRoom(roomNo, usrNo);

        //    var msg = $"{_userService.Me.UsrNm}님이 나갔다";

        //    InsertChat(roomNo, _userService.Me.UsrNo, "D", msg);

        //    return msg;
        //}

        //public List<User> RoomUserList(int roomNo)
        //{
        //    return _chatRepository.SelectRoomUserList(roomNo);
        //}

        //public int CountRoomWithMe(int usrNo)
        //{
        //    return _chatRepository.CountRoomWithMe(_userService.Me.UsrNo, usrNo);
        //}

        [Transaction]
        public bool IsThereSomeoneinRoom(int roomNo, List<int> usrNos)
        {
            bool result = false;
            foreach(var usrNo in usrNos)
            {
                int countHeinRoom = _chatRepository.CountHeinRoom(roomNo, usrNo);
                if (countHeinRoom > 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        //public void EditTitle(int roomNo, int usrNo, string title)
        //{
        //    _chatRepository.UpdateTitle(roomNo, usrNo, title);
        //}

        [Transaction]
        public int ReadChat(int roomNo, int usrNo)
        {
            return _chatRepository.ReadChat(roomNo, usrNo);
        }
    }
}
