namespace rest1.Controllers
{
    public class RoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        //public List<RoomUser>? GetRoomTitle(int roomNo, int usrNo)
        //{
        //    var query = from room in _context.Rooms
        //                join roomUser in _context.RoomUsers on room.RoomNo equals roomUser.RoomNo
        //                where room.RoomNo == roomNo
        //                      && roomUser.UsrNo == usrNo
        //                      && roomUser.DelYn == "N"
        //                select new RoomUser() { RoomNo = room.RoomNo, Title = roomUser.Title };

        //    return query.ToList<RoomUser>();
        //}
    }
}
