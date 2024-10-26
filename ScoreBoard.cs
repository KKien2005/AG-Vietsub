using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.Drawing; // Thư viện cho đồ họa
using System.Linq; // Thư viện cho LINQ
using System.Text; // Thư viện cho xử lý chuỗi
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    public class ScoreBoard // Định nghĩa lớp ScoreBoard
    {
        public int SCORE = 0; // Điểm hiện tại
        public int HIGHSCORE = 1000000; // Điểm cao nhất
        private int m_nShipsLeft = 0; // Số tàu còn lại
        private List<Ship> m_ShipsLeft = new List<Ship>(); // Danh sách các tàu còn lại

        public int SHIPSLEFT // Thuộc tính số tàu còn lại
        {
            get { return m_nShipsLeft; } // Lấy số tàu còn lại
            set
            {
                m_nShipsLeft = value; // Gán số tàu còn lại
                if (m_ShipsLeft.Count != m_nShipsLeft) // Nếu số tàu trong danh sách không bằng số tàu còn lại
                {
                    m_ShipsLeft.Clear(); // Xóa danh sách tàu
                    for (int x = 0; x < m_nShipsLeft; x++) // Tạo tàu mới
                    {
                        Ship cur = new Ship(canvas); // Tạo tàu mới
                        cur.position = new Point(445 + (x * 23), 168); // Đặt vị trí tàu
                        m_ShipsLeft.Add(cur); // Thêm tàu vào danh sách
                    }
                }
            }
        }

        public int CURLEVEL = 0; // Cấp độ hiện tại

        public string COPYRIGHTMESSAGE = "©1979 ATARI INC"; // Thông điệp bản quyền
        public string STARTMESSAGE = "PUSH START"; // Thông điệp bắt đầu
        public string GAMEOVERMESSAGE = "GAME OVER"; // Thông điệp kết thúc trò chơi
        public string PLAYERMESSAGE = "PLAYER 1"; // Thông điệp người chơi

        public long blinkTicks; // Thời gian nhấp nháy
        public bool m_bIsPlaying = false; // Trạng thái chơi
        private bool m_bBlinkOn = true; // Trạng thái nhấp nháy

        // Thông tin hiển thị màn hình điểm cao
        private const double DELAYBETWEENHIGHSCOREDISPLAY = 16.0f; // Thời gian giữa các lần hiển thị điểm cao
        private bool m_bIsHighScoreScreenVisible = true; // Trạng thái hiển thị màn hình điểm cao

        public void displayHighScoreScreen() // Hàm hiển thị màn hình điểm cao
        {
            m_bIsHighScoreScreenVisible = true; // Đặt trạng thái hiển thị màn hình điểm cao
            m_lNextToggle = System.DateTime.Now.AddTicks((long)(DELAYBETWEENHIGHSCOREDISPLAY * (double)frmAsteroids.TICKSPERSECOND)).Ticks; // Đặt thời gian tiếp theo để chuyển đổi
        }

        private long m_lNextToggle; // Thời gian tiếp theo để chuyển đổi

        private frmAsteroids canvas; // Tham chiếu đến form chính

        private HighScores m_highScores = new HighScores(); // Đối tượng điểm cao
        public HighScores highScores // Thuộc tính điểm cao
        {
            get { return m_highScores; } // Lấy điểm cao
        }

        public ScoreBoard(frmAsteroids frm) // Constructor của lớp ScoreBoard
        {
            canvas = frm; // Gán form chính vào biến canvas
            m_lNextToggle = System.DateTime.Now.AddTicks((long)(DELAYBETWEENHIGHSCOREDISPLAY * (double)frmAsteroids.TICKSPERSECOND)).Ticks; // Đặt thời gian tiếp theo để chuyển đổi
            blinkTicks = System.DateTime.Now.AddTicks(6000000).Ticks; // Đặt thời gian nhấp nháy
        }

        private Point m_retainStartMsgLeft = new Point(); // Vị trí thông điệp bắt đầu

        public void Draw() // Hàm vẽ bảng điểm
        {
            // ASTEROIDS
            AsteroidsFontHandler afh = new AsteroidsFontHandler(canvas); // Tạo đối tượng xử lý phông chữ
            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White); // Tạo bút vẽ màu trắng

            // SCOREBOARD LEFT!
            afh.FontSize = 32.0f; afh.Kerning = 7; // Đặt kích thước và khoảng cách phông chữ
            afh.Text = SCORE.ToString("D2"); // Đặt văn bản điểm
            afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2) - 410 - (int)afh.TextWidth, 60); // Đặt vị trí văn bản
            afh.Draw(); // Vẽ văn bản

            // SCOREBOARD HIGH SCORE
            afh.FontSize = 18.0f; // Đặt kích thước phông chữ
            afh.Kerning = 3; // Đặt khoảng cách phông chữ
            afh.Text = m_highScores.GetHighScore().ToString(); // Đặt văn bản điểm cao
            afh.TextPosition = new Point((int)((canvas.Width - afh.TextWidth) / 2), 70); // Đặt vị trí văn bản
            afh.Draw(); // Vẽ văn bản

            afh.FontSize = 32.0f; afh.Kerning = 10; // Đặt kích thước và khoảng cách phông chữ
            afh.Text = SCORE.ToString("D2"); // Đặt văn bản điểm

            // ...

            if (System.DateTime.Now.Ticks > blinkTicks) // Nếu thời gian hiện tại lớn hơn thời gian nhấp nháy
            {
                blinkTicks = System.DateTime.Now.AddTicks(4750000).Ticks; // Đặt thời gian nhấp nháy mới
                m_bBlinkOn = !m_bBlinkOn; // Đổi trạng thái nhấp nháy
            }

            if (m_bBlinkOn && !m_bIsPlaying) // Nếu đang nhấp nháy và không chơi
            {
                // STATUS MESSAGE
                afh.Kerning = 10; // Đặt khoảng cách phông chữ
                afh.Text = STARTMESSAGE; // Đặt văn bản thông điệp bắt đầu
                m_retainStartMsgLeft = new Point(((canvas.Width - afh.TextWidth) / 2), 175); // Đặt vị trí thông điệp bắt đầu
                afh.TextPosition = m_retainStartMsgLeft; // Đặt vị trí văn bản

                afh.Draw(); // Vẽ văn bản
            }

            if (!m_bIsPlaying) // Nếu không chơi
            {
                if (m_lNextToggle <= System.DateTime.Now.Ticks) // Nếu thời gian tiếp theo để chuyển đổi nhỏ hơn hoặc bằng thời gian hiện tại
                {
                    m_bIsHighScoreScreenVisible = !m_bIsHighScoreScreenVisible; // Đổi trạng thái hiển thị màn hình điểm cao

                    m_lNextToggle = System.DateTime.Now.AddTicks((long)(DELAYBETWEENHIGHSCOREDISPLAY * (double)frmAsteroids.TICKSPERSECOND)).Ticks; // Đặt thời gian tiếp theo để chuyển đổi
                }

                if (m_bIsHighScoreScreenVisible) // Nếu đang hiển thị màn hình điểm cao
                {
                    afh.Kerning = 10; // Đặt khoảng cách phông chữ
                    afh.Text = "HIGH SCORES"; // Đặt văn bản điểm cao
                    afh.TextPosition = new Point(m_retainStartMsgLeft.X, m_retainStartMsgLeft.Y + 90); // Đặt vị trí văn bản
                    afh.Draw(); // Vẽ văn bản

                    int nX = 0; // Biến đếm
                    foreach (HighScore hs in m_highScores.list.OrderByDescending(hs => hs.Score)) // Lặp qua danh sách điểm cao
                    {
                        String replaceUnderscores = hs.Initials.Replace('_', ' '); // Thay thế dấu gạch dưới bằng khoảng trắng
                        afh.Text = String.Format("{0}{1}.{2,5} {3}", // Đặt văn bản điểm cao
                                            ((nX + 1).ToString().Length == 1) ? " " : "", // Đặt khoảng trắng nếu cần
                                            nX + 1, // Đặt số thứ tự
                                            hs.Score, // Đặt điểm
                                            replaceUnderscores); // Đặt chữ cái
                        afh.TextPosition = new Point(m_retainStartMsgLeft.X - 25, m_retainStartMsgLeft.Y + 175 + (nX * 40)); // Đặt vị trí văn bản
                        afh.Draw(); // Vẽ văn bản
                        nX += 1; // Tăng biến đếm
                    }
                }
            }
            else if (m_bIsPlaying) // Nếu đang chơi
            {
                // STATUS MESSAGE
                afh.Text = PLAYERMESSAGE; // Đặt văn bản thông điệp người chơi
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), 225); // Đặt vị trí văn bản
                afh.Draw(); // Vẽ văn bản

                foreach (Ship s in m_ShipsLeft) // Lặp qua danh sách tàu
                    s.Draw(); // Vẽ tàu
            }

            // GAME OVER MESSAGE
            if (canvas.m_bGameOver && canvas.m_EnterHighScoreScreen.m_bDisplay != true) // Nếu trò chơi kết thúc và không hiển thị màn hình nhập điểm cao
            {
                afh.Text = GAMEOVERMESSAGE; // Đặt văn bản thông điệp kết thúc trò chơi
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), (canvas.Height / 2) - 100); // Đặt vị trí văn bản
                afh.Draw(); // Vẽ văn bản
            }

            // Copyright Message
            afh.FontSize = 18.0f; // Đặt kích thước phông chữ
            afh.Kerning = 3; // Đặt khoảng cách phông chữ
            afh.Text = COPYRIGHTMESSAGE; // Đặt văn bản bản quyền
            afh.TextPosition = new Point((int)((canvas.Width - afh.TextWidth) / 2), canvas.Height - 50); // Đặt vị trí văn bản
            afh.Draw(); // Vẽ văn bản

            // Draw the number of ships the player currently has
        }
    }
}