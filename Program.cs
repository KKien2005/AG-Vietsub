using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.Linq; // Thư viện cho LINQ
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ
using System.Windows.Forms; // Thư viện cho Windows Forms

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    static class Program // Định nghĩa lớp tĩnh Program
    {
        /// <summary>
        /// Điểm vào chính của ứng dụng.
        /// </summary>
        [STAThread] // Chỉ định rằng ứng dụng sẽ sử dụng mô hình luồng đơn
        static void Main() // Hàm Main, điểm vào chính của ứng dụng
        {
            Application.EnableVisualStyles(); // Bật kiểu giao diện người dùng hiện đại
            Application.SetCompatibleTextRenderingDefault(false); // Đặt chế độ render văn bản tương thích
            Application.Run(new frmAsteroids()); // Chạy form frmAsteroids
        }
    }
}