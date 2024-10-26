// ------------------------------------------------------------------------------------------------------
// Sounds.CS
//
// Âm thanh được xác thực bởi trình tạo tệp WAVE này, sau khi được tạo ra bằng mã FileStream
// https://indiehd.com/auxiliary/flac-validator/progress/?X-Progress-ID=606ebbf2c76df
// 
// Cách tiếp cận lớp âm thanh để hình thành luồng tệp WAVE được khai thác từ bài viết này:
// https://www.codeguru.com/columns/dotnet/making-sounds-with-waves-using-c.html
//
// Cuối cùng, cách tiếp cận hàm sóng vuông để xây dựng mẫu âm thanh trong bộ đệm được lấy từ câu trả lời của câu hỏi 
// trên Stack Overflow:
// https://stackoverflow.com/questions/6168954/playing-sound-byte-in-c-sharp
// 
// }
// ------------------------------------------------------------------------------------------------------
using System; // Thư viện hệ thống
using System.Collections.Generic; // Thư viện cho danh sách và từ điển
using System.IO; // Thư viện cho thao tác tệp
using System.Linq; // Thư viện cho LINQ
using System.Text; // Thư viện cho xử lý chuỗi
using System.Threading.Tasks; // Thư viện cho tác vụ bất đồng bộ
using System.Media; // Thư viện cho âm thanh
using System.Windows.Media; // Thư viện cho âm thanh trong WPF
using System.Runtime.InteropServices; // Thư viện cho gọi hàm bên ngoài
using System.Threading; // Thư viện cho đa luồng
//using Microsoft.DirectX.DirectSound; // Thư viện DirectX cho âm thanh (đã bị chú thích)

namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    public class Sounds // Định nghĩa lớp Sounds
    {
        private frmAsteroids canvas; // Biến canvas để lưu trữ tham chiếu đến form chính
        [DllImport("winmm.dll")] // Nhập khẩu hàm từ thư viện winmm.dll
        static extern Int32 mciSendString(string command, IntPtr alwaysNull, int bufferSize, IntPtr hwndCallback); // Định nghĩa hàm gửi lệnh đến MCI

        // Định nghĩa các loại âm thanh
        public enum SOUNDS
        {
            PURETONE = 0, // Âm thanh tần số thuần
            BGMUSICLOW = 1, // Nhạc nền thấp
            BGMUSICHIGH = 2, // Nhạc nền cao
            THRUST = 3, // Âm thanh đẩy
            PROJECTILE = 4, // Âm thanh đạn
            DESTROYEDASTEROIDSMALL = 5, // Âm thanh tiểu hành tinh bị phá hủy
            DESTROYEDASTEROIDMEDIUM = 6, // Âm thanh hành tinh trung bị phá hủy
            DESTROYEDASTEROIDLARGE = 7, // Âm thanh hành tinh lớn bị phá hủy
            SPACESHIPLARGE = 8, // Âm thanh tàu vũ trụ lớn
            SPACESHIPSMALL = 9 // Âm thanh tàu vũ trụ nhỏ
        }

        public Dictionary<SOUNDS, FX> library = new Dictionary<SOUNDS, FX>(); // Từ điển để lưu trữ âm thanh

        public Sounds(frmAsteroids rootForm) // Constructor của lớp Sounds
        {
            canvas = rootForm; // Gán form chính vào biến canvas
            //FileStream fs = new FileStream("SHIT2.WAV", FileMode.OpenOrCreate); // Tạo luồng tệp (đã bị chú thích)
            //fs.Write(generateAudio(SOUNDS.PURETONE), 0, generateAudio(SOUNDS.PURETONE).Length); // Ghi âm thanh vào tệp (đã bị chú thích)
            //fs.Close(); // Đóng luồng tệp (đã bị chú thích)

            // Thêm âm thanh vào thư viện
            library.Add(SOUNDS.PURETONE, generateAudio(SOUNDS.PURETONE));
            library.Add(SOUNDS.BGMUSICLOW, generateAudio(SOUNDS.BGMUSICLOW));
            library.Add(SOUNDS.BGMUSICHIGH, generateAudio(SOUNDS.BGMUSICHIGH));
            library.Add(SOUNDS.THRUST, generateAudio(SOUNDS.THRUST));
            library.Add(SOUNDS.PROJECTILE, generateAudio(SOUNDS.PROJECTILE));
            library.Add(SOUNDS.DESTROYEDASTEROIDSMALL, generateAudio(SOUNDS.DESTROYEDASTEROIDSMALL));
            library.Add(SOUNDS.DESTROYEDASTEROIDMEDIUM, generateAudio(SOUNDS.DESTROYEDASTEROIDMEDIUM));
            library.Add(SOUNDS.DESTROYEDASTEROIDLARGE, generateAudio(SOUNDS.DESTROYEDASTEROIDLARGE));
            library.Add(SOUNDS.SPACESHIPLARGE, generateAudio(SOUNDS.SPACESHIPLARGE));
            library.Add(SOUNDS.SPACESHIPSMALL, generateAudio(SOUNDS.SPACESHIPSMALL));
        }

        ~Sounds() // Destructor của lớp Sounds
        {
            mciSendString("close all", IntPtr.Zero, 0, IntPtr.Zero); // Đóng tất cả các âm thanh
        }

        private FX generateAudio(SOUNDS curSnd) // Hàm tạo âm thanh
        {
            string sFileName = ""; // Tên tệp âm thanh
            byte[] buff = null; // Bộ đệm để lưu trữ âm thanh
            List<short> ssData = new List<short>(); // Danh sách để lưu trữ dữ liệu âm thanh

            switch (curSnd) // Chọn loại âm thanh
            {
                case SOUNDS.PURETONE:
                    ssData.AddRange(ShapeSoundSquare(255, 1000, 20000)); // Tạo âm thanh tần số thuần
                    sFileName = "PURETONE.WAV"; // Tên tệp âm thanh
                    break;
                case SOUNDS.BGMUSICLOW:
                    ssData.AddRange(ShapeSoundSquare(81, 100, 2500)); // Tạo nhạc nền thấp
                    sFileName = "BGMUSICLOW.WAV"; // Tên tệp âm thanh
                    break;
                case SOUNDS.BGMUSICHIGH:
                    ssData.AddRange(ShapeSoundSquare(87, 100, 2500)); // Tạo nhạc nền cao
                    sFileName = "BGMUSICHIGH.WAV"; // Tên tệp âm thanh
                    break;
                case SOUNDS.THRUST:
                    ssData.AddRange(ShapeSoundThrust()); // Tạo âm thanh đẩy
                    sFileName = "THRUST.WAV"; // Tên tệp âm thanh
                    break;
                case SOUNDS.PROJECTILE:
                    buff = new byte[AsteroidsResources.Projectile.Length]; // Tạo bộ đệm cho âm thanh đạn
                    AsteroidsResources.Projectile.Read(buff, 0, (int)AsteroidsResources.Projectile.Length); // Đọc âm thanh đạn từ tài nguyên
                    sFileName = "Projectile.wav"; // Tên tệp âm thanh
                    break;
                case SOUNDS.DESTROYEDASTEROIDSMALL:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidSmall.Length]; // Tạo bộ đệm cho âm thanh tiểu hành tinh bị phá hủy
                    AsteroidsResources.DestroyedAsteroidSmall.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidSmall.Length); // Đọc âm thanh tiểu hành tinh bị phá hủy từ tài nguyên
                    sFileName = "DestroyedAsteroidSmall.wav"; // Tên tệp âm thanh
                    break;
                case SOUNDS.DESTROYEDASTEROIDMEDIUM:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidMedium.Length]; // Tạo bộ đệm cho âm thanh hành tinh trung bị phá hủy
                    AsteroidsResources.DestroyedAsteroidMedium.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidMedium.Length); // Đọc âm thanh hành tinh trung bị phá hủy từ tài nguyên
                    sFileName = "DestroyedAsteroidMedium.wav"; // Tên tệp âm thanh
                    break;
                case SOUNDS.DESTROYEDASTEROIDLARGE:
                    buff = new byte[AsteroidsResources.DestroyedAsteroidLarge.Length]; // Tạo bộ đệm cho âm thanh hành tinh lớn bị phá hủy
                    AsteroidsResources.DestroyedAsteroidLarge.Read(buff, 0, (int)AsteroidsResources.DestroyedAsteroidLarge.Length); // Đọc âm thanh hành tinh lớn bị phá hủy từ tài nguyên
                    sFileName = "DestroyedAsteroidLarge.wav"; // Tên tệp âm thanh
                    break;
                case SOUNDS.SPACESHIPLARGE:
                    buff = new byte[AsteroidsResources.SpaceShipLarge.Length]; // Tạo bộ đệm cho âm thanh tàu vũ trụ lớn
                    AsteroidsResources.SpaceShipLarge.Read(buff, 0, (int)AsteroidsResources.SpaceShipLarge.Length); // Đọc âm thanh tàu vũ trụ lớn từ tài nguyên
                    sFileName = "SpaceShipLarge.wav"; // Tên tệp âm thanh
                    break;
                case SOUNDS.SPACESHIPSMALL:
                    buff = new byte[AsteroidsResources.SpaceShipSmall.Length]; // Tạo bộ đệm cho âm thanh tàu vũ trụ nhỏ
                    AsteroidsResources.SpaceShipSmall.Read(buff, 0, (int)AsteroidsResources.SpaceShipSmall.Length); // Đọc âm thanh tàu vũ trụ nhỏ từ tài nguyên
                    sFileName = "SpaceShipSmall.wav"; // Tên tệp âm thanh
                    break;
            }

            List<byte> data = new List<byte>(); // Danh sách để lưu trữ dữ liệu âm thanh
            WaveHeader header = new WaveHeader(); // Header của tệp âm thanh
            FormatChunk format = new FormatChunk(); // Định dạng của tệp âm thanh
            DataChunk dc = new DataChunk(); // Bộ đệm dữ liệu của tệp âm thanh
            List<byte> tempBytes = new List<byte>(); // Danh sách để lưu trữ dữ liệu tạm thời

            dc.AddSampleDataStereo(ssData.ToArray(), ssData.ToArray()); // Thêm dữ liệu âm thanh vào bộ đệm
            header.FileLength += format.Length() + dc.Length(); // Tính toán độ dài của tệp âm thanh
            tempBytes.AddRange(header.GetBytes()); // Thêm header vào danh sách tạm thời
            tempBytes.AddRange(format.GetBytes()); // Thêm định dạng vào danh sách tạm thời
            tempBytes.AddRange(dc.GetBytes()); // Thêm bộ đệm dữ liệu vào danh sách tạm thời

            string sFullPath = createWaveOutput(curSnd, sFileName, (buff == null) ? tempBytes.ToArray() : buff); // Tạo tệp âm thanh

            return new FX(canvas.Handle, curSnd, sFullPath); // Tạo đối tượng âm thanh
        }

        private string createWaveOutput(Sounds.SOUNDS curSnd, string sFileName, byte[] data) // Hàm tạo tệp âm thanh
        {
            string sFULL = String.Format("{0}{1}", Path.GetTempPath(), sFileName); // Đường dẫn đầy đủ của tệp âm thanh
            if (!File.Exists(sFULL)) // Nếu tệp âm thanh chưa tồn tại
                File.WriteAllBytes(sFULL, data); // Tạo tệp âm thanh

            return sFULL; // Trả về đường dẫn đầy đủ của tệp âm thanh
        }

        public List<short> ShapeSoundSquare(UInt16 frequency, int msDuration, UInt16 volume) // Hàm tạo âm thanh tần số thuần
        {
            List<short> data = new List<short>(); // Danh sách để lưu trữ dữ liệu âm thanh

            const double TAU = 2 * Math.PI; // Hằng số TAU
            int samplesPerSecond = 44100; // Số mẫu mỗi giây
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000); // Số mẫu cần thiết

            double theta = frequency * TAU / (double)samplesPerSecond; // Góc quay của sóng
            double amp = volume >> 2; // Độ lớn của sóng

            for (int step = 0; step < samples; step++) // Tạo sóng vuông
            {
                short s = (short)(amp * Math.Sin(theta * (double)step));
                data.Add(s);
            }

            return data; // Trả về danh sách dữ liệu âm thanh
        }

        public List<short> ShapeSoundThrust() // Hàm tạo âm thanh đẩy
        {
            UInt16 frequency = 92; // Tần số của sóng
            int msDuration = 500; // Thời gian của sóng
            List<short> data = new List<short>(); // Danh sách để lưu trữ dữ liệu âm thanh

            const double TAU = 2 * Math.PI; // Hằng số TAU
            int samplesPerSecond = 44100; // Số mẫu mỗi giây
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000); // Số mẫu cần thiết

            double theta = frequency * TAU / (double)samplesPerSecond; // Góc quay của sóng
            double amp = 2500; // Độ lớn của sóng

            Random r = new Random(); // Đối tượng ngẫu nhiên

            for (int step = 0; step < samples; step++) // Tạo sóng đẩy
            {
                if (step % 150 == 0) // Thay đổi độ lớn của sóng
                    amp = r.Next(200, 1000);

                short s = (short)(amp * Math.Sin(theta * (double)step));
                data.Add(s);
            }

            return data; // Trả về danh sách dữ liệu âm thanh
        }

        // TODO: Can't get this one working.... 
        // need to understand sound better... ARGHHHH
        public List<short> ShapeSoundProjectile() // Hàm tạo âm thanh đạn
        {
            int msDuration = 258; // Thời gian của sóng
            List<short> data = new List<short>(); // Danh sách để lưu trữ dữ liệu âm thanh

            const double TAU = 2 * Math.PI; // Hằng số TAU
            int samplesPerSecond = 44100; // Số mẫu mỗi giây
            int samples = (int)((decimal)samplesPerSecond * msDuration / 1000); // Số mẫu cần thiết

            double amp = 3000; // Độ lớn của sóng
            Random r = new Random(); // Đối tượng ngẫu nhiên

            double mult = 1.0; // Hệ số nhân
            double fShift = -0.6; // Hệ số dịch chuyển

            int stopBumpUpFreq = samples / 4; // Giới hạn tăng tần số
            int curSign = 1; // Ký hiệu của sóng
            int nToggleStep = 0; // Số bước toggle
            int currentStep = 0; // Số bước hiện tại
            bool bOnDecline = false; // Trạng thái giảm dần
            int noteshift = 0; // Số bước dịch chuyển
            int curModulusOperator = 19; // Số bước modulus
            float shiftLowEnd = 1.0f; // Hệ số dịch chuyển thấp

            for (int step = 0; step < samples; step++) // Tạo sóng đạn
            {
                if (step % 400 == 0) // Thay đổi hệ số nhân
                {
                    mult -= 0.015;
                    //amp = mult * r.Next(250, 1000);
                }
                if (stopBumpUpFreq > step && ((step % 40) == 0) && fShift <= 1.0) // Tăng tần số
                    fShift += 0.015;
                else if (stopBumpUpFreq > step && ((step % 40) == 0) && fShift <= 1.7) // Tăng tần số
                    fShift += 0.005;
                else if (((step % 10) == 0) && (fShift > 0.0)) // Giảm tần số
                {
                    bOnDecline = true;
                    fShift -= 0.0005;
                }
                else if ((fShift <= 0.0) && (bOnDecline == true)) // Reset tần số
                {
                    fShift = 0.0;
                    //if ( (step % 25) == 0 )
                    //    curModulusOperator += 1;
                }

                nToggleStep = ((step + 1) % curModulusOperator); // Tính toán số bước toggle
                if (nToggleStep == 0) // Toggle ký hiệu của sóng
                {
                    currentStep += 1;
                    if (currentStep == 3)
                    {
                        curSign = 1;
                        currentStep = 0;
                    }
                    else
                        curSign = -1;
                }

                short curNote = Convert.ToInt16((mult * amp * curSign)); // Tính toán giá trị của sóng
                if (curSign < 0 && bOnDecline) // Điều chỉnh giá trị của sóng
                {
                    //shiftLowEnd += 0.001f;
                    curNote = (short)((double)-200.0 * (double)shiftLowEnd); // Convert.ToInt16((shiftLowEnd)*(mult * amp * curSign) + (fShift * 10000));
                }

                data.Add(curNote); // Thêm giá trị của sóng vào danh sách
            }

            return data; // Trả về danh sách dữ liệu âm thanh
        }
    }

    public class FX // Định nghĩa lớp FX
    {
        [DllImport("winmm.dll")] // Nhập khẩu hàm từ thư viện winmm.dll
        static extern Int32 mciSendString(string command, IntPtr buffPtr, int bufferSize, IntPtr hwndCallback); // Định nghĩa hàm gửi lệnh đến MCI

        [DllImport("winmm.dll")] // Nhập khẩu hàm từ thư viện winmm.dll
        static extern Int32 mciSendString(string command, StringBuilder returnValue, int returnLength, IntPtr hwndCallback); // Định nghĩa hàm gửi lệnh đến MCI

        private bool m_bIsStopped = false; // Trạng thái dừng của âm thanh
        private Sounds.SOUNDS m_SoundType; // Loại âm thanh

        private Dictionary<Sounds.SOUNDS, string> m_soundDictionary = new Dictionary<Sounds.SOUNDS, string>(); // Từ điển để lưu trữ âm thanh
        private string m_sFileName; // Tên tệp âm thanh

        private string m_curAlias = ""; // Alias của âm thanh
        private IntPtr hwndCanvas; // Tham chiếu đến form chính

        private long medialengthInTicks; // Độ dài của âm thanh
        private long canFireNext; // Thời gian có thể phát âm thanh tiếp theo
        private string m_sFullOpenText; // Chuỗi lệnh mở âm thanh

        [System.Flags] // Định nghĩa các giá trị của enum
        public enum PlaySoundFlags : uint
        {
            SND_SYNC = 0x0000, // Phát âm thanh đồng bộ
            SND_ASYNC = 0x0001, // Phát âm thanh không đồng bộ
            SND_NODEFAULT = 0x0002, // Không phát âm thanh mặc định
            SND_MEMORY = 0x0004, // Phát âm thanh từ bộ nhớ
            SND_LOOP = 0x0008, // Phát âm thanh lặp lại
            SND_NOSTOP = 0x0010, // Không dừng âm thanh
            SND_NOWAIT = 0x00002000, // Không chờ âm thanh
            SND_FILENAME = 0x00020000, // Phát âm thanh từ tệp
            SND_RESOURCE = 0x00040004 // Phát âm thanh từ tài nguyên
        }

        public enum PlaySoundStatus : uint // Định nghĩa các trạng thái của âm thanh
        {
            CLOSED = 0, // Âm thanh đã đóng
            NOTREADY = 1, // Âm thanh chưa sẵn sàng
            PAUSED = 2, // Âm thanh đang tạm dừng
            PLAYING = 3, // Âm thanh đang phát
            STOPPED = 4 // Âm thanh đã dừng
        }

        public FX(IntPtr hwnd, Sounds.SOUNDS curSoundType, string fileName) // Constructor của lớp FX
        {
            hwndCanvas = hwnd; // Gán tham chiếu đến form chính
            m_sFileName = fileName; // Gán tên tệp âm thanh

            m_curAlias = getUniqueAlias(); // Tạo alias cho âm thanh
            m_sFullOpenText = String.Format("open {0} type waveaudio alias {1}", m_sFileName, m_curAlias); // Tạo chuỗi lệnh mở âm thanh
            mciSendString(m_sFullOpenText, IntPtr.Zero, 0, IntPtr.Zero); // Mở âm thanh

            StringBuilder strReturn = new StringBuilder(256, 256); // Tạo đối tượng StringBuilder
            int ret; // Biến để lưu trữ kết quả
            ret = mciSendString("status " + m_curAlias + " length", strReturn, strReturn.Capacity, IntPtr.Zero); // Lấy độ dài của âm thanh
            medialengthInTicks = new TimeSpan(0, 0, 0, 0, System.Convert.ToInt32(strReturn.ToString())).Ticks; // Tính toán độ dài của âm thanh
            canFireNext = System.DateTime.Now.Ticks; // Tính toán thời gian có thể phát âm thanh tiếp theo

            m_SoundType = curSoundType; // Gán loại âm thanh
            m_bIsStopped = true; // Đặt trạng thái dừng của âm thanh
        }

        private static int m_nUID; // Biến để lưu trữ ID duy nhất
        private static int getUID { get { m_nUID = (m_nUID < int.MaxValue) ? m_nUID + 1 : 0; return m_nUID; } } // Tạo ID duy nhất
        private string getUniqueAlias() { return string.Format("temp{0}", getUID); } // Tạo alias duy nhất

        public void Stop() // Hàm dừng âm thanh
        {
            canFireNext = System.DateTime.Now.Ticks; // Tính toán thời gian có thể phát âm thanh tiếp theo
            mciSendString(("stop " + m_curAlias), IntPtr.Zero, 0, IntPtr.Zero); // Dừng âm thanh
            mciSendString((String.Format("seek {0} to start", m_curAlias)), IntPtr.Zero, 0, IntPtr.Zero); // Đặt vị trí phát lại
            m_bIsStopped = true; // Đặt trạng thái dừng của âm thanh
            return;
        }

        public bool IsPlaying() { return canFireNext > System.DateTime.Now.Ticks; } // Hàm kiểm tra trạng thái phát của âm thanh

        public PlaySoundStatus getStatus() // Hàm lấy trạng thái của âm thanh
        {
            StringBuilder strReturn = new StringBuilder(256, 256); // Tạo đối tượng StringBuilder
            int ret; // Biến để lưu trữ kết quả
            ret = mciSendString("status " + m_curAlias + " mode", strReturn, strReturn.Capacity, IntPtr.Zero); // Lấy trạng thái của âm thanh
            switch (strReturn.ToString()) // Chuyển đổi trạng thái của âm thanh
            {
                case "stopped":
                    return PlaySoundStatus.STOPPED; // Âm thanh đã dừng
                case "playing":
                    return PlaySoundStatus.PLAYING; // Âm thanh đang phát
                case "paused":
                    return PlaySoundStatus.PAUSED; // Âm thanh đang tạm dừng
                case "not ready":
                    return PlaySoundStatus.NOTREADY; // Â m thanh chưa sẵn sàng
                default:
                    return PlaySoundStatus.CLOSED; // Âm thanh đã đóng
            }
        }

        private Thread myThread = null; // Đối tượng Thread
        private string m_sSeekString; // Chuỗi lệnh tìm kiếm
        private string m_sPlayData; // Chuỗi lệnh phát
        private List<Thread> m_Threads = new List<Thread>(10); // Danh sách đối tượng Thread

        public void Play() // Hàm phát âm thanh
        {
            if (myThread == null) // Nếu đối tượng Thread chưa được tạo
            {
                m_sSeekString = (String.Format("seek {0} to start", m_curAlias)); // Tạo chuỗi lệnh tìm kiếm
                m_sPlayData = (String.Format("play {0}", m_curAlias)); // Tạo chuỗi lệnh phát
                myThread = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData)); // Tạo đối tượng Thread
            }

            if (m_SoundType == Sounds.SOUNDS.PROJECTILE) // Nếu âm thanh là đạn
            {
                Thread cur = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData)); // Tạo đối tượng Thread
                cur.Start(); // Bắt đầu đối tượng Thread
                m_Threads.Add(cur); // Thêm đối tượng Thread vào danh sách
                for (int x = m_Threads.Count - 1; x >= 0; x--) // Xóa đối tượng Thread đã dừng
                {
                    if (m_Threads[x].ThreadState == ThreadState.Stopped)
                        m_Threads.RemoveAt(x);
                }
            }
            else if (!IsPlaying() && (myThread.ThreadState == ThreadState.Unstarted || myThread.ThreadState == ThreadState.Stopped)) // Nếu âm thanh chưa được phát và đối tượng Thread chưa được tạo hoặc đã dừng
            {
                myThread = new Thread(() => playSound(m_sFullOpenText, m_sSeekString, m_sPlayData)); // Tạo đối tượng Thread
                myThread.Start(); // Bắt đầu đối tượng Thread
                canFireNext = System.DateTime.Now.AddTicks(medialengthInTicks).Ticks; // Tính toán thời gian có thể phát âm thanh tiếp theo
            }
        }

        private static void playSound(string cmdOpenText, string cmdSeek, string cmdPlay) // Hàm phát âm thanh
        {
            mciSendString(cmdOpenText, IntPtr.Zero, 0, IntPtr.Zero); // Mở âm thanh
            mciSendString(cmdSeek, IntPtr.Zero, 0, IntPtr.Zero); // Tìm kiếm âm thanh
            mciSendString(cmdPlay, IntPtr.Zero, 0, IntPtr.Zero); // Phát âm thanh
        }
    }

    public class WaveHeader // Định nghĩa lớp WaveHeader
    {
        private const string FILE_TYPE_ID = "RIFF"; // Định danh loại tệp
        private const string MEDIA_TYPE_ID = "WAVE"; // Định danh loại phương tiện

        public string FileTypeId { get; private set; } // Loại tệp
        public UInt32 FileLength { get; set; } // Độ dài của tệp
        public string MediaTypeId { get; private set; } // Loại phương tiện

        public WaveHeader() // Constructor của lớp WaveHeader
        {
            FileTypeId = FILE_TYPE_ID; // Gán loại tệp
            MediaTypeId = MEDIA_TYPE_ID; // Gán loại phương tiện
            // Minimum size is always 4 bytes
            FileLength = 4; // Gán độ dài của tệp
        }

        public byte[] GetBytes() // Hàm lấy byte của header
        {
            List<Byte> chunkData = new List<byte>(); // Tạo danh sách byte

            chunkData.AddRange(Encoding.ASCII.GetBytes(FileTypeId)); // Thêm loại tệp vào danh sách
            chunkData.AddRange(BitConverter.GetBytes(FileLength)); // Thêm độ dài của tệp vào danh sách
            chunkData.AddRange(Encoding.ASCII.GetBytes(MediaTypeId)); // Thêm loại phương tiện vào danh sách

            return chunkData.ToArray(); // Trả về byte của header
        }
    }

    public class DataChunk // Định nghĩa lớp DataChunk
    {
        private const string CHUNK_ID = "data"; // Định danh của chunk

        public string ChunkId { get; private set; } // Định danh của chunk
        public UInt32 ChunkSize { get; set; } // Kích thước của chunk
        public short[] WaveData; // Dữ liệu âm thanh

        public DataChunk() // Constructor của lớp DataChunk
        {
            ChunkId = CHUNK_ID; // Gán định danh của chunk
            ChunkSize = 0; // Gán kích thước của chunk
        }

        public UInt32 Length() // Hàm lấy kích thước của chunk
        {
            return (UInt32)GetBytes().Length; // Trả về kích thước của chunk
        }

        public byte[] GetBytes() // Hàm lấy byte của chunk
        {
            List<Byte> chunkBytes = new List<byte>(); // Tạo danh sách byte

            chunkBytes.AddRange(Encoding.ASCII.GetBytes(ChunkId)); // Thêm định danh của chunk vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(ChunkSize)); // Thêm kích thước của chunk vào danh sách
            byte[] bufferBytes = new byte[WaveData.Length * 2]; // Tạo bộ đệm byte
            System.Buffer.BlockCopy(WaveData.ToArray(), 0, bufferBytes, 0, bufferBytes.Length); // Sao chép dữ liệu âm thanh vào bộ đệm
            chunkBytes.AddRange(bufferBytes.ToList()); // Thêm bộ đệm vào danh sách

            return chunkBytes.ToArray(); // Trả về byte của chunk
        }

        public void AddSampleDataStereo(short[] leftBuffer, short[] rightBuffer) // Hàm thêm dữ liệu âm thanh stereo
        {
            WaveData = new short[leftBuffer.Length + rightBuffer.Length]; // Tạo dữ liệu âm thanh
            int bufferOffset = 0; // Offset của bộ đệm
            for (int index = 0; index < WaveData.Length; index += 2) // Sao chép dữ liệu âm thanh vào bộ đệm
            {
                WaveData[index] = leftBuffer[bufferOffset]; // Sao chép dữ liệu âm thanh trái vào bộ đệm
                WaveData[index + 1] = rightBuffer[bufferOffset]; // Sao chép dữ liệu âm thanh phải vào bộ đệm
                bufferOffset++; // Tăng offset của bộ đệm
            }
            ChunkSize = (UInt32)WaveData.Length * 2; // Tính toán kích thước của chunk
        }
    }

    public class FormatChunk // Định nghĩa lớp FormatChunk
    {
        private ushort _bitsPerSample; // Số bit mỗi mẫu
        private ushort _channels; // Số kênh
        private uint _frequency; // Tần số
        private const string CHUNK_ID = "fmt "; // Định danh của chunk

        public string ChunkId { get; private set; } // Định danh của chunk
        public UInt32 ChunkSize { get; private set; } // Kích thước của chunk
        public UInt16 FormatTag { get; private set; } // Định danh định dạng

        public UInt16 Channels // Số kênh
        {
            get { return _channels; } // Lấy số kênh
            set { _channels = value; RecalcBlockSizes(); } // Gán số kênh và tính toán kích thước của block
        }

        public UInt32 Frequency // Tần số
        {
            get { return _frequency; } // Lấy tần số
            set { _frequency = value; RecalcBlockSizes(); } // Gán tần số và tính toán kích thước của block
        }

        public UInt32 AverageBytesPerSec { get; private set; } // Số byte trung bình mỗi giây
        public UInt16 BlockAlign { get; private set; } // Kích thước của block

        public UInt16 BitsPerSample // Số bit mỗi mẫu
        {
            get { return _bitsPerSample; } // Lấy số bit mỗi mẫu
            set { _bitsPerSample = value; RecalcBlockSizes(); } // Gán số bit mỗi mẫu và tính toán kích thước của block
        }

        public FormatChunk() // Constructor của lớp FormatChunk
        {
            ChunkId = CHUNK_ID; // Gán định danh của chunk
            ChunkSize = 16; // Gán kích thước của chunk
            FormatTag = 1; // Gán định danh định dạng
            Channels = 2; // Gán số kênh
            Frequency = 44100; // Gán tần số
            BitsPerSample = 16; // Gán số bit mỗi mẫu
            RecalcBlockSizes(); // Tính toán kích thước của block
        }

        private void RecalcBlockSizes() // Hàm tính toán kích thước của block
        {
            BlockAlign = (UInt16)(_channels * (_bitsPerSample / 8)); // Tính toán kích thước của block
            AverageBytesPerSec = _frequency * BlockAlign; // Tính toán số byte trung bình mỗi giây
        }

        public byte[] GetBytes() // Hàm lấy byte của chunk
        {
            List<Byte> chunkBytes = new List<byte>(); // Tạo danh sách byte

            chunkBytes.AddRange(Encoding.ASCII.GetBytes(ChunkId)); // Thêm định danh của chunk vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(ChunkSize)); // Thêm kích thước của chunk vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(FormatTag)); // Thêm định danh định dạng vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(Channels)); // Thêm số kênh vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(Frequency)); // Thêm tần số vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(AverageBytesPerSec)); // Thêm số byte trung bình mỗi giây vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(BlockAlign)); // Thêm kích thước của block vào danh sách
            chunkBytes.AddRange(BitConverter.GetBytes(BitsPerSample)); // Thêm số bit mỗi mẫu vào danh sách

            return chunkBytes.ToArray(); // Trả về byte của chunk
        }

        public UInt32 Length() // Hàm lấy kích thước của chunk
        {
            return (UInt32)GetBytes().Length; // Trả về kích thước của chunk
        }
    }
}