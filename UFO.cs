using System; // Thư viện chứa các kiểu dữ liệu cơ bản và các phương thức hỗ trợ
using System.Collections.Generic; // Thư viện hỗ trợ các cấu trúc dữ liệu như List
using System.Drawing; // Thư viện hỗ trợ vẽ đồ họa
using System.Linq; // Thư viện hỗ trợ các phương thức LINQ
using System.Numerics; // Thư viện hỗ trợ các phép toán vector
using System.Text; // Thư viện hỗ trợ xử lý chuỗi
using System.Threading.Tasks; // Thư viện hỗ trợ lập trình bất đồng bộ
using System.Windows.Forms; // Thư viện hỗ trợ xây dựng giao diện Windows Forms

namespace ASTEROIDS // Tên không gian tên của dự án
{
    class UFO // Định nghĩa lớp UFO
    {
        private frmAsteroids canvas; // Biến lưu trữ đối tượng canvas (khung vẽ)

        private bool m_bLeftToRight; // Biến xác định hướng di chuyển của UFO
        public float m_myAngle = 0.0f; // Góc di chuyển của UFO

        // Tốc độ di chuyển
        public const float SMALLSPEED = 5.0f; // Tốc độ nhỏ
        public const float LARGESPEED = 4.5f; // Tốc độ lớn hơn nhưng chậm hơn
        private const long TICKSPERSECOND = 10000000; // Số tick trong một giây
        public const float MAXVELOCITY = 0.8f; // Tốc độ tối đa
        public const int RADIUS = 27; // Bán kính tối đa để vẽ từ tâm UFO
        public const int XMARGIN = 200; // Khu vực chết ngăn chặn thay đổi hướng

        public Point collisionLocation; // Vị trí va chạm
        public Animations destructionAnimation = new Animations(Animations.ANIMTYPE.GENERAL); // Hoạt ảnh hủy diệt của UFO

        private const int MAXDIRECTIONALCHANGES = 5; // Số lần thay đổi hướng tối đa
        private const long MININTERVALFORDIRECTIONALCHANGE = TICKSPERSECOND * 2; // Khoảng thời gian tối thiểu giữa các lần thay đổi hướng
        private int m_nDirectionalChanges = 0; // Số lần đã thay đổi hướng
        private const float PROBABILITYOFDIRECTIONCHANGESTART = 5.0f; // Xác suất thay đổi hướng ban đầu
        private const float PROBABILITYOFDIRECTIONCHANGEINCREASEPERTICK = 0.1f; // Tăng xác suất thay đổi hướng theo từng tick

        private float m_fProbabilityOfDirectionChange = PROBABILITYOFDIRECTIONCHANGESTART; // Xác suất hiện tại của việc thay đổi hướng
        private long nextPossibleDirectionalChange; // Thời gian tiếp theo có thể thay đổi hướng

        private float m_sa; // Sin của góc di chuyển
        private float m_ca; // Cosin của góc di chuyển
        private float m_nForAngularSpeedAdjustmentHack = 90.0f; // Biến hack để điều chỉnh tốc độ góc

        public enum UFOSIZE // Enum để xác định kích thước UFO
        {
            LARGE = 0, // Kích thước lớn
            SMALL = 1 // Kích thước nhỏ
        }

        public UFOSIZE m_ufoType = UFOSIZE.SMALL; // Loại UFO hiện tại

        public Point position = new Point(); // Vị trí hiện tại của UFO
        private bool m_bIsActive = false; // Biến xác định UFO có đang hoạt động hay không
        public bool IsActive
        {
            get { return m_bIsActive; } // Getter cho trạng thái hoạt động
            set { m_bIsActive = value; } // Setter cho trạng thái hoạt động
        }

        public bool bIsHyperSpace = false; // Biến xác định UFO có đang ở không gian siêu tốc hay không

        private long m_DestructionAnimationTickStart; // Thời gian bắt đầu hoạt ảnh hủy diệt
        private bool m_bUFOIsDestroyed = false; // Biến xác định UFO có bị hủy diệt hay không
        public bool bUFOIsDestroyed
        {
            get { return m_bUFOIsDestroyed; } // Getter cho trạng thái hủy diệt
            set
            {
                if (value)
                    m_DestructionAnimationTickStart = DateTime.Now.Ticks; // Ghi lại thời gian khi UFO bị hủy diệt

                m_bUFOIsDestroyed = value; // Cập nhật trạng thái hủy diệt
            }
        }

        // Constructor khởi tạo UFO với canvas
        public UFO(frmAsteroids frm)
        {
            canvas = frm; // Gán canvas
        }

        // Phương thức vẽ UFO lên canvas
        public void Draw()
        {
            Pen pen = new Pen(Color.White); // Tạo bút vẽ màu trắng

            if (m_bUFOIsDestroyed) // Nếu UFO bị hủy diệt
            {
                if (destructionAnimation.radius < RADIUS) // Nếu bán kính của hoạt ảnh nhỏ hơn bán kính tối đa
                    destructionAnimation.radius += 2.25f; // Tăng bán kính của hoạt ảnh
                else
                {
                    IsActive = false; // UFO không hoạt động nữa
                    m_bUFOIsDestroyed = false; // Reset trạng thái hủy diệt
                }

                Brush curBrush = (Brush)Brushes.White; // Tạo brush màu trắng
                List<Point> daPoints = new List<Point>(); // Danh sách các điểm vẽ
                foreach (Vector2 vec in destructionAnimation.sequence()) // Duyệt qua các vector của hoạt ảnh
                {
                    float ND = -(float)((float)Math.PI / 2.0f); // Góc quay
                    int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius); // Tính toán tọa độ X
                    int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius); // Tính toán tọa độ Y

                    canvas.g.FillRectangle(curBrush, collisionLocation.X + nX, collisionLocation.Y + nY, 2, 2); // Vẽ hình chữ nhật tại vị trí va chạm
                }
            }
            else if (m_bIsActive) // Nếu UFO đang hoạt động
            {
                bool bShouldIChangeDirection = (canvas.randomizer.Next(1, (int)m_fProbabilityOfDirectionChange) == 1) ? true : false; // Xác định xem có nên thay đổi hướng hay không
                // Xác suất thay đổi hướng được tính toán dựa trên số lần thay đổi hướng và thời gian đã trôi qua

                if (bShouldIChangeDirection)
                    m_fProbabilityOfDirectionChange = PROBABILITYOFDIRECTIONCHANGESTART; // Reset xác suất thay đổi hướng
                else if (m_fProbabilityOfDirectionChange > 2)
                    m_fProbabilityOfDirectionChange -= PROBABILITYOFDIRECTIONCHANGEINCREASEPERTICK; // Giảm xác suất thay đổi hướng

                // Chỉ thay đổi hướng nếu thời gian đã trôi qua đủ lâu và số lần thay đổi hướng chưa đạt tối đa
                if ((nextPossibleDirectionalChange < DateTime.Now.Ticks) &&
                     (bShouldIChangeDirection) &&
                     (m_nDirectionalChanges < MAXDIRECTIONALCHANGES))
                {
                    m_nDirectionalChanges += 1; // Tăng số lần thay đổi hướng

                    // Tính toán góc di chuyển mới
                    int nDirection = canvas.randomizer.Next(1, 4);
                weirdShitWithRandomNumbers:

                    if (nDirection < 1 || nDirection > 3) goto weirdShitWithRandomNumbers;

                    m_nForAngularSpeedAdjustmentHack = (45 * (int)nDirection);

                    m_myAngle = ((m_bLeftToRight) ? 1 : -1) * (float)((float)m_nForAngularSpeedAdjustmentHack * (Math.PI / 180.0f));

                    m_sa = (float)Math.Sin(m_myAngle);
                    m_ca = (float)Math.Cos(m_myAngle);

                    nextPossibleDirectionalChange = DateTime.Now.Ticks + MININTERVALFORDIRECTIONALCHANGE; // Cập nhật thời gian tiếp theo có thể thay đổi hướng
                }

                // Nếu UFO đang di chuyển đến cạnh màn hình, buộc nó quay lại
                if (((position.X + XMARGIN) > canvas.Width) && m_bLeftToRight ||
                    ((position.X - XMARGIN) < 0) && !m_bLeftToRight &&
                    (Math.Abs(m_myAngle) != 90.0f))
                {
                    m_myAngle = ((m_bLeftToRight) ? 1 : -1) * (float)((float)90.0f * (Math.PI / 180.0f));

                    m_sa = (float)Math.Sin(m_myAngle);
                    m_ca = (float)Math.Cos(m_myAngle);
                }

                // Cập nhật vị trí của UFO
                position.X += (int)((float)((m_bLeftToRight) ? 1 : -1) * ((m_ufoType == UFOSIZE.LARGE) ? LARGESPEED : SMALLSPEED));
                position.Y += (int)(m_ca * ((float)((m_ufoType == UFOSIZE.LARGE) ? LARGESPEED : SMALLSPEED)));

                if (position.Y < 0) position.Y = canvas.Height + position.Y;
                if (position.Y > canvas.Height) position.Y = position.Y - canvas.Height;

                if (position.X - RADIUS > canvas.Width || position.X + RADIUS < 0)
                {
                    m_bIsActive = false; // UFO không hoạt động nữa
                    canvas.onUFOExit(); // Gọi sự kiện khi UFO rời khỏi màn hình
                }
                else
                {
                    List<Point> points = new List<Point>(); // Danh sách các điểm vẽ

                    int px = position.X;
                    int py = position.Y;

                    if (m_ufoType == UFOSIZE.LARGE) // Nếu UFO có kích thước lớn
                    {
                        points.AddRange(new[]
                                            {
                                        new Point(px + 27, py),
                                        new Point(px + 10, py-9),
                                        new Point(px + 5, py-17),
                                        new Point(px - 5, py-17),
                                        new Point(px - 10, py-9),
                                        new Point(px + 10, py-9),
                                        new Point(px - 10, py-9),
                                        new Point(px - 27, py),
                                        new Point(px + 27, py),
                                        new Point(px + 10, py +9),
                                        new Point(px - 10, py +9),
                                        new Point(px - 27, py),
                                    });
                    }
                    else // Nếu UFO có kích thước nhỏ
                    {
                        points.AddRange(new[]
                                            {
                                        new Point(px + 27/2, py),
                                        new Point(px + 10/2, py-9/2),
                                        new Point(px + 5/2, py-17/2),
                                        new Point(px - 5/2, py-17/2),
                                        new Point(px - 10/2, py-9/2),
                                        new Point(px + 10/2, py-9/2),
                                        new Point(px - 10/2, py-9/2),
                                        new Point(px - 27/2, py),
                                        new Point(px + 27/2, py),
                                        new Point(px + 10/2, py +9/2),
                                        new Point(px - 10/2, py +9/2),
                                        new Point(px - 27/2, py),
                                    });
                    }

                    canvas.g.DrawLines(pen, points.ToArray()); // Vẽ UFO lên canvas
                }
                // doPhysics(); // Thực hiện các phép tính vật lý
            }
        }

        private void doPhysics()
        {

        }

        // Phương thức spawn UFO mới
        public void spawnUFO(int level, long levelTime)
        {
            if (!m_bIsActive) // Chỉ spawn UFO mới khi UFO cũ đã bị hủy diệt
            {
                // Xác định loại UFO dựa trên cấp độ và thời gian chơi
                m_ufoType = (((levelTime / TICKSPERSECOND) * level) < 45) ? UFOSIZE.LARGE : UFOSIZE.SMALL;

                // Xác định hướng di chuyển của UFO
                m_bLeftToRight = (canvas.randomizer.Next(1, 100) < 50) ? true : false;

                // Tính toán vị trí ban đầu của UFO
                position = new Point((m_bLeftToRight) ? -RADIUS : canvas.Width + RADIUS, RADIUS + canvas.randomizer.Next(canvas.Height - (2 * RADIUS)));

                nextPossibleDirectionalChange = DateTime.Now.Ticks + MININTERVALFORDIRECTIONALCHANGE;
                m_nDirectionalChanges = 0;

                // Tính toán góc di chuyển ban đầu
                m_myAngle = ((m_bLeftToRight) ? 1 : -1) * (float)(Math.PI / 2);

                // Cập nhật sin và cos của góc di chuyển
                m_sa = (float)Math.Sin(m_myAngle);
                m_ca = (float)Math.Cos(m_myAngle);

                // UFO đã sẵn sàng hoạt động
                m_bIsActive = true;
            }
        }

        // Phương thức kiểm tra va chạm
        public bool doesObjectCollide(Point p, float objectradius)
        {
            float checkVal = (float)Math.Sqrt(Math.Pow(position.X - p.X, 2) + Math.Pow(position.Y - p.Y, 2));
            if (checkVal < objectradius)
            {
                collisionLocation = position;
                destructionAnimation.radius = 0.0f;
                return true;
            }

            return false;
        }

        // Phương thức kích hoạt hoạt ảnh hủy diệt
        public void triggerCollisionSequence()
        {
            collisionLocation = position;
            destructionAnimation.radius = 0.0f;
            m_bIsActive = false;
            m_bUFOIsDestroyed = true;
        }
    }
}