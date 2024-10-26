using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ASTEROIDS // Định nghĩa không gian tên cho chương trình
{
    class Animations // Định nghĩa lớp Animations
    {
        // Khai báo danh sách chứa các kích thước hoạt ảnh (animation dimensions)
        private static List<List<Vector2>> animationDimensions =
                                        new List<List<Vector2>>();

        // Phương thức khởi tạo danh sách kích thước hoạt ảnh
        private void InitializeAnimationDimensionsList()
        {
            // Khởi tạo danh sách hiện tại (current list)
            List<Vector2> lCur = new List<Vector2>();

            // Thêm các kích thước hoạt ảnh vào danh sách hiện tại
            lCur.Add(new Vector2(0.85f, 40.0f)); // 1: Kích thước hoạt ảnh đầu tiên
            lCur.Add(new Vector2(0.75f, 75.0f)); // 2: Kích thước hoạt ảnh thứ hai
            lCur.Add(new Vector2(1.0f, 68.0f)); // 3: Kích thước hoạt ảnh thứ ba
            lCur.Add(new Vector2(0.6f, 118.0f)); // 4: Kích thước hoạt ảnh thứ tư
            lCur.Add(new Vector2(1.0f, 113.0f)); // 5: Kích thước hoạt ảnh thứ năm
            lCur.Add(new Vector2(0.8f, 136.0f)); // 6: Kích thước hoạt ảnh thứ sáu
            lCur.Add(new Vector2(0.45f, 154.0f)); // 7: Kích thước hoạt ảnh thứ bảy
            lCur.Add(new Vector2(0.7f, 210.0f)); // 8: Kích thước hoạt ảnh thứ tám
            lCur.Add(new Vector2(0.75f, 248.0f)); // 9: Kích thước hoạt ảnh thứ chín
            lCur.Add(new Vector2(0.35f, 273.0f)); // 10: Kích thước hoạt ảnh thứ mười
            lCur.Add(new Vector2(1.0f, 313.0f)); // 11: Kích thước hoạt ảnh thứ mười một
            lCur.Add(new Vector2(0.5f, 343.0f)); // 12: Kích thước hoạt ảnh thứ mười hai

            // Thêm danh sách hiện tại vào danh sách kích thước hoạt ảnh
            animationDimensions.Add(lCur);
        }

        // Định nghĩa kiểu hoạt ảnh (Animation Type)
        public enum ANIMTYPE
        {
            GENERAL = 0 // Kiểu hoạt ảnh chung
        }

        // Biến lưu trữ kiểu hoạt ảnh
        public ANIMTYPE animType;
        // Biến lưu trữ bán kính
        public float radius = 0.0f;

        // Hàm khởi tạo cho lớp Animations
        public Animations(ANIMTYPE anim)
        {
            // Khởi tạo danh sách kích thước hoạt ảnh
            InitializeAnimationDimensionsList();
            // Gán kiểu hoạt ảnh
            animType = anim;
        }

        // Phương thức trả về danh sách kích thước hoạt ảnh dựa trên kiểu hoạt ảnh
        public List<Vector2> sequence()
        {
            return animationDimensions[(int)animType]; // Trả về danh sách kích thước tương ứng với kiểu hoạt ảnh
        }
    }
}