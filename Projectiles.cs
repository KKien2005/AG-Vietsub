using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.Drawing; // Thư viện cho đồ họa
using System.Linq; // Thư viện cho LINQ
using System.Text; // Thư viện cho xử lý chuỗi
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    class Projectiles // Định nghĩa lớp Projectiles
    {
        public List<Projectile> list = new List<Projectile>(); // Danh sách các viên đạn
        private frmAsteroids canvas; // Tham chiếu đến form chính
        private long lastTick = 0; // Thời gian của lần vẽ cuối

        public Projectiles(frmAsteroids frm) // Constructor của lớp Projectiles
        {
            canvas = frm; // Gán form chính vào biến canvas
            lastTick = System.DateTime.Now.Ticks; // Đặt thời gian của lần vẽ cuối
        }

        public void Draw() // Hàm vẽ các viên đạn
        {
            foreach (Projectile p in list) // Lặp qua từng viên đạn trong danh sách
            {
                float sa = (float)Math.Sin(p.firingAngle - (Math.PI / 2)); // Tính toán sin của góc bắn
                float ca = (float)Math.Cos(p.firingAngle - (Math.PI / 2)); // Tính toán cos của góc bắn
                float psa = (float)Math.Sin(p.angleOfShipMomentum - (Math.PI / 2)); // Tính toán sin của góc động lực của tàu
                float pca = (float)Math.Cos(p.angleOfShipMomentum - (Math.PI / 2)); // Tính toán cos của góc động lực của tàu
                double sox = psa * p.shipVelocity; // Tính toán vận tốc theo trục X
                double soy = pca * p.shipVelocity; // Tính toán vận tốc theo trục Y

                Brush curBrush = (Brush)Brushes.White; // Tạo bút vẽ màu trắng

                // Tính toán vị trí mới
                Point position = new Point(p.position.X + (int)(ca * Projectile.SPEED), // Tính toán vị trí X
                                           p.position.Y + (int)(sa * Projectile.SPEED)); // Tính toán vị trí Y
                // Kiểm tra và điều chỉnh vị trí nếu ra ngoài màn hình
                if (position.X < 0)
                    position = new Point(canvas.Width + position.X, position.Y); // Nếu ra ngoài bên trái
                else if (position.X > canvas.Width)
                    position = new Point(position.X - canvas.Width, position.Y); // Nếu ra ngoài bên phải
                if (position.Y < 0)
                    position = new Point(position.X, canvas.Height + position.Y); // Nếu ra ngoài trên
                else if (position.Y > canvas.Height)
                    position = new Point(position.X, position.Y - canvas.Height); // Nếu ra ngoài dưới

                // Vẽ viên đạn
                canvas.g.FillRectangle(curBrush, position.X, position.Y, 2, 2); // Vẽ hình chữ nhật nhỏ đại diện cho viên đạn

                // Cập nhật thông tin viên đạn
                p.position = position; // Cập nhật vị trí viên đạn
                p.decay += System.DateTime.Now.Ticks - lastTick; // Cập nhật thời gian tồn tại của viên đạn
            }

            lastTick = System.DateTime.Now.Ticks; // Cập nhật thời gian của lần vẽ cuối

            // Xóa các viên đạn đã hết thời gian tồn tại
            if (list.Count > 0) // Nếu có viên đạn trong danh sách
            {
                for (int nCur = list.Count; nCur > 0; nCur--) // Lặp ngược qua danh sách viên đạn
                {
                    if ((list[nCur - 1].decay / 10000000) > Projectile.DECAYTIME) // Nếu thời gian tồn tại lớn hơn thời gian tối đa
                        list.RemoveAt(nCur - 1); // Xóa viên đạn
                }
            }
        }

        private long ticksSinceLastFire = 0; // Thời gian từ lần bắn cuối
        public void Fire(Ship player, Point value, float curAngle, double av) // Hàm bắn viên đạn
        {
            long curTicks = System.DateTime.Now.Ticks; // Lấy thời gian hiện tại
            // Tính toán thời gian để giới hạn tốc độ bắn viên đ ạn
            if (list.Count < 4 && ((curTicks - ticksSinceLastFire) > 1000000)) // Nếu số viên đạn trong danh sách nhỏ hơn 4 và thời gian từ lần bắn cuối lớn hơn 1/10 giây
            {
                ticksSinceLastFire = curTicks; // Cập nhật thời gian từ lần bắn cuối
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = value, angulerVelocity = av, shipVelocity = player.velocity, angleOfShipMomentum = player.m_myAngle }); // Tạo viên đạn mới
            }
        }

        public void RandomFire(Point origin) // Hàm bắn viên đạn ngẫu nhiên
        {
            long curTicks = System.DateTime.Now.Ticks; // Lấy thời gian hiện tại
            float curAngle = (float)canvas.randomizer.Next(1, 360); // Tạo góc bắn ngẫu nhiên
            float av = (float)canvas.randomizer.Next(1, 360); // Tạo vận tốc góc ngẫu nhiên

            // Tính toán thời gian để giới hạn tốc độ bắn viên đạn
            if (list.Count < 4 && ((curTicks - ticksSinceLastFire) > 1000000)) // Nếu số viên đạn trong danh sách nhỏ hơn 4 và thời gian từ lần bắn cuối lớn hơn 1/10 giây
            {
                ticksSinceLastFire = curTicks; // Cập nhật thời gian từ lần bắn cuối
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = origin, angulerVelocity = av }); // Tạo viên đạn mới
            }
        }

        public void NotSoRandomFire(Point origin, float angleOfPlayerFromUFO) // Hàm bắn viên đạn không ngẫu nhiên
        {
            long curTicks = System.DateTime.Now.Ticks; // Lấy thời gian hiện tại
            float curAngle = angleOfPlayerFromUFO, av = curAngle; // Tạo góc bắn và vận tốc góc dựa trên góc của người chơi

            // Tính toán thời gian để giới hạn tốc độ bắn viên đạn
            if (list.Count < 4 && ((curTicks - ticksSinceLastFire) > 1000000)) // Nếu số viên đạn trong danh sách nhỏ hơn 4 và thời gian từ lần bắn cuối lớn hơn 1/10 giây
            {
                ticksSinceLastFire = curTicks; // Cập nhật thời gian từ lần bắn cuối
                list.Add(new Projectile() { decay = 0.0f, firingAngle = curAngle, position = origin, angulerVelocity = av }); // Tạo viên đạn mới
            }
        }
    }

    public class Projectile // Định nghĩa lớp Projectile
    {
        public const float DECAYTIME = 1.2f; // Thời gian tồn tại tối đa của viên đạn
        public const float SPEED = 19.0f; // Vận tốc của viên đạn

        public float decay = 0.0f; // Thời gian tồn tại của viên đạn
        public float firingAngle = 0.0f; // Góc bắn của viên đạn
        public double angulerVelocity = 0.0f; // Vận tốc góc của viên đạn
        public Point position = new Point(0, 0); // Vị trí của viên đạn

        // Thông tin động lực của tàu
        public double shipVelocity = 0.0f; // Vận tốc của tàu
        public float angleOfShipMomentum = 0.0f; // Góc động lực của tàu
    }
}