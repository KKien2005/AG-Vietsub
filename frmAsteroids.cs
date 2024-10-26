using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ASTEROIDS
{
    public partial class frmAsteroids : Form
    {
        private int m_lX = 0; // Biến lưu trữ tọa độ X
        private int m_lY = 0; // Biến lưu trữ tọa độ Y

        private const int DLEN = 2; // Độ dài hằng số
        private const long DELAYBETWEENLEVELS = 25000000; // Thời gian trễ giữa các cấp độ
        private const float PLAYERRESURRECTIONDELAY = 3.0f; // Thời gian hồi sinh của người chơi
        public const long TICKSPERSECOND = 10000000; // Số tick trong một giây
        public Random randomizer = new Random(DateTime.Now.TimeOfDay.Milliseconds); // Tạo số ngẫu nhiên dựa trên thời gian hiện tại

        private bool bCurLevelTransition = false; // Biến kiểm tra xem có đang chuyển cấp độ hay không

        private List<Asteroid> m_asteroids = new List<Asteroid>(); // Danh sách các tiểu hành tinh
        private Ship m_player; // Đối tượng người chơi
        private Projectiles m_projectiles; // Đối tượng đạn của người chơi
        private bool m_bIsSpacePressed = false; // Kiểm tra xem phím Space có được nhấn hay không
        private Projectiles m_ufoprojectiles; // Đạn từ UFO
        private ScoreBoard m_mainScoreBoard; // Bảng điểm chính
        public EnterHighScoreScreen m_EnterHighScoreScreen; // Màn hình nhập điểm cao
        public bool? m_bDisplayEnterHighScoreScreen = null; // Biến kiểm tra hiển thị màn hình nhập điểm cao
        private const double TIMEOUTBEFOREDISPLAYINGHIGHSCORESCREEN = 3.0f; // Thời gian chờ trước khi hiển thị màn hình điểm cao
        private const double TIMEOUTFORHIGHSCOREDISPLAYSCREEN = 30.0f; // Thời gian hiển thị màn hình điểm cao
        private long ticksHighScoreDisplayScreenTimesOut; // Thời gian hết hạn cho màn hình điểm cao

        private UFO m_ufo; // Đối tượng UFO

        public Graphics g; // Đối tượng đồ họa
        public float ASPECTRATIO; // Tỷ lệ khung hình
        public string DEBUGOUTPUT = ""; // Dữ liệu gỡ lỗi
        private long m_nTicksSinceStart; // Số tick từ khi bắt đầu trò chơi

        private long[] m_nPlayerDestroyedSFTicks = new long[2]; // Thời gian người chơi bị tiêu diệt
        private long m_nUFOFiringSFTicks; // Thời gian UFO bắn
        private long[] m_nPlayerInHyperSpaceSFTicks = new long[2]; // Thời gian người chơi trong không gian siêu tốc

        private int m_numDemoScreensteroids = 10; // Số tiểu hành tinh trong màn hình demo

        public bool m_bGameOver = false; // Biến kiểm tra trò chơi đã kết thúc hay chưa
        public long m_nGameOverResetAtTicks = 0; // Thời gian reset khi trò chơi kết thúc

        public long m_NextRandomUFOEncounter; // Thời gian gặp UFO ngẫu nhiên tiếp theo

        public Sounds mySounds; // Đối tượng âm thanh

        // Biến liên quan đến âm thanh
        private long m_tckNextBGSound; // Thời gian âm thanh nền tiếp theo
        private bool m_bBGSoundLow = false; // Kiểm tra âm thanh nền thấp
        private bool m_bPlayerIsThrusting = false; // Kiểm tra xem người chơi có đang tăng tốc hay không
        private double m_AmbientAccelerator = 1.0; // Biến này dùng để tăng tốc độ âm thanh nền

        public frmAsteroids()
        {
            InitializeComponent(); // Khởi tạo các thành phần của form

            // Ẩn con trỏ chuột
            System.Windows.Forms.Cursor.Hide();
            this.Width = Screen.PrimaryScreen.Bounds.Width; // Đặt chiều rộng của form bằng chiều rộng của màn hình
            this.Height = Screen.PrimaryScreen.Bounds.Height; // Đặt chiều cao của form bằng chiều cao của màn hình
            this.Top = 0; // Đặt vị trí trên cùng của form
            this.Left = 0; // Đặt vị trí trái của form

            ASPECTRATIO = (float)Screen.PrimaryScreen.Bounds.Height / (float)Screen.PrimaryScreen.Bounds.Width; // Tính tỷ lệ khung hình

            timer1.Interval = (int)(1000.0f / 60.0f); // Đặt thời gian trễ cho timer

            //g = this.CreateGraphics(); // Tạo đối tượng đồ họa
            m_player = new Ship(this); // Tạo đối tượng người chơi
            m_mainScoreBoard = new ScoreBoard(this); // Tạo bảng điểm chính
            m_projectiles = new Projectiles(this); // Tạo đối tượng đạn của người chơi
            m_ufoprojectiles = new Projectiles(this); // Tạo đối tượng đạn từ UFO
            m_ufo = new UFO(this); // Tạo đối tượng UFO
            m_EnterHighScoreScreen = new EnterHighScoreScreen(this); // Tạo màn hình nhập điểm cao
            mySounds = new Sounds(this); // Tạo đối tượng âm thanh

            m_ufo.position = new Point(250, 250); // Đặt vị trí UFO

            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true); // Bật chế độ đệm kép

            for (int x = 0; x < m_numDemoScreensteroids; x++)
            {
                m_asteroids.Add(new Asteroid(this, (Asteroid.SIZEOFASTEROID)randomizer.Next(0, 3))); // Tạo các tiểu hành tinh trong màn hình demo
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long lCurTicks = System.DateTime.Now.Ticks; // Số tick hiện tại

            if (m_mainScoreBoard.m_bIsPlaying && !m_bGameOver) // Kiểm tra xem trò chơi có đang diễn ra hay không
            {
                // Người chơi đang chơi và chưa kết thúc trò chơi
                if (m_asteroids.Count > 0 && !m_player.m_bIsActive)
                {
                    // Xóa các tiểu hành tinh và reset điểm
                    m_asteroids.Clear();
                    m_mainScoreBoard.SCORE = 0;
                }
                else if ((lCurTicks - m_nTicksSinceStart) > 20000000 && !m_player.m_bIsActive)
                {
                    // Người chơi đã sẵn sàng và chưa bị tiêu diệt
                    m_player.m_bIsActive = true;
                    m_mainScoreBoard.PLAYERMESSAGE = "";

                    m_mainScoreBoard.CURLEVEL = 1;
                    m_AmbientAccelerator = 1.0;
                    m_NextRandomUFOEncounter = System.DateTime.Now.AddTicks(TICKSPERSECOND * randomizer.Next(5, 15)).Ticks;
                    loadAsteroidsForThisLevel(); // Tải các tiểu hành tinh cho cấp độ hiện tại
                }
                else if (m_asteroids.Count == 0 && m_player.m_bIsActive && !bCurLevelTransition)
                {
                    // Người chơi đã tiêu diệt hết các tiểu hành tinh và chưa chuyển cấp độ
                    m_nTicksSinceStart = System.DateTime.Now.Ticks;

                    m_mainScoreBoard.CURLEVEL += 1;
                    m_AmbientAccelerator = 1.0;

                    switch (m_mainScoreBoard.CURLEVEL)
                    {
                        case 2:
                            m_NextRandomUFOEncounter = System.DateTime.Now.AddTicks(TICKSPERSECOND * randomizer.Next(10, 30)).Ticks;
                            break;
                        default:
                            m_NextRandomUFOEncounter = System.DateTime.Now.AddTicks(TICKSPERSECOND * randomizer.Next(10, 30)).Ticks;
                            break;
                    }
                    bCurLevelTransition = true;
                }
                else if (bCurLevelTransition && ((lCurTicks - m_nTicksSinceStart) > DELAYBETWEENLEVELS))
                {
                    // Người chơi đã tiêu diệt hết các tiểu hành tinh và đã chuyển cấp độ
                    bCurLevelTransition = false;
                    loadAsteroidsForThisLevel(); // Tải các tiểu hành tinh cho cấp độ hiện tại
                }

                if (m_player.bIsHyperSpace)
                {
                    // Người chơi đang trong không gian siêu tốc
                    m_nPlayerInHyperSpaceSFTicks[1] = DateTime.Now.Ticks;
                    if (m_nPlayerInHyperSpaceSFTicks[0] < m_nPlayerInHyperSpaceSFTicks[1])
                    {
                        m_player.velocity = 0.0f;
                        m_player.position.X = randomizer.Next(200, Screen.PrimaryScreen.Bounds.Width - 200);
                        m_player.position.Y = randomizer.Next(250, Screen.PrimaryScreen.Bounds.Height - 250);
                        m_player.bIsHyperSpace = false;
                    }
                }

                if (!m_player.bPlayerIsDestroyed)
                {
                    // Kiểm tra xem người chơi có bị tiêu diệt bởi các tiểu hành tinh hay không
                    foreach (Asteroid a in m_asteroids)
                    {
                        if (m_player.doesObjectCollide(a.Position, a.myRadius))
                        {
                            if (!m_player.bPlayerIsDestroyed)
                            {
                                a.bPlayerCollided = true;
                                mySounds.library[Sounds.SOUNDS.DESTROYEDASTEROIDLARGE].Play();
                                initiatePlayerDestroyedSequence(); // Khởi tạo chuỗi tiêu diệt người chơi
                            }
                            break;
                        }
                    }
                }
                if (m_player.bPlayerIsDestroyed)
                {
                    // Người chơi đã bị tiêu diệt
                    m_nPlayerDestroyedSFTicks[1] = DateTime.Now.Ticks;
                    if ((m_nPlayerDestroyedSFTicks[1] - m_nPlayerDestroyedSFTicks[0]) > (PLAYERRESURRECTIONDELAY * TICKSPERSECOND))
                    {
                        m_player.resetToCenter(); // Reset người chơi về vị trí trung tâm
                        // Nếu người chơi có thể trở lại và không bị tiêu diệt ngay lập tức
                        foreach (Asteroid a in m_asteroids)
                        {
                            if (a.doesPointCollide(m_player.position))
                                goto fuggedAboutIt;
                        }

                        m_player.bPlayerIsDestroyed = false;
                        m_player.velocity = 0.0f;
                    }
                }

            fuggedAboutIt:
                if (m_ufo.IsActive && !m_ufo.bUFOIsDestroyed)
                {
                    // UFO đang hoạt động và chưa bị tiêu diệt
                    mySounds.library[(m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? Sounds.SOUNDS.SPACESHIPLARGE : Sounds.SOUNDS.SPACESHIPSMALL].Play();

                    // Tốc độ bắn của UFO nhỏ hơn so với UFO lớn
                    double firingRate = (m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? 0.9 : 0.7;

                    if ((((double)System.DateTime.Now.Ticks - (double)m_nUFOFiringSFTicks) / (double)TICKSPERSECOND) >= firingRate)
                    {
                        m_nUFOFiringSFTicks = System.DateTime.Now.Ticks;

                        // Xác suất UFO bắn trúng người chơi
                        int rVal = (m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? randomizer.Next(1, 5) : randomizer.Next(1, 4);
                        bool bIsRandom = (rVal != 1);

                        if (bIsRandom)
                            m_ufoprojectiles.RandomFire(m_ufo.position);
                        else
                        {
                            float angle = (float)Math.Atan2(m_player.position.Y - m_ufo.position.Y,
                                                             m_player.position.X - m_ufo.position.X);

                            // Điều chỉnh góc bắn
                            angle = angle + ((float)Math.PI / 2.0f);
                            // DEBUGOUTPUT = String.Format("AOF={0:0.00}", angle );

                            m_ufoprojectiles.NotSoRandomFire(m_ufo.position, angle);
                        }
                        mySounds.library[Sounds.SOUNDS.PROJECTILE].Play();
                    }

                    if (m_player.doesObjectCollide(m_ufo.position, UFO.RADIUS))
                    {
                        // Người chơi bị tiêu diệt bởi UFO
                        initiatePlayerDestroyedSequence();
                        m_ufo.triggerCollisionSequence();
                        Sounds.SOUNDS curSnd = (m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? Sounds.SOUNDS.SPACESHIPLARGE : Sounds.SOUNDS.SPACESHIPSMALL;
                        mySounds.library[Sounds.SOUNDS.DESTROYEDASTEROIDLARGE].Play();
                        mySounds.library[curSnd].Stop();

                        if (m_ufo.m_ufoType == UFO.UFOSIZE.SMALL)
                            m_mainScoreBoard.SCORE += 1000;
                        else
                            m_mainScoreBoard.SCORE += 200;
                    }
                    else
                    {
                        // Kiểm tra xem UFO có bị tiêu diệt bởi các tiểu hành tinh hay không
                        foreach (Asteroid a in m_asteroids)
                        {
                            if (m_ufo.doesObjectCollide(a.Position, a.myRadius))
                            {
                                m_ufo.bUFOIsDestroyed = true;
                                mySounds.library[(m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? Sounds.SOUNDS.SPACESHIPLARGE : Sounds.SOUNDS.SPACESHIPSMALL].Stop();
                                a.bDestroyed = true;
                                if (a.mySize != Asteroid.SIZEOFASTEROID.SMALL)
                                {
                                    for (int nX = 0; nX < 2; nX++)
                                    {
                                        Asteroid newOne = new Asteroid(this, a.mySize - 1, a.Position);
                                        newOne.m_fMoveAngle = a.m_fMoveAngle + randomizer.Next(-25, 25);
                                        newOne.newPseudoRandomVelocity(a);
                                        m_asteroids.Add(newOne);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }

                if (m_NextRandomUFOEncounter < System.DateTime.Now.Ticks && !m_ufo.IsActive && m_player.m_bIsActive)
                {
                    // Gặp UFO ngẫu nhiên
                    m_nUFOFiringSFTicks = System.DateTime.Now.Ticks;
                    if ((System.DateTime.Now.Ticks - m_nTicksSinceStart) < (10 * TICKSPERSECOND))
                        m_AmbientAccelerator = 1.0;
                    else if ((System.DateTime.Now.Ticks - m_nTicksSinceStart) < (20 * TICKSPERSECOND))
                        m_AmbientAccelerator = 0.8;
                    else if ((System.DateTime.Now.Ticks - m_nTicksSinceStart) < (30 * TICKSPERSECOND))
                        m_AmbientAccelerator = 0.6;
                    else
                        m_AmbientAccelerator = 0.4;

                    m_ufo.spawnUFO(m_mainScoreBoard.CURLEVEL, System.DateTime.Now.Ticks - m_nTicksSinceStart);
                    m_NextRandomUFOEncounter = System.DateTime.Now.AddTicks(TICKSPERSECOND * randomizer.Next(5, 20)).Ticks;
                }
                // Tiếp tục gặp UFO ngẫu nhiên
                else if (m_ufo.IsActive && m_player.m_bIsActive)
                {
                    m_NextRandomUFOEncounter = System.DateTime.Now.AddTicks(TICKSPERSECOND * randomizer.Next(5, 15)).Ticks;
                }
            }
            else if (m_bGameOver) // toggle off the game over message and flip to the high score screen (if necessary)
            {
                // Trò chơi đã kết thúc
                if (m_bDisplayEnterHighScoreScreen == null)
                {
                    // Nếu người chơi đạt được điểm cao
                    if (m_mainScoreBoard.highScores.isHighScore(m_mainScoreBoard.SCORE))
                    {
                        m_bDisplayEnterHighScoreScreen = true;
                        m_EnterHighScoreScreen = new EnterHighScoreScreen(this);
                        m_EnterHighScoreScreen.newHighScore.Score = m_mainScoreBoard.SCORE;
                        ticksHighScoreDisplayScreenTimesOut = System.DateTime.Now.AddTicks(
                            (long)((TIMEOUTBEFOREDISPLAYINGHIGHSCORESCREEN + TIMEOUTFORHIGHSCOREDISPLAYSCREEN) * (double)TICKSPERSECOND)).Ticks;
                    }
                    // Người chơi đạt được điểm cao và màn hình nhập điểm cao chưa hết hạn
                    else if ((m_bDisplayEnterHighScoreScreen == true) && (ticksHighScoreDisplayScreenTimesOut > System.DateTime.Now.Ticks))
                    {
                        // Không làm gì cả
                    }
                    else
                    {
                        m_bDisplayEnterHighScoreScreen = false;
                    }
                }
                if (m_bDisplayEnterHighScoreScreen == true)
                {
                    long curTicks = (ticksHighScoreDisplayScreenTimesOut - DateTime.Now.Ticks) / TICKSPERSECOND;
                    if (curTicks > TIMEOUTBEFOREDISPLAYINGHIGHSCORESCREEN &&
                       curTicks < TIMEOUTFORHIGHSCOREDISPLAYSCREEN)
                    {
                        m_bGameOver = true;
                        // m_mainScoreBoard.m_bIsPlaying = false;
                        m_player.m_bIsActive = false;
                        m_EnterHighScoreScreen.m_bDisplay = true;
                        if (m_EnterHighScoreScreen.IsDone)
                        {
                            m_EnterHighScoreScreen.m_bDisplay = false;
                            m_mainScoreBoard.highScores.AddScore(m_EnterHighScoreScreen.newHighScore);
                            m_mainScoreBoard.displayHighScoreScreen();
                            m_bDisplayEnterHighScoreScreen = null;
                            m_mainScoreBoard.m_bIsPlaying = false;
                            m_player.m_bIsActive = false;
                            m_bGameOver = false;
                        }
                    }
                    else if (curTicks <= 0)
                    {
                        m_bDisplayEnterHighScoreScreen = null;
                        m_mainScoreBoard.m_bIsPlaying = false;
                        m_player.m_bIsActive = false;
                        m_bGameOver = false;
                        m_EnterHighScoreScreen.m_bDisplay = false;
                    }
                }
                else if (m_bDisplayEnterHighScoreScreen == false || m_bDisplayEnterHighScoreScreen == null)
                {
                    if (m_nGameOverResetAtTicks < DateTime.Now.Ticks)
                    {
                        m_bDisplayEnterHighScoreScreen = null;
                        m_mainScoreBoard.m_bIsPlaying = false;
                        m_player.m_bIsActive = false;
                        m_bGameOver = false;
                    }
                }
            }

            // Âm thanh
            if (m_mainScoreBoard.m_bIsPlaying && !m_bGameOver)
            {
                if (m_tckNextBGSound < System.DateTime.Now.Ticks)
                {
                    m_tckNextBGSound = System.DateTime.Now.AddTicks((long)((double)TICKSPERSECOND * m_AmbientAccelerator)).Ticks;
                    m_bBGSoundLow = !m_bBGSoundLow;
                    mySounds.library[(m_bBGSoundLow) ? Sounds.SOUNDS.BGMUSICLOW : Sounds.SOUNDS.BGMUSICHIGH].Play(); //  0, Microsoft.DirectX.DirectSound.BufferPlayFlags.Default );
                }
                if (!mySounds.library[Sounds.SOUNDS.THRUST].IsPlaying() && m_bPlayerIsThrusting)
                {
                    mySounds.library[Sounds.SOUNDS.THRUST].Play(); //  0, Microsoft.DirectX.DirectSound.BufferPlayFlags.Default);
                }
            }
            else
                m_bPlayerIsThrusting = false;

            Invalidate();
        }

        private void loadAsteroidsForThisLevel()
        {
            int nL = 0, nM = 0, nS = 0;

            switch (m_mainScoreBoard.CURLEVEL)
            {
                case 1:
                    nL = 4;
                    break;
                case 2:
                    nL = 6;
                    break;
                case 3:
                    nL = 8;
                    break;
                case 4:
                    nL = 9;
                    break;
                case 5:
                    nL = 10;
                    break;

                default:
                    nL = 11; // 
                             // 
                    break;
            }
            int nCur;
            for (nCur = 0; nCur < nL; nCur++)
            {
            tryAgain:
                Asteroid newOne = new Asteroid(this, Asteroid.SIZEOFASTEROID.LARGE);
                if (m_player.doesObjectCollide(newOne.Position, (int)newOne.myRadius))
                    goto tryAgain;

                m_asteroids.Add(newOne);
            }

        }

        private void frmAsteroids_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1:
                    if (!m_mainScoreBoard.m_bIsPlaying)
                    {
                        m_mainScoreBoard.m_bIsPlaying = true;
                        m_mainScoreBoard.SHIPSLEFT = 3;

                        m_nTicksSinceStart = System.DateTime.Now.Ticks;
                    }
                    break;
                case Keys.Up:
                    if (m_player.m_bIsActive)
                    {
                        m_player.Accelerate(e.KeyCode, true);
                        m_bPlayerIsThrusting = true;
                    }
                    break;
                case Keys.Left:
                case Keys.Right:
                    if (m_player.m_bIsActive)
                        m_player.Rotate(e.KeyCode, true);
                    else if (m_bDisplayEnterHighScoreScreen == true)
                        m_EnterHighScoreScreen.ProcessKey(e.KeyCode);
                    break;
                case Keys.Down:
                    if (m_player.m_bIsActive)
                    {
                        m_nPlayerInHyperSpaceSFTicks[0] = System.DateTime.Now.Ticks + (randomizer.Next(5, 25) * 1000000);
                        m_player.bIsHyperSpace = true;
                    }
                    else if (m_bDisplayEnterHighScoreScreen == true)
                        m_EnterHighScoreScreen.ProcessKey(e.KeyCode);

                    break;
                case Keys.Space:
                    if (m_player.m_bIsActive)
                    {
                        if (!m_bIsSpacePressed)
                        {
                            m_projectiles.Fire(m_player, m_player.position, m_player.rotationAngle, m_player.velocity);
                            mySounds.library[Sounds.SOUNDS.PROJECTILE].Play();
                            m_bIsSpacePressed = true;
                        }
                    }
                    break;
            }
        }

        private void frmAsteroids_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (m_player.m_bIsActive)
                    {
                        m_player.Accelerate(e.KeyCode, false);
                        mySounds.library[Sounds.SOUNDS.THRUST].Stop();
                        m_bPlayerIsThrusting = false;
                    }

                    break;
                case Keys.Left:
                case Keys.Right:
                    if (m_player.m_bIsActive)
                        m_player.Rotate(e.KeyCode, false);
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Space:
                    if (m_bIsSpacePressed)
                        m_bIsSpacePressed = false;
                    break;
                default:
                    break;
            }
        }

        private void frmAsteroids_Paint(object sender, PaintEventArgs e)
        {
            //Bitmap buffer = new Bitmap(this.Width, this.Height);
            //g = Graphics.FromImage(buffer);
            g = e.Graphics;

            //g.Clear(this.BackColor);
            List<Projectile> list = m_projectiles.list;
            List<Projectile> listUFOProjectiles = m_ufoprojectiles.list;

            for (int aCur = m_asteroids.Count - 1; aCur >= 0; aCur--)
            {
                Asteroid a = m_asteroids[aCur];
                if (!a.bDestroyed)
                {
                    if (list.Count > 0)
                    {
                        for (int nCur = list.Count - 1; nCur >= 0; nCur--)
                        {
                            if (m_ufo.IsActive && !m_ufo.bUFOIsDestroyed)
                            {
                                if (m_ufo.doesObjectCollide(list[nCur].position, UFO.RADIUS))
                                {
                                    mySounds.library[(m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? Sounds.SOUNDS.SPACESHIPLARGE : Sounds.SOUNDS.SPACESHIPSMALL].Stop();
                                    mySounds.library[Sounds.SOUNDS.DESTROYEDASTEROIDLARGE].Play();
                                    if (m_ufo.m_ufoType == UFO.UFOSIZE.SMALL)
                                        m_mainScoreBoard.SCORE += 1000;
                                    else
                                        m_mainScoreBoard.SCORE += 200;

                                    m_ufo.bUFOIsDestroyed = true;
                                }
                            }

                            if (a.doesPointCollide(list[nCur].position) && !m_player.bPlayerIsDestroyed)
                            {
                                a.bDestroyed = true;
                                m_mainScoreBoard.SCORE += a.ASTEROIDPOINTVALUES[(int)a.mySize];
                                if (a.mySize != Asteroid.SIZEOFASTEROID.SMALL)
                                {
                                    Sounds.SOUNDS cur = (a.mySize == Asteroid.SIZEOFASTEROID.LARGE) ?
                                                        Sounds.SOUNDS.DESTROYEDASTEROIDLARGE : Sounds.SOUNDS.DESTROYEDASTEROIDMEDIUM;
                                    mySounds.library[cur].Play();
                                    for (int nX = 0; nX < 2; nX++)
                                    {
                                        Asteroid newOne = new Asteroid(this, a.mySize - 1, list[nCur].position);
                                        newOne.m_fMoveAngle = a.m_fMoveAngle + randomizer.Next(-25, 25);
                                        newOne.newPseudoRandomVelocity(a);
                                        m_asteroids.Add(newOne);
                                    }
                                }
                                else
                                    mySounds.library[Sounds.SOUNDS.DESTROYEDASTEROIDSMALL].Play();

                                list.RemoveAt(nCur);
                            }
                        }
                    }
                    if (listUFOProjectiles.Count > 0)
                    {
                        for (int nCur = listUFOProjectiles.Count - 1; nCur >= 0; nCur--)
                        {
                            if (m_player.m_bIsActive && !m_player.bPlayerIsDestroyed)
                            {
                                // Was the player hit by a UFO projectile?
                                if (m_player.doesObjectCollide(listUFOProjectiles[nCur].position, 10))
                                {
                                    if (!m_player.bPlayerIsDestroyed)
                                    {
                                        initiatePlayerDestroyedSequence();
                                    }
                                    listUFOProjectiles.RemoveAt(nCur);
                                    break;
                                }
                            }

                            if (a.doesPointCollide(listUFOProjectiles[nCur].position))
                            {
                                a.bDestroyed = true;
                                if (a.mySize != Asteroid.SIZEOFASTEROID.SMALL)
                                {
                                    for (int nX = 0; nX < 2; nX++)
                                    {
                                        Asteroid newOne = new Asteroid(this, a.mySize - 1, listUFOProjectiles[nCur].position);
                                        newOne.m_fMoveAngle = a.m_fMoveAngle + randomizer.Next(-25, 25);
                                        newOne.newPseudoRandomVelocity(a);
                                        m_asteroids.Add(newOne);
                                    }
                                }
                                listUFOProjectiles.RemoveAt(nCur);
                            }
                        }
                    }

                    if (a.bPlayerCollided)
                    {
                        a.bDestroyed = true;
                        a.collisionLocation = m_player.position;
                        m_mainScoreBoard.SCORE += a.ASTEROIDPOINTVALUES[(int)a.mySize];
                        if (a.mySize != Asteroid.SIZEOFASTEROID.SMALL)
                        {
                            for (int nX = 0; nX < 2; nX++)
                            {
                                Asteroid newOne = new Asteroid(this, a.mySize - 1, m_player.position);
                                newOne.m_fMoveAngle = a.m_fMoveAngle + randomizer.Next(-25, 25);
                                newOne.newPseudoRandomVelocity(a);
                                m_asteroids.Add(newOne);
                            }
                        }
                    }

                    if (!m_EnterHighScoreScreen.m_bDisplay)
                    {
                        a.Draw();
                        a.Move();
                    }
                }
                else if (a.destructionAnimation.radius < a.MAXANIMRADIUS)
                {
                    if (!m_EnterHighScoreScreen.m_bDisplay)
                    {
                        a.destructionAnimation.radius += 2.25f; // 3.5=Animation speed for expansion of the explosion
                        a.Draw(); // Draw fixed animation of asteroid with no movement, should be destruction sequence.
                    }
                }
                else
                    m_asteroids.RemoveAt(aCur);

            }
            if (m_player.m_bIsActive) //  && !m_player.bPlayerIsDestroyed
            {
                m_player.Draw(); // Draw player /or/ destruction sequence.
                m_projectiles.Draw();
            }

            if (!m_EnterHighScoreScreen.m_bDisplay)
            {
                if (m_ufo.IsActive)
                    m_ufoprojectiles.Draw();

                m_ufo.Draw();
            }

            m_mainScoreBoard.Draw();
            m_EnterHighScoreScreen.Draw();

        }

        private void initiatePlayerDestroyedSequence()
        {
            m_player.bPlayerIsDestroyed = true;
            m_mainScoreBoard.SHIPSLEFT -= 1;
            if (m_mainScoreBoard.SHIPSLEFT == 0)
            {
                m_bGameOver = true;
                m_nGameOverResetAtTicks = DateTime.Now.Ticks + (4 * TICKSPERSECOND);
            }

            m_nPlayerDestroyedSFTicks[0] = DateTime.Now.Ticks;
        }

        public void onUFOExit()
        {
            mySounds.library[(m_ufo.m_ufoType == UFO.UFOSIZE.LARGE) ? Sounds.SOUNDS.SPACESHIPLARGE : Sounds.SOUNDS.SPACESHIPSMALL].Stop();
        }

        private void frmAsteroids_Load(object sender, EventArgs e)
        {
            DoubleBuffered = true;
        }
    }
}