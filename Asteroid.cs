using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class Asteroid
    {
        //Vị trí của asteroid
        public Point Position;
        //Danh sách các điểm tạo thành hình dạng cho asteroid
        public List<Point> points = new List<Point>();
        //Tốc độ di chuyển của thiên thạch
        public float velocity;
        //Khung vẽ (canvas) để hiển thị asteroid
        private frmAsteroids canvas;
        
        //Góc di chuyển hiện tại của asteroid
        public float m_fMoveAngle;

        // Size constants - hằng số kích thước 
        private const float NUMPOINTS = 11.0f;
        private const float LA_WIDTHLARGE = 55;
        private const float MA_WIDTHLARGE = 28;
        private const float SA_WIDTHLARGE = 13;

        // Speed constants - hằng số tốc độ 
        private const int SLOWSPEED = 4;
        private const int FASTSPEEDBIG = 5;
        private const int FASTSPEEDMEDIUM = 6;
        private const int FASTSPEEDSMALL = 7;

        //Enum để xác định kích thước của asteroid
        public enum SIZEOFASTEROID
        {
            SMALL = 0,
            MEDIUM = 1,
            LARGE = 2
        } ;

        //Giá trị điểm cho mỗi loại asteroid
        public List<int> ASTEROIDPOINTVALUES = new List<int>() { 100, 50, 20 };

        //Kích thước của asteroid hiện tại
        public SIZEOFASTEROID mySize;
        //Bán kính của asteroid
        public float myRadius = 0.0f;

        //Trạng thái của asteroid
        public bool bDestroyed = false;
        public bool bPlayerCollided = false;

        //Vị trí va chạm
        public Point collisionLocation;
        //Hoạt ảnh phá hủy
        public Animations destructionAnimation = new Animations( Animations.ANIMTYPE.GENERAL );
        //Bán kính tối đa cho hoạt ảnh 
        public float MAXANIMRADIUS = LA_WIDTHLARGE;

        //Hàm khởi tạo cho lớp Asteroid
        public Asteroid(frmAsteroids frm, SIZEOFASTEROID asteroidSize, Point? p = null)
        {
            int fs; //Tốc dộ tối đa
            canvas = frm; //Gán canvas
            mySize = asteroidSize; //Gán kích thước cho asteroid

            //Khởi tạo danh sách kích thước asteroid nếu chưa được khởi tạo
            if (asteroidDimensions.Count == 0)
                InitializeAsteroidDimensionsList();

            //Bán tốc độ tối đa dựa trên kích thước asteroid
            if (mySize == SIZEOFASTEROID.SMALL)
                fs = FASTSPEEDSMALL;
            else if (mySize == SIZEOFASTEROID.MEDIUM)
                fs = FASTSPEEDMEDIUM;
            else
                fs = FASTSPEEDBIG;

            //Nếu không có vị trí được chỉ định, tạo vị trí ngẫu nhiên cho asteroid
            if (p == null)
                Position = new Point(canvas.randomizer.Next((int)LA_WIDTHLARGE, canvas.Width - (int)LA_WIDTHLARGE),
                                    canvas.randomizer.Next((int)LA_WIDTHLARGE, canvas.Height - (int)LA_WIDTHLARGE));
            else
                Position = (Point) p;

            //Tạo hình dạng cho asteroid
            Generate(mySize);
            // Gán tốc độ ngẫu nhiên trong khoảng từ SLOWSPEED đến tốc độ tối đa
            velocity = canvas.randomizer.Next(SLOWSPEED, fs);
            // Gán góc di chuyển ngẫu nhiên từ 0 đến 360 độ
            m_fMoveAngle = ((float)canvas.randomizer.Next(0, 360));
        }

        // Danh sách chứa các kích thước của asteroid
        private static List<List<Vector2>> asteroidDimensions = 
                                        new List<List<Vector2>>();

        //Hàm khởi tạo danh sách kích thước asteroid
        private void InitializeAsteroidDimensionsList()
        {
            List<Vector2> lCur = new List<Vector2>();

            // Big Asteroid #1 - Kích thước cho asteroid lớn #1
            lCur.Add(new Vector2(0.4f, 5.0f));//1
            lCur.Add(new Vector2(0.9f, 30.0f));
            lCur.Add(new Vector2(1.0f, 68.0f));
            lCur.Add(new Vector2(0.65f, 89.0f));
            lCur.Add(new Vector2(1.0f, 113.0f));
            lCur.Add(new Vector2(0.8f, 160.0f));//6
            lCur.Add(new Vector2(0.85f, 208.0f));
            lCur.Add(new Vector2(0.9f, 246.0f));
            lCur.Add(new Vector2(0.95f, 295.0f));
            lCur.Add(new Vector2(0.87f, 333.0f));//10
            asteroidDimensions.Add(lCur); // Thêm kích thước vào danh sách

            // Big Asteroid #2 - Kích thước cho asteroid lớn #2
            lCur = new List<Vector2>();
            lCur.Add(new Vector2(0.9f, 26.0f));//1
            lCur.Add(new Vector2(0.95f, 65.0f));
            lCur.Add(new Vector2(0.45f, 65.0f));
            lCur.Add(new Vector2(0.92f, 103.0f));
            lCur.Add(new Vector2(0.92f, 153.0f));
            lCur.Add(new Vector2(0.7f, 202.0f));//6
            lCur.Add(new Vector2(0.95f, 210.0f));
            lCur.Add(new Vector2(1.0f, 247.0f));
            lCur.Add(new Vector2(1.0f, 293.0f));
            lCur.Add(new Vector2(0.95f, 331.0f));
            lCur.Add(new Vector2(0.6f, 354.0f));//11
            asteroidDimensions.Add(lCur); // Thêm kích thước vào danh sách

            // Big Asteroid #3 - Kích thước cho asteroid lớn #3
            lCur = new List<Vector2>();
            lCur.Add(new Vector2(0.7f, 19.0f));//1
            lCur.Add(new Vector2(0.92f, 72.0f));
            lCur.Add(new Vector2(0.88f, 84.0f));
            lCur.Add(new Vector2(0.2f, 111.0f));
            lCur.Add(new Vector2(1.0f, 117.0f));
            lCur.Add(new Vector2(0.96f, 152.0f));//6
            lCur.Add(new Vector2(0.65f, 160.0f));
            lCur.Add(new Vector2(0.96f, 205.0f));
            lCur.Add(new Vector2(0.88f, 252.0f));
            lCur.Add(new Vector2(0.89f, 290.0f));
            lCur.Add(new Vector2(0.35f, 329.0f));
            lCur.Add(new Vector2(0.75f, 330.0f));//12
            asteroidDimensions.Add(lCur); // Thêm kích thước vào danh sách
        }

        //Hàm vẽ asteroid lên canvas 
        public void Draw()
        {
            Pen penColor = new Pen(Color.White); // Màu bút vẽ là trắng
            if (!bDestroyed) // Nếu asteroid chưa bị hủy
            {
                for (int nPoint = 0; nPoint < points.Count; nPoint++)
                {
                    Point from, to; // Điểm bắt đầu và kết thúc để vẽ đường
                    if (nPoint < (points.Count - 1))
                    {
                        // Vẽ giữa hai điểm liên tiếp
                        from = new Point(Position.X + points[nPoint].X, Position.Y + points[nPoint].Y);
                        to = new Point(Position.X + points[nPoint + 1].X, Position.Y + points[nPoint + 1].Y);
                    }
                    else
                    {
                        // Vẽ từ điểm cuối về điểm đầu để tạo hình khép kín
                        from = new Point(Position.X + points[nPoint].X, Position.Y + points[nPoint].Y);
                        to = new Point(Position.X + points[0].X, Position.Y + points[0].Y);
                    }
                    // Vẽ đường giữa hai điểm 
                    canvas.g.DrawLine(penColor, from, to);
                }
            }
            else // Nếu asteroid đã bị phá hủy
            {
                Brush curBrush = (Brush)Brushes.White; // Màu để vẽ hoạt ảnh phá hủy 
                List<Point> daPoints = new List<Point>(); // Danh sách các điểm để vẽ hoạt ảnh
                foreach (Vector2 vec in destructionAnimation.sequence() )
                {
                    float ND = -(float)((float)Math.PI / 2.0f);// Điều chỉnh góc để vẽ
                    // Tính toán tạo độ x và y cho hoạt ảnh phá hủy 
                    int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius );
                    int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * destructionAnimation.radius);

                    // Vẽ hình vuông nhỏ tại ví trị va chạm
                    canvas.g.FillRectangle(curBrush, collisionLocation.X + nX, collisionLocation.Y + nY , 2, 2);
                    //daPoints.Add(new Point(nX, nY));
                }
            }
        }

        // Hàm di chuyển asteroid
        public void Move()
        {
            // Cập nhật vị trí của asteroid theo tốc độ và di chuyển 
            this.Position.X += (int)(Math.Sin((m_fMoveAngle * Math.PI) / 180.0f) * velocity);
            this.Position.Y += (int)(Math.Cos((m_fMoveAngle * Math.PI) / 180.0f) * velocity);

            // Kiểm tra nếu asteroid ra ngoài biên của canvas và điều chỉnh lại vị trí
            if (this.Position.X > canvas.Width)
                this.Position.X = this.Position.X - canvas.Width; // Quay lại từ bên trái
            else if (this.Position.Y > canvas.Height)
                this.Position.Y = this.Position.Y - canvas.Height;// Quay lại từ trên xuống
            else if (this.Position.X < 0)
                this.Position.X = canvas.Width + this.Position.X;// Quay lại từ bên phải 
            else if (this.Position.Y < 0)
                this.Position.Y = canvas.Height  + this.Position.Y;// Quay lại từ dưới lên
        }

        // Hàm tạo hình dạng cho asteroid dựa trên kích thích
        private void Generate( SIZEOFASTEROID asteroidSize )
        {
            float rotationAngle = (float)(Math.PI * 2.0f) / (float)(NUMPOINTS);
            float curAngle = 0; // Khởi tạo góc hiện tại 

            // Gán bán kính cho asteroid dựa trên kích thước
            switch (asteroidSize)
            {
                case SIZEOFASTEROID.LARGE:
                    myRadius = LA_WIDTHLARGE; // Bán kính cho asteroid lên
                    break;
                case SIZEOFASTEROID.MEDIUM:
                    myRadius = MA_WIDTHLARGE; // Bán kính cho asteroid vừa
                    break;
                case SIZEOFASTEROID.SMALL:
                    myRadius = SA_WIDTHLARGE; // Bán kính cho asteroid nhỏ
                    break;
            }

            // Lấy ngẫu nhiên một kích thích asteroid từ danh sách 
            int nCurAsteroid = canvas.randomizer.Next(0, asteroidDimensions.Count);
            foreach (Vector2 vec in asteroidDimensions[nCurAsteroid])
            {
                float ND = -(float)((float)Math.PI / 2.0f); // Điều chỉnh góc
                // Tính toán tọa độ x và y cho các điểm của asteroid 
                int nX = (int)(Math.Cos((vec.Y / 180) * Math.PI + ND) * vec.X * myRadius);
                int nY = (int)(Math.Sin((vec.Y / 180) * Math.PI + ND) * vec.X * myRadius);
                points.Add(new Point(nX, nY)); // Thêm điểm vào danh sách
            }
        }

        // HÀm kiểm tra và chạm với 1 điểm 
        public bool doesPointCollide( Point p )
        {
            // Tính khoảng cách từ vị trí của asteroid
            float checkVal = (float)Math.Sqrt(Math.Pow(Position.X - p.X, 2) + Math.Pow(Position.Y - p.Y, 2));

            // myRadius + X, where X is the fudge factor leniency for a hit, extending the radius in this case 
            // by 8 pixels to  accomodate for higher speed projectile calculation misses. This won't catch 
            // everything that should register as a hit, but does a lot better than requiring a direct hit 
            // for a projectile that moves right through (and past) the target without registering a hit because
            // for one frame it's on one side of the target and the next it's on the other. 
            // Kiểm tra điểm P và tâm asteroid có nhỏ hơn tổng bán kính hay không
            if (checkVal < ( myRadius + 2)) 
            {
                collisionLocation = p;// Ghi lại vị trí va chạm
                return true; // Trả về true nếu có va chạm 
            }

            return false; // Trả về false nếu không có va chạm 
        }

        // Hàm tạo vận tốc ngẫu nhiên cho asteroid mới
        public void newPseudoRandomVelocity( Asteroid a )
        {
            // Đặt giới hạn dưới cho vận tốc
            float lb = (a.velocity - 5 > SLOWSPEED) ? a.velocity - 5 : SLOWSPEED;
            float ub;
            // Xác định giới hạn trên dựa trên kích thước của asteroid 
            if (mySize == SIZEOFASTEROID.MEDIUM)
                ub = (a.velocity + 5 > FASTSPEEDMEDIUM) ? a.velocity + 5 : FASTSPEEDMEDIUM;
            else
                ub = (a.velocity + 5 > FASTSPEEDSMALL) ? a.velocity + 5 : FASTSPEEDSMALL;

            // Tạo vận tốc ngẫu nhiên trong khoảng (lb, ub)
            velocity = canvas.randomizer.Next((int)lb, (int)ub);
        }
    }
}
