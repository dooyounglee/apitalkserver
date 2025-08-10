using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;

namespace rest1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RoomRepository _roomRepository;

        private readonly DbHelper _db;

        public HomeController(AppDbContext context, RoomRepository roomRepository, DbHelper db)
        {
            _context = context;
            _roomRepository = roomRepository;
            _db = db;
        }

        //[HttpPost("hello")]
        //public async Task<IActionResult> Index([FromBody] MyRequest request)
        //{
        //    var users = await _context.Users.ToListAsync();
        //    return Ok(users);
        //}

        //[HttpGet("chat")]
        //public async Task<IActionResult> Chat()
        //{
        //    var title = _roomRepository.GetRoomTitle(1, 2);
        //    return Ok(title);
        //}

        [HttpGet("roomlist")]
        public async Task<IActionResult> roomlist()
        {
            string sql = @$"SELECT a.room_no
                                 , b.title
                                 , c.chat
                                 , coalesce(c.rgt_dtm, a.rgt_dtm) as rgt_dtm
                                 , (SELECT COUNT(*)
                                      FROM talk.chatuser
                                     WHERE ROOM_NO = a.room_no
                                       AND USR_NO = b.usr_no) as cnt_unread
                              FROM talk.room a
                             inner join talk.roomuser b on (a.room_no = b.room_no)
                              left join (select room_no
                                              , chat
                                              , rgt_dtm
                                           from talk.chat
                                          where (room_no, chat_no) in (select room_no, max(chat_no) as chat_no
                                                                         from talk.chat
                                                                        group by room_no)
                                        ) c
                                on (a.room_no = c.room_no)
                             where b.usr_no = @usrNo
                               and b.del_yn = 'N'
                             order by rgt_dtm desc";
            var param = new Dictionary<string, object>
            {
                { "usrNo", 1 },
            };

            var dt = _db.ExecuteSelect(sql, param);

            if (dt == null)
                return StatusCode(500, "DB 조회 실패");

            var roomlist = dt.AsEnumerable().Select(row => new
            {
                Id = row["room_no"],
                Name = row["title"],
                chat = row["chat"],
                rgtDtm = row["rgt_dtm"],
                unread = row["cnt_unread"]
            });

            return Ok(roomlist);
        }
    }
}
