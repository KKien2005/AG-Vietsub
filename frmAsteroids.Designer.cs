namespace ASTEROIDS // Không gian tên cho trò chơi ASTEROIDS
{
    partial class frmAsteroids // Định nghĩa lớp frmAsteroids
    {
        /// <summary>
        /// Biến thiết kế cần thiết.
        /// </summary>
        private System.ComponentModel.IContainer components = null; // Biến chứa các thành phần giao diện

        /// <summary>
        /// Dọn dẹp bất kỳ tài nguyên nào đang được sử dụng.
        /// </summary>
        /// <param name="disposing">true nếu các tài nguyên quản lý nên được giải phóng; ngược lại, false.</param>
        protected override void Dispose(bool disposing) // Hàm giải phóng tài nguyên
        {
            if (disposing && (components != null)) // Nếu đang giải phóng tài nguyên và components không null
            {
                components.Dispose(); // Giải phóng các thành phần
            }
            base.Dispose(disposing); // Gọi hàm cơ sở
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Phương thức yêu cầu cho hỗ trợ Designer - không nên sửa đổi
        /// nội dung của phương thức này với trình biên tập mã.
        /// </summary>
        private void InitializeComponent() // Hàm khởi tạo giao diện
        {
            this.components = new System.ComponentModel.Container(); // Tạo một container cho các thành phần
            this.timer1 = new System.Windows.Forms.Timer(this.components); // Tạo một timer
            this.SuspendLayout(); // Tạm dừng bố cục

            // 
            // timer1
            // 
            this.timer1.Enabled = true; // Bật timer
            this.timer1.Interval = 30; // Đặt khoảng thời gian tick là 30ms
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick); // Gán sự kiện tick cho timer

            // 
            // frmAsteroids
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(21F, 39F); // Đặt kích thước tự động
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // Chế độ tự động kích thước
            this.BackColor = System.Drawing.Color.Black; // Đặt màu nền là đen
            this.ClientSize = new System.Drawing.Size(1224, 863); // Đặt kích thước của form
            this.ControlBox = false; // Ẩn hộp điều khiển
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; // Ẩn viền form
            this.KeyPreview = true; // Cho phép xử lý phím trước khi gửi đến các điều khiển
            this.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8); // Đặt khoảng cách
            this.MaximizeBox = false; // Ẩn nút tối đa
            this.MinimizeBox = false; // Ẩn nút tối thiểu
            this.Name = "frmAsteroids"; // Đặt tên cho form
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide; // Ẩn nút thay đổi kích thước
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual; // Đặt vị trí bắt đầu
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized; // Đặt trạng thái cửa sổ là tối đa
            this.Load += new System.EventHandler(this.frmAsteroids_Load); // Gán sự kiện load cho form
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmAsteroids_Paint); // Gán sự kiện vẽ cho form
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyDown); // Gán sự kiện nhấn phím cho form
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmAsteroids_KeyUp); // Gán sự kiện thả phím cho form
            this.ResumeLayout(false); // Tiếp tục bố cục
        }

        #endregion

        private System.Windows.Forms.Timer timer1; // Biến lưu timer
    }
}