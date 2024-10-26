using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.Drawing; // Thư viện cho đồ họa
using System.Linq; // Thư viện cho LINQ
using System.Numerics; // Thư viện cho số học phức
using System.Text; // Thư viện cho xử lý chuỗi
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ
using System.Windows.Forms; // Thư viện cho Windows Forms

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    class Ship // Định nghĩa lớp Ship
    {
        private frmAsteroids canvas; // Biến canvas để lưu trữ tham chiếu đến form chính
        public float m_myAngle = 0.0f; // Góc hiện tại của tàu
        public float m_accelerationAngle = 0.0f; // Góc tăng tốc
        public float rotationAngle; // Góc quay
        public double velocity = 0.0f; // Vận tốc hiện tại

        // Tốc độ tăng tốc và giảm tốc
        public const double ACCELERATIONSPEED = 0.015f; // Tốc độ tăng tốc
        public const double DECELERATIONSPEED = 0.0002f; // Tốc độ giảm tốc
        public const double MAXVELOCITY = 0.8f; // Vận tốc tối đa
        public const float ROTSPEED = ((float)Math.PI / 90.0f) * 2.8f; // Tốc độ quay
        public const int RADIUS = 0; // Bán kính của tàu

        private bool m_bAccelerating; // Trạng thái tăng tốc

        private bool m_bIsRotating; // Trạng thái quay
        private Keys m_keyDepressed; // Phím đang được nhấn

        public Point position = new Point(); // Vị trí của tàu
        public bool m_bIsActive = false; // Trạng thái hoạt động của tàu
        public bool bIsHyperSpace = false; // Trạng thái không gian siêu

        private long m_DestructionAnimationTickStart; // Thời gian bắt đầu hoạt ảnh phá hủy
        private bool m_bPlayerIsDestroyed = false; // Trạng thái tàu bị phá hủy

        public bool bPlayerIsDestroyed // Thuộc tính kiểm tra trạng thái tàu bị phá hủy
        {
            get { return m_bPlayerIsDestroyed; } // Lấy trạng thái
            set
            {
                if (value) // Nếu tàu bị phá hủy
                    m_DestructionAnimationTickStart = DateTime.Now.Ticks; // Ghi lại thời gian bắt đầu hoạt ảnh phá hủy

                m_bPlayerIsDestroyed = value; // Gán trạng thái
            }
        }

        public Ship(frmAsteroids frm) // Constructor của lớp Ship
        {
            canvas = frm; // Gán form chính vào biến canvas
            resetToCenter(); // Đặt tàu về giữa màn hình
        }

        public void resetToCenter() // Hàm đặt tàu về giữa màn hình
        {
            position.X = (canvas.Width - 15) / 2; // Tính toán vị trí X
            position.Y = (canvas.Height + 25) / 2; // Tính toán vị trí Y
        }

        public void Draw() // Hàm vẽ tàu
        {
            if (bIsHyperSpace) // Nếu đang ở không gian siêu
                return; // Không vẽ gì, không có vật lý trong không gian siêu

            if (m_bIsRotating) // Nếu đang quay
            {
                if (m_keyDepressed == Keys.Left) // Nếu phím trái được nhấn
                    m_accelerationAngle -= ((m_accelerationAngle > 0.0f) ? ROTSPEED : -((float)(Math.PI * 2.0f) - ROTSPEED)); // Giảm góc tăng tốc
                else // Nếu không phải phím trái
                    m_accelerationAngle += ((m_accelerationAngle < (Math.PI * 2.0f)) ? ROTSPEED : -(m_accelerationAngle - ROTSPEED)); // Tăng góc tăng tốc

                rotationAngle = m_accelerationAngle; // Cập nhật góc quay
            }

            Pen pen = new Pen(Color.White); // Tạo bút vẽ màu trắng
            if (m_bPlayerIsDestroyed) // Nếu tàu bị phá hủy
            {
                int offset = (int)((DateTime.Now.Ticks - m_DestructionAnimation TickStart) / 500000); // Tính toán offset cho hoạt ảnh phá hủy

                // Vẽ hoạt ảnh phá hủy
                if (offset < 26)
                    canvas.g.DrawLine(pen,
                                        new Point(position.X - 7,
                                                  position.Y - 9 - offset),
                                        new Point(position.X + 9,
                                                  position.Y - 6 - offset));

                if (offset < 32)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 2 + (int)(Math.Sin(Math.PI / 3) * offset),
                                              position.Y - 9 - (int)(Math.Cos(Math.PI / 3) * offset)),
                                    new Point(position.X + 15 + (int)(Math.Sin(Math.PI / 3) * offset),
                                              position.Y - (int)(Math.Cos(Math.PI / 3) * offset)));

                float sa = (float)Math.Sin(Math.PI / 2);
                float ca = (float)Math.Cos(Math.PI / 2);

                if (offset < 38)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X + 7 + (int)(sa * offset),
                                              position.Y + (int)(ca * offset)),
                                    new Point(position.X + 15 + (int)(sa * offset),
                                              position.Y + 10 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 30)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * offset),
                                              position.Y + 7 + (int)(ca * offset)),
                                    new Point(position.X + 10 + (int)(sa * offset),
                                              position.Y + 4 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 12)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 7 - (int)(sa * offset),
                                              position.Y + 2 + (int)(ca * offset)),
                                    new Point(position.X - (int)(sa * offset),
                                              position.Y + 8 + (int)(ca * offset)));

                sa = (float)Math.Sin(Math.PI / 6);
                ca = (float)Math.Cos(Math.PI / 6);

                if (offset < 50)
                    canvas.g.DrawLine(pen,
                                    new Point(position.X - 3 - (int)(sa * offset),
                                              position.Y - 3 - (int)(ca * offset)),
                                    new Point(position.X - 10 - (int)(sa * offset),
                                              position.Y + 9 - (int)(ca * offset)));
            }
            else // Nếu không bị phá hủy
            {
                float sa = (float)Math.Sin(m_accelerationAngle);
                float ca = (float)Math.Cos(m_accelerationAngle);

                // Vẽ tàu
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * 13),
                                              position.Y - (int)(ca * 13)),
                                    new Point(position.X + (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) + (int)(sa * 10)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) + (int)(sa * 10)),
                                    new Point(position.X + (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) + (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) + (int)(sa * 4)),
                                    new Point(position.X - (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) - (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X - (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) - (int)(sa * 10)),
                                    new Point(position.X - (int)(ca * 4) - (int)(sa * 12),
                                              position.Y + (int)(ca * 12) - (int)(sa * 4)));
                canvas.g.DrawLine(pen,
                                    new Point(position.X + (int)(sa * 13),
                                              position.Y - (int)(ca * 13)),
                                    new Point(position.X - (int)(ca * 10) - (int)(sa * 17),
                                              position.Y + (int)(ca * 17) - (int)(sa * 10)));

                // Vẽ luồng phản lực nếu đang tăng tốc
                if (m_bAccelerating)
                {
                    canvas.g.DrawLine(pen,
                                        new Point(position.X - (int)(ca * 4) - (int)(sa * 14),
                                                  position.Y + (int)(ca * 14) - (int)(sa * 4)),
                                        new Point(position.X - (int)(sa * 23),
                                                  position.Y + (int)(ca * 23)));
                    canvas.g.DrawLine(pen,
                                        new Point(position.X + (int)(ca * 4) - (int)(sa * 14),
                                                  position.Y + (int)(ca * 14) + (int)(sa * 4)),
                                        new Point(position.X - (int)(sa * 23),
                                                  position.Y + (int)(ca * 23)));
                }

                doPhysics(); // Thực hiện vật lý
            }
        }

        public void Rotate(Keys key, bool bIsRotating) // Hàm quay tàu
        {
            m_bIsRotating = bIsRotating; // Gán trạng thái quay
            m_keyDepressed = key; // Gán phím đang được nhấn
        }

        public void Accelerate(Keys key, bool bAccelerate) // Hàm tăng tốc tàu
        {
            m_bAccelerating = bAccelerate; // Gán trạng thái tăng tốc
            //m_accelerationAngle = m_myAngle; // Cập nhật góc tăng tốc
        }

        private void doPhysics() // Hàm thực hiện vật lý
        {
            // Bước 1: Tính toán gia tốc mới, không thay đổi hướng
            if (!m_bAccelerating) // Nếu không tăng tốc
            {
                if (velocity > 0.0f) // Nếu vận tốc lớn hơn 0
                    velocity -= DECELERATIONSPEED; // Giảm vận tốc
                else // Nếu vận tốc bằng 0
                    velocity = 0.0f; // Đặt vận tốc bằng 0
            }
            // Bước 2: Tính toán gia tốc thay đổi, tăng tốc dương
            // Tăng vận tốc từ trạng thái dừng
            else if (velocity == 0.0f) // Nếu vận tốc bằng 0
            {
                velocity += ACCELERATIONSPEED; // Tăng vận tốc
                m_myAngle = m_accelerationAngle; // Cập nhật góc hiện tại
            }
            // Tăng vận tốc liên tục, cùng hướng
            else if (((velocity + ACCELERATIONSPEED) < MAXVELOCITY) // Nếu vận tốc cộng với gia tốc nhỏ hơn vận tốc tối đa
                      && (m_myAngle == m_accelerationAngle)) // Và góc hiện tại bằng góc tăng tốc
            {
                velocity += ACCELERATIONSPEED; // Tăng vận tốc
            }
            else // Tăng vận tốc, thay đổi hướng
            {
                // Tính toán vị trí mới dựa trên hướng tăng tốc
                double vfCX = velocity * Math.Cos(m_myAngle);
                double vfCY = velocity * Math.Sin(m_myAngle);
                double vfNX = ACCELERATIONSPEED * Math.Cos(m_accelerationAngle);
                double vfNY = ACCELERATIONSPEED * Math.Sin(m_accelerationAngle);
                // Đảm bảo gia tốc mới không vượt quá vận tốc tối đa
                if ((Math.Sqrt(Math.Pow(vfCX + vfNX, 2) + Math.Pow(vfCY + vfNY, 2)) < MAXVELOCITY))
                    velocity = Math.Sqrt(Math.Pow(vfCX + vfNX, 2) + Math.Pow(vfCY + vfNY, 2)); // Cập nhật vận tốc

                // Tính toán góc mới
                m_myAngle = (float)Math.Atan2((vfCY + vfNY), (vfCX + vfNX)); // Cập nhật góc hiện tại
            }

            float sa = (float)Math.Sin(m_myAngle);
            float ca = (float)Math.Cos(m_myAngle);

            // Cập nhật vị trí tàu
            position = new Point(position.X + (int)(sa * velocity * 15),
                                 position.Y - (int)(ca * velocity * 15));
            if (position.X < 0) // Nếu vị trí X nhỏ hơn 0
                position = new Point(canvas.Width + position.X, position.Y); // Đặt vị trí X bằng chiều rộng màn hình cộng với vị trí X
            else if (position.X > canvas.Width) // Nếu vị trí X lớn hơn chiều rộng màn hình
                position = new Point(position.X - canvas.Width, position.Y); // Đặt vị trí X bằng vị trí X trừ đi chiều rộng màn hình
            if (position.Y < 0) // Nếu vị trí Y nhỏ hơn 0
                position = new Point(position.X, canvas.Height + position.Y); // Đặt vị trí Y bằng chiều cao màn hình cộng với vị trí Y
            else if (position.Y > canvas.Height) // Nếu vị trí Y lớn hơn chiều cao màn hình
                position = new Point(position.X, position.Y - canvas.Height); // Đặt vị trí Y bằng vị trí Y trừ đi chiều cao màn hình

            /*canvas.DEBUGOUTPUT = String.Format("ROT={0:0.00},ACC={1:0.00},AA={2:0.00}",
                                                ((m_myAngle * 180.0f) / Math.PI),
                                                m_Acceleration,
                                                ((m_accelerationAngle * 180.0f) / Math.PI) ); */
        }

        public bool doesObjectCollide(Point p, float objectradius) // Hàm kiểm tra va chạm
        {
            float checkVal = (float)Math.Sqrt(Math.Pow(position.X - p.X, 2) + Math.Pow(position.Y - p.Y, 2));
            if (checkVal < (objectradius + RADIUS + 10)) // Nếu khoảng cách nhỏ hơn bán kính cộng với 10
                return true; // Trả về true
            else if (objectradius < 10) // Nếu bán kính nhỏ hơn 10
                throw new Exception("SHOULD NEVER BE HERE!"); // Ném ra ngoại lệ

            return false; // Trả về false
        }
    }
}