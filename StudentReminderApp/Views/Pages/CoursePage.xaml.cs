using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using StudentReminderApp.BLL;
using StudentReminderApp.Helpers;
using StudentReminderApp.Models;

namespace StudentReminderApp.Views.Pages
{
    public class CurriculumItem
    {
        public int HocKy { get; set; }
        public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }
        public double SoTC { get; set; }
        public string StatusText { get; set; }  // Đã học, Đang học, Chưa học
        public string BgColor { get; set; }     // Flat Background Color
        public string FgColor { get; set; }     // Flat Foreground Text Color
        public string DiemChu { get; set; }
        public double DiemSo { get; set; }
        public string GiangVien { get; set; } = "N/A";
        public string ThoiKhoaBieu { get; set; } = "N/A";
        public string TuanHoc { get; set; } = "1-15";
    }

    public partial class CoursePage : Page
    {
        private List<CurriculumItem> _allCourses = new List<CurriculumItem>();

        public CoursePage()
        {
            InitializeComponent();
            LoadStudentInfo();
            LoadAllMockData();
            RenderRoadmap();
        }

        private void LoadStudentInfo()
        {
            if (SessionManager.CurrentUser != null)
            {
                TxtStudentInfo.Text = $"SV: {SessionManager.CurrentUser.HoTen} - ID: {SessionManager.CurrentUser.IdAcc}";
            }
        }

        private void Config_Changed(object sender, RoutedEventArgs e)
        {
            // Sự kiện gọi khi đổi ComboBox Hệ đào tạo hoặc RadioButton Chuyên ngành
            if (IsLoaded) RenderRoadmap();
        }

        private void RenderRoadmap()
        {
            int maxSemester = CmbHeDaoTao.SelectedIndex == 0 ? 8 : 10;
            bool isNhat = RbNhat.IsChecked == true;
            bool isAI = RbAI.IsChecked == true;
            bool isDacThu = RbDacThu.IsChecked == true;

            // 1. Lọc môn học theo cấu hình
            var filteredCourses = _allCourses.Where(c => c.HocKy <= maxSemester).ToList();

            if (isDacThu)
                filteredCourses = filteredCourses.Where(c => !c.TenHocPhan.Contains("(Nhật)")).ToList();
            if (isAI)
                filteredCourses = filteredCourses.Where(c => !c.TenHocPhan.Contains("(Nhật)") || c.TenHocPhan.Contains("Trí tuệ nhân tạo")).ToList();

            // 2. Đổ dữ liệu vào DataGrid Khung chương trình
            DgRoadmap.ItemsSource = filteredCourses;

            // 3. Đổ dữ liệu môn Đang học hiện tại (Phần 1)
            DgCurrentCourses.ItemsSource = filteredCourses.Where(c => c.StatusText == "Đang học").ToList();

            // 4. Đổ dữ liệu History (chỉ những môn Đã học / Đang học)
            DgHistory.ItemsSource = filteredCourses.Where(c => c.StatusText != "Chưa học").ToList();
        }

        private void LoadAllMockData()
        {
            // Hàm Helper để set màu nhạt (Pastel) và chữ đậm cho Flat Design UI
            CurriculumItem CreateItem(int hk, string ma, string ten, double tc, string status)
            {
                var item = new CurriculumItem { HocKy = hk, MaHocPhan = ma, TenHocPhan = ten, SoTC = tc, StatusText = status, GiangVien = "Nguyễn Văn A", ThoiKhoaBieu = "Thứ 2 (T1-3)" };
                if (status == "Đã học") { item.BgColor = "#DBEAFE"; item.FgColor = "#1E3A8A"; item.DiemChu = "B+"; item.DiemSo = 3.5; }
                else if (status == "Đang học") { item.BgColor = "#FEF3C7"; item.FgColor = "#92400E"; item.DiemChu = "-"; item.DiemSo = 0; }
                else { item.BgColor = "#F1F5F9"; item.FgColor = "#475569"; item.DiemChu = "-"; item.DiemSo = 0; }
                return item;
            }

            // Add dataset bạn cung cấp với các môn Đang học bám sát yêu cầu
            _allCourses = new List<CurriculumItem>
            {
                // Học kỳ 1 (Giả lập: Đã học)
                CreateItem(1, "3190111", "Giải tích 1", 4, "Đã học"),
                CreateItem(1, "3190260", "Đại số tuyến tính", 3, "Đã học"),
                CreateItem(1, "1022970", "Cấu trúc máy tính và vi xử lý", 2, "Đã học"),
                CreateItem(1, "1023600", "Nhập môn ngành (Nhật)", 2, "Đã học"),
                CreateItem(1, "1022863", "Kỹ thuật lập trình", 3, "Đã học"),
                CreateItem(1, "5070030", "Tiếng Nhật 1 (CNTT)", 1, "Đã học"),

                // Học kỳ 2 (Giả lập: Đã học + Đang học để khớp đề bài)
                CreateItem(2, "3190121", "Giải tích 2", 4, "Đang học"),
                CreateItem(2, "1021263", "Toán rời rạc", 3, "Chưa học"),
                CreateItem(2, "1023280", "Cấu trúc dữ liệu", 2, "Chưa học"),
                CreateItem(2, "1020072", "Phương pháp tính", 3, "Chưa học"),
                CreateItem(2, "5070040", "Tiếng Nhật 2 (CNTT)", 1, "Chưa học"),

                // Học kỳ 3 (Giả lập: Chưa học)
                CreateItem(3, "3190041", "Xác suất thống kê", 3, "Chưa học"),
                CreateItem(3, "1023720", "Lập trình hướng đối tượng", 3, "Chưa học"),
                CreateItem(3, "1020102", "Cơ sở dữ liệu", 2, "Chưa học"),
                CreateItem(3, "1022913", "Nguyên lý hệ điều hành", 2, "Chưa học"),
                CreateItem(3, "1023690", "PBL 2: Dự án cơ sở lập trình", 2, "Chưa học"),
                CreateItem(3, "5070050", "Tiếng Nhật 3 (CNTT)", 1, "Chưa học"),

                // Học kỳ 4 (Giả lập: Đang học 2 môn theo đề bài)
                CreateItem(4, "1022830", "Phân tích & thiết kế giải thuật", 2, "Chưa học"),
                CreateItem(4, "1020292", "Mạng máy tính", 2, "Chưa học"),
                CreateItem(4, "1023703", "Lập trình .NET", 2.5, "Đang học"),
                CreateItem(4, "1023713", "Lập trình Java", 2.5, "Đang học"),
                CreateItem(4, "5070060", "Tiếng Nhật 4 (CNTT)", 1, "Chưa học"),
                
                // Học kỳ 5
                CreateItem(5, "1020313", "Trí tuệ nhân tạo", 2, "Chưa học"),
                CreateItem(5, "1021523", "Công nghệ Web", 2, "Chưa học"),
                CreateItem(5, "5070070", "Tiếng Nhật 5 (CNTT)", 1, "Chưa học"),

                // Học kỳ 6
                CreateItem(6, "1020252", "Công nghệ phần mềm", 2, "Chưa học"),
                CreateItem(6, "1023880", "Lập trình mạng", 2, "Chưa học"),
                CreateItem(6, "1022470", "Xử lý tín hiệu số", 2, "Chưa học"),
                
                // Học kỳ 7
                CreateItem(7, "1023140", "Học máy và ứng dụng", 2, "Chưa học"),
                CreateItem(7, "1023910", "Quản trị mạng", 3, "Chưa học"),
                CreateItem(7, "1020503", "An toàn Thông tin mạng", 2, "Chưa học"),

                // Học kỳ 8
                CreateItem(8, "1023950", "Trí tuệ nhân tạo nâng cao", 3, "Chưa học"),
                CreateItem(8, "1023960", "Khoa học dữ liệu nâng cao", 3, "Chưa học"),
                CreateItem(8, "1024020", "PBL 7: Dự án chuyên ngành 2", 3, "Chưa học"),
                
                // Học kỳ 9 (Chỉ hiện nếu chọn Kỹ sư)
                CreateItem(9, "1024040", "Kiến trúc phần mềm", 3, "Chưa học"),
                CreateItem(9, "1024060", "Công nghệ IoT", 3, "Chưa học"),
                CreateItem(9, "1024100", "Các hệ thống thông minh", 3, "Chưa học"),

                // Học kỳ 10 (Chỉ hiện nếu chọn Kỹ sư)
                CreateItem(10, "1024890", "Thực tập tốt nghiệp (Nhật)", 5, "Chưa học"),
                CreateItem(10, "1024120", "Đồ án tốt nghiệp (Nhật)", 10, "Chưa học")
            };
        }
    }
}