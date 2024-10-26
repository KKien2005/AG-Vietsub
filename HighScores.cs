using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.Linq; // Thư viện cho LINQ
using System.Text; // Thư viện cho xử lý chuỗi
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ
using System.IO; // Thư viện cho thao tác với hệ thống tệp
using System.Drawing; // Thư viện cho đồ họa
using System.Windows.Forms; // Thư viện cho Windows Forms

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    public class HighScores // Định nghĩa lớp HighScores
    {
        private int MAXHIGHSCORES = 10; // Số điểm cao tối đa
        private List<HighScore> m_lstHighScores = new List<HighScore>(); // Danh sách các điểm cao
        private String HighScoresPath = Directory.GetCurrentDirectory() + "\\HighScores.DAT"; // Đường dẫn đến tệp điểm cao

        public HighScores() // Constructor của lớp HighScores
        {
            Load(); // Tải danh sách điểm cao từ tệp
        }

        public List<HighScore> list // Thuộc tính danh sách điểm cao
        {
            get { return m_lstHighScores; } // Lấy danh sách điểm cao
        }

        public void Save() // Hàm lưu danh sách điểm cao
        {
            List<string> lines = new List<string>(); // Danh sách các dòng để lưu
            foreach (HighScore hs in m_lstHighScores) // Lặp qua từng điểm cao
            {
                lines.Add(String.Format("{0}{1}", hs.Initials, hs.Score)); // Thêm thông tin vào danh sách
            }
            File.WriteAllLines(@HighScoresPath, lines.ToArray(), Encoding.UTF8); // Lưu danh sách vào tệp

            return; // Trả về
        }

        public bool Load() // Hàm tải danh sách điểm cao
        {
            m_lstHighScores.Clear(); // Xóa danh sách điểm cao hiện tại
            if (File.Exists(HighScoresPath)) // Kiểm tra xem tệp có tồn tại không
            {
                string[] lines = System.IO.File.ReadAllLines(@HighScoresPath); // Đọc tất cả dòng từ tệp

                // Có thể có từ 0 đến n điểm cao trong tệp, nếu ai đó đã chỉnh sửa...
                for (int nIndex = 0; nIndex < lines.Length; nIndex++) // Lặp qua tất cả các dòng
                {
                    // Giới hạn danh sách điểm cao ở số lượng tối đa đã định nghĩa
                    if (nIndex >= MAXHIGHSCORES)
                        break; // Dừng nếu đã đạt số lượng tối đa

                    HighScore hs = new HighScore(); // Tạo đối tượng HighScore mới
                    hs.Initials = lines[nIndex].Substring(0, 3); // Lấy chữ cái đầu tiên
                    hs.Score = System.Convert.ToInt32(lines[nIndex].Substring(3, lines[nIndex].Length - 3)); // Lấy điểm số
                    m_lstHighScores.Add(hs); // Thêm vào danh sách điểm cao
                }
            }

            return true; // Trả về true khi tải thành công
        }

        public bool isHighScore(int score) // Hàm kiểm tra xem điểm có phải là điểm cao không
        {
            bool bIshighScore = false; // Biến đánh dấu
            if (m_lstHighScores.Count < MAXHIGHSCORES) bIshighScore = true; // Nếu số điểm thấp hơn tối đa
            else // Nếu đã đạt số điểm tối đa
            {
                for (int nIndex = 0; nIndex < m_lstHighScores.Count; nIndex++) // Lặp qua danh sách điểm cao
                {
                    if (m_lstHighScores[nIndex].Score < score) // Nếu có điểm thấp hơn điểm hiện tại
                    {
                        bIshighScore = true; // Đánh dấu là điểm cao
                        break; // Thoát khỏi vòng lặp
                    }
                }
            }

            return bIshighScore; // Trả về kết quả
        }

        public void AddScore(HighScore hs) // Hàm thêm điểm cao
        {
            bool bIshighScore = false; // Biến đánh dấu
            int nInsertAt = 0; // Vị trí chèn
            for (int nIndex = 0; nIndex < m_lstHighScores.Count; nIndex++) // Lặp qua danh sách điểm cao
            {
                if (m_lstHighScores[nIndex].Score < hs.Score) // Nếu có điểm thấp hơn điểm hiện tại
                {
                    bIshighScore = true; // Đánh dấu là điểm cao
                    nInsertAt = nIndex; // Lưu vị trí chèn
                    break; // Thoát khỏi vòng lặp
                }
            }

            if (m_lstHighScores.Count < MAXHIGHSCORES) // Nếu số điểm thấp hơn tối đa
            {
                m_lstHighScores.Add(hs); // Thêm vào danh sách điểm cao
            }
            else if (bIshighScore == true) // Nếu đã đạt số điểm tối đa và có điểm cao hơn
            {
                m_lstHighScores.Insert(nInsertAt, hs); // Chèn vào vị trí thích hợp
                m_lstHighScores.RemoveAt(m_lstHighScores.Count - 1); // Xóa điểm thấp nhất
            }
            Save(); Load(); // Lưu và tải lại danh sách điểm cao
        }

        public int GetHighScore() // Hàm lấy điểm cao nhất
        {
            int nScore = 0; // Biến lưu điểm cao nhất
            if (m_lstHighScores.Count > 0) // Nếu có điểm cao
                nScore = m_lstHighScores.OrderByDescending(hs => hs.Score).ToList()[0].Score; // Lấy điểm cao nhất

            return nScore; // Trả về điểm cao nhất
        }
    }

    public class HighScore // Định nghĩa lớp HighScore
    {
        private int m_nCurInitialIndex = 0; // Chỉ số hiện tại của chữ cái đầu tiên
        private string m_sInitials = "A__"; // Chữ cái đầu tiên
        public string Initials // Thuộc tính chữ cái đầu tiên
        {
            get { return m_sInitials; } // Lấy chữ cái đầu tiên
            set { m_sInitials = value; } // Đặt chữ cái đầu tiên
        }
        private int m_nScore; // Điểm số
        public int Score // Thuộc tính điểm số
        {
            get { return m_nScore; } // Lấy điểm số
            set { m_nScore = value; } // Đặt điểm số
        }

        public HighScore() // Constructor của lớp HighScore
        {
        }
    }

    public class EnterHighScoreScreen // Định nghĩa lớp EnterHighScoreScreen
    {
        public frmAsteroids canvas; // Biến lưu màn hình chơi game
        private int m_CurIndex = 0; // Chỉ số hiện tại của chữ cái đầu tiên
        public bool m_bDisplay = false; // Biến đánh dấu hiển thị màn hình nhập điểm cao
        public HighScore newHighScore = new HighScore(); // Đối tượng HighScore mới

        public bool IsDone // Thuộc tính kiểm tra xem đã nhập xong điểm cao chưa
        {
            get { return m_CurIndex > 2; } // Trả về true nếu đã nhập xong
        }

        public EnterHighScoreScreen(frmAsteroids frm) // Constructor của lớp EnterHighScoreScreen
        {
            canvas = frm; // Đặt màn hình chơi game
            newHighScore.Initials = "A__"; // Đặt chữ cái đầu tiên
        }

        public void Draw() // Hàm vẽ màn hình nhập điểm cao
        {
            if (m_bDisplay) // Nếu hiển thị màn hình nhập điểm cao
            {
                // ASTEROIDS
                AsteroidsFontHandler afh = new AsteroidsFontHandler(canvas); // Tạo đối tượng AsteroidsFontHandler
                System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White); // Tạo đối tượng SolidBrush

                int topPosOfBottomRowOfHelpText = 485; // Vị trí trên cùng của dòng trợ giúp cuối cùng

                // Tính toán vị trí trái của dòng trợ giúp cuối cùng
                afh.FontSize = 32.0f; afh.Kerning = 10;
                afh.Text = "PUSH HYPERSPACE WHEN LETTER IS CORRECT";
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2), topPosOfBottomRowOfHelpText);
                afh.Draw();

                int left = afh.TextLeft;
                afh.Text = "PUSH ROTATE TO SELECT LETTER";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                afh.Text = "PLEASE ENTER YOUR INITIALS";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                afh.Text = "YOUR SCORE IS ONE OF THE TEN BEST";
                afh.TextPosition = new Point(left, topPosOfBottomRowOfHelpText -= 50);
                afh.Draw();

                // Vẽ khu vực nhập điểm cao
                afh.FontSize = 50.0f; afh.Kerning = 20;
                afh.Text = newHighScore.Initials;
                afh.TextPosition = new Point(((canvas.Width - afh.TextWidth) / 2) - 28, canvas.Height - 215);
                afh.Draw();
            }
        }

        public bool ProcessKey(Keys key) // Hàm xử lý phím nhập
        {
            bool bProcessed = false; // Biến đánh dấu
            if (!IsDone) // Nếu chưa nhập xong điểm cao
            {
                char cur = newHighScore.Initials[m_CurIndex]; // Lấy chữ cái hiện tại
                char[] allInitials = newHighScore.Initials.ToCharArray(); // Lấy tất cả các chữ cái
                string allChars = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // Tất cả các chữ cái có thể nhập
                int indexofCurChar = allChars.IndexOf(cur); // Tìm vị trí của chữ cái hiện tại

                switch (key) // Xử lý phím nhập
                {
                    case Keys.Left:
                        if (indexofCurChar == 0)
                            indexofCurChar = allChars.Length;
                        cur = allChars.ToCharArray()[indexofCurChar - 1];
                        allInitials[m_CurIndex] = cur;
                        newHighScore.Initials = string.Join("", allInitials);
                        bProcessed = true;
                        break;
                    case Keys.Right:
                        if (indexofCurChar == allChars.Length - 1)
                            indexofCurChar = -1;
                        cur = allChars.ToCharArray()[indexofCurChar + 1];
                        allInitials[m_CurIndex] = cur;
                        newHighScore.Initials = string.Join("", allInitials);
                        bProcessed = true;
                        break;
                    case Keys.Down:
                        m_CurIndex += 1;
                        bProcessed = true;
                        break;
                }
            }

            return bProcessed; // Trả về kết quả
        }
    }
}