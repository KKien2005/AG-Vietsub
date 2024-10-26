using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS
{
    class AsteroidsFontHandler
    {
        private frmAsteroids canvas; // Khung vẽ để hiển thị văn bản

        private int m_nTextWidth; // Chiều rộng của văn bản
        public int TextWidth { get { return m_nTextWidth; } } // Thuộc tính để lấy chiều rộng văn bản
        private int m_nTextHeight; // Chiều cao của văn bản
        public int TextHeight { get { return m_nTextHeight; } } // Thuộc tính để lấy chiều cao văn bản
        private int m_nTextLeft; // Vị trí bên trái của văn bản
        public int TextLeft { get { return m_nTextLeft; } } // Thuộc tính để lấy vị trí bên trái của văn bản
        private int m_nTextTop; // Vị trí phía trên của văn bản
        public int TextTop { get { return m_nTextTop; } } // Thuộc tính để lấy vị trí phía trên của văn bản

        private int m_nKerning; // Khoảng cách giữa các ký tự
        public int Kerning { get { return m_nKerning; } set { m_nKerning = value; } } // Thuộc tính để lấy hoặc thiết lập khoảng cách giữa các ký tự

        public Point TextPosition // Vị trí của văn bản
        {
            set { m_nTextLeft = value.X; m_nTextTop = value.Y; } // Thiết lập vị trí của văn bản
        }
        private string m_sText; // Văn bản cần hiển thị

        private int m_nCharSize; // Kích thước ký tự
        public string Text // Thuộc tính để lấy hoặc thiết lập văn bản
        {
            get { return m_sText; }
            set
            {
                m_sText = value; // Thiết lập văn bản
                m_nTextWidth = (int)((m_nCharSize + m_nKerning) * m_sText.Length); // Tính chiều rộng văn bản dựa trên kích thước ký tự và khoảng cách
            }
        }

        private float m_fFontSize; // Kích thước phông chữ
        public float FontSize // Thuộc tính để lấy hoặc thiết lập kích thước phông chữ
        {
            get { return m_fFontSize; }
            set
            {
                m_fFontSize = value; // Thiết lập kích thước phông chữ
                m_nCharSize = (int)(0.7f * m_fFontSize); // Tính kích thước ký tự dựa trên kích thước phông chữ
            }
        }

        public AsteroidsFontHandler(frmAsteroids frm) // Constructor để khởi tạo đối tượng với khung vẽ
        {
            canvas = frm; // Gán khung vẽ
        }

        private List<Point> m_secondaryPoints = new List<Point>(); // Danh sách các điểm phụ, chỉ sử dụng cho ký hiệu bản quyền hoặc các đường không tách rời
        public void Draw() // Phương thức để vẽ văn bản
        {
            Pen p = new Pen(Color.White); // Tạo bút với màu trắng
            var ca = m_sText.ToCharArray(); // Chuyển đổi văn bản thành mảng ký tự

            // Vòng lặp qua từng ký tự trong văn bản
            for (int x = 0; x < ca.Length; x++)
            {
                char s = ca[x]; // Lấy ký tự hiện tại
                if (s != '.' && s != ' ') // Nếu ký tự không phải là dấu chấm hoặc khoảng trắng
                {
                    canvas.g.DrawLines(p, GetPoints(s, x)); // Vẽ các đường cho ký tự
                    if (m_secondaryPoints.Count > 0) // Nếu có điểm phụ
                    {
                        canvas.g.DrawLines(p, m_secondaryPoints.ToArray()); // Vẽ các đường cho điểm phụ
                        m_secondaryPoints.Clear(); // Xóa danh sách điểm phụ
                    }
                }
                else if (s == '.') // Nếu ký tự là dấu chấm
                {
                    calcOffsetsAndStoreInSecondaryPoints(x); // Tính toán và lưu trữ điểm phụ
                    canvas.g.FillRectangle(p.Brush, m_secondaryPoints[0].X, m_secondaryPoints[0].Y, 1, 1); // Vẽ hình chữ nhật cho dấu chấm
                    m_secondaryPoints.Clear(); // Xóa danh sách điểm phụ
                }
            }
            return; // Kết thúc phương thức
        }

        private void calcOffsetsAndStoreInSecondaryPoints(int ox) // Phương thức để tính toán và lưu trữ điểm phụ
        {
            int lx = ox * (m_nCharSize + m_nKerning); // Tính toán vị trí của điểm phụ
            int fs = (int)FontSize; // Lấy kích thước phông chữ

            m_secondaryPoints.Add(new Point(m_nTextLeft + lx, m_nTextTop + fs)); // Thêm điểm phụ vào danh sách
        }

        private Point[] GetPoints(char cur, int ox) // Phương thức để lấy các điểm cho ký tự
        {
            List<Point> points = new List<Point>(); // Tạo danh sách các điểm
            int lx = ox * (m_nCharSize + m_nKerning); // Tính toán vị trí của điểm
            int fs = (int)FontSize; // Lấy kích thước phông chữ

            // Xử lý các ký tự khác nhau
            switch (cur)
            {
                // Các ký tự A-Z, a-z, 0-9, và các ký hiệu đặc biệt
                case 'A':
                case 'a':
                    // Thêm các điểm cho ký tự A/a
                    points.AddRange(new[]
                    {
                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/3)),
                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((2*fs)/3)),
                        new Point(m_nTextLeft + lx, m_nTextTop + ((2*fs)/3))
                    });
                    break;
                    
                    // Tương tự thêm các kí tự khác

                case 'B':
                case 'b':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((2*fs)/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + ((5*fs)/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/2))

                                    });
                    break;

                case 'C':
                case 'c':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'D':
                case 'd':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +(fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +(2*fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop)
                                    });
                    break;

                case 'E':
                case 'e':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4) , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });

                    break;
                case 'F':
                case 'f':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4) , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'G':
                case 'g':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3))
                                    });
                    break;

                case 'H':
                case 'h':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs/2),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + fs)
                                    });
                    break;

                case 'I':
                case 'i':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop +fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop +fs)
                                    });
                    break;


                case 'J':
                case 'j':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (2*fs/3))
                                    });
                    break;

                case 'K':
                case 'k':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'L':
                case 'l':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize , m_nTextTop + fs)
                                    });
                    break;

                case 'M':
                case 'm':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;


                case 'N':
                case 'n':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'P':
                case 'p':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2))
                                    });
                    break;
                case 'Q':
                case 'q':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6))
                                    });
                    break;
                case 'R':
                case 'r':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case 'S':
                case 's':
                case '5':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                    });
                    break;
                case 'T':
                case 't':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs)
                                    });
                    break;

                case 'U':
                case 'u':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'V':
                case 'v':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'W':
                case 'w':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (2*fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                                    });
                    break;

                case 'X':
                case 'x':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs)
                                    });
                    break;

                case 'Y':
                case 'y':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + (fs/3)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs)
                                    });
                    break;

                case 'Z':
                case 'z':
                    points.AddRange(new[]
                                        {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                // NUMBERS
                case '0':
                case 'O':
                case 'o':
                    points.AddRange(new[]
                                    {   new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop)
                                    });
                    break;

                case '1':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/2), m_nTextTop + fs),
                                    });
                    break;

                case '2':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                    });
                    break;

                case '3':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                    });
                    break;

                case '4':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case '6':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2))
                                    });
                    break;

                case '7':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;

                case '8':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                    });
                    break;

                case '9':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/2)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/2))
                                    });
                    break;

                case '©':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx, m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop)
                                    });

                    m_secondaryPoints.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + (fs/6)),
                                        new Point(m_nTextLeft + lx + (m_nCharSize/4), m_nTextTop + (5*fs/6)),
                                        new Point(m_nTextLeft + lx + (3*m_nCharSize/4), m_nTextTop + (5*fs/6))
                                    });
                    break;

                case '_':
                    points.AddRange(new[]
                                    {
                                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs)
                                    });
                    break;
                default:
                    // Thêm các điểm cho ký tự mặc định
                    points.AddRange(new[]
                    {
                        new Point(m_nTextLeft + lx, m_nTextTop),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx, m_nTextTop),
                        new Point(m_nTextLeft + lx+m_nCharSize, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx, m_nTextTop + fs),
                        new Point(m_nTextLeft + lx + m_nCharSize, m_nTextTop)
                    });
                    break;
            }
            return points.ToArray(); // Trả về các điểm cho ký tự
        }
    }
}