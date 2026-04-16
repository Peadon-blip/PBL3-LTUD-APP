using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using StudentReminderApp.BLL;
using StudentReminderApp.Helpers;
using StudentReminderApp.Models;

namespace StudentReminderApp.Views.Pages
{
    public partial class AdvisorPage : Page
    {
        private readonly AdvisorBLL _advisorBll = new AdvisorBLL();
        private readonly CourseBLL _courseBll = new CourseBLL(); // Thêm dòng này

        public AdvisorPage()
        {
            InitializeComponent();
            Loaded += AdvisorPage_Loaded;
        }

        private async void AdvisorPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AnalyzeDataAsync();
        }

        private async void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            await AnalyzeDataAsync();
        }

        private bool _isLoading = false;

        private async Task AnalyzeDataAsync()
        {
            if (!SessionManager.IsLoggedIn || _isLoading) return;
            _isLoading = true;
            LoadingOverlay.Visibility = Visibility.Visible;

            try
            {
                long idSv = SessionManager.CurrentUser.IdAcc;
                int hocKy = CmbHocKy.SelectedIndex + 1;
                string namHoc = ((ComboBoxItem)CmbNamHoc.SelectedItem).Content.ToString();

                // Lưu GPA và Tín chỉ nếu người dùng có sửa tay
                string rawGpa = TxtGpa.Text.Trim().Replace(",", ".");
                if (double.TryParse(rawGpa, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double gpa) && int.TryParse(TxtAccCredit.Text.Trim(), out int cred))
                {
                    await _advisorBll.UpdateManualStatsAsync(idSv, gpa, cred);
                }

                // Gọi trực tiếp các hàm Async để chạy song song (không bọc Task.Run để tránh lỗi Task<Task<T>>)
                var summaryTask = _advisorBll.GetSummaryAsync(idSv, hocKy, namHoc);
                var suggestedTask = _advisorBll.GetSuggestedCoursesAsync(idSv, hocKy, namHoc);
                var registeredTask = _advisorBll.GetRegisteredCoursesAsync(idSv, hocKy, namHoc);

                await Task.WhenAll(summaryTask, suggestedTask, registeredTask);

                var summary = await summaryTask;
                var suggestedCourses = await suggestedTask;
                var registeredCourses = await registeredTask;

                TxtAccCredit.Text = summary.TotalAccumulatedCredits.ToString();
                TxtRegCredit.Text = summary.RegisteredCreditsThisTerm.ToString();
                TxtRemaining.Text = $"Còn lại: {summary.RemainingCredits} TC";
                TxtGpa.Text = summary.GPAFormatted;
                TxtGpaLevel.Text = summary.GPALevel;

                // Tính toán thanh Progress Bar tín chỉ
                TxtCreditFraction.Text = $"{summary.RegisteredCreditsThisTerm} / {summary.MaxCreditsAllowed} TC";
                double percent = (double)summary.RegisteredCreditsThisTerm / summary.MaxCreditsAllowed * 100;
                if (percent > 100) percent = 100;
                ProgressBar.Width = percent * 2.5; // Giả sử chiều rộng tối đa là 250px
                TxtProgressLabel.Text = percent >= 100 ? "Đã đạt giới hạn tín chỉ" : "Có thể đăng ký thêm";

                DgSuggested.ItemsSource = suggestedCourses;
                TxtSuggestCount.Text = suggestedCourses.Count.ToString();
                TxtNoSuggest.Visibility = suggestedCourses.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

                RegisteredList.ItemsSource = registeredCourses;

                // --- THUẬT TOÁN TẠO LỜI KHUYÊN (NLG - Natural Language Generation) ---
                string advice = $"Dựa trên hồ sơ của bạn, GPA hiện tại là {summary.GPAFormatted} ({summary.GPALevel}). ";

                if (suggestedCourses.Count > 0)
                {
                    var bestCourse = suggestedCourses[0];
                    advice += $"Hệ thống đã loại trừ các môn vi phạm Tiên quyết và tìm được {suggestedCourses.Count} lớp học phần tối ưu nhất. " +
                              $"💡 Khuyến nghị hàng đầu: Hãy ưu tiên đăng ký môn '{bestCourse.TenMonHoc}' do Giảng viên {bestCourse.TenGiangVien} phụ trách, vì đây là lớp có đánh giá chất lượng dạy cao nhất và phù hợp với lộ trình của bạn.";
                }
                else
                {
                    advice += "Tuyệt vời! Hiện tại bạn đã đăng ký đủ môn học hoặc không có môn mới nào mở trong kỳ này. Hãy tập trung hoàn thành tốt các môn đang học nhé.";
                }

                if (summary.RegisteredCreditsThisTerm < 14 && suggestedCourses.Count > 0)
                {
                    advice += "\n⚠️ Cảnh báo: Khối lượng tín chỉ kỳ này của bạn đang khá thấp. Bạn nên đăng ký thêm để tránh trễ tiến độ ra trường.";
                }

                TxtAiAdvice.Text = advice;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi phân tích dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnAskAI_Click(object sender, RoutedEventArgs e)
        {
            string q = TxtChatInput.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(q) || q == "hỏi ai tư vấn thêm...") return;

            string response = "\n\n🤖 AI Trả lời: ";

            // Cây quyết định NLP (Natural Language Processing) cơ bản
            if (q.Contains("rớt") || q.Contains("khó") || q.Contains("nặng"))
                response += "Nếu bạn đang cảm thấy đuối sức, hệ thống khuyên bạn nên giữ nhịp độ ở mức 12-14 tín chỉ/kỳ. Đừng vội vàng, hãy ưu tiên đăng ký lại các môn tiên quyết bị hỏng trước để mở khóa lộ trình nhé!";
            else if (q.Contains("sớm") || q.Contains("nhanh") || q.Contains("ra trường"))
                response += "Để đẩy nhanh tiến độ, bạn nên tận dụng tối đa Học kỳ 3 (Kỳ hè) và đăng ký kịch kim 20-25 tín chỉ cho kỳ chính. Nhìn vào gợi ý ở dưới, hãy gom các môn có cùng tòa nhà để tiết kiệm thời gian di chuyển!";
            else if (q.Contains("điểm") || q.Contains("gpa") || q.Contains("thấp"))
                response += "GPA của bạn đã được ghi nhận. Để kéo điểm lên, hãy chọn học các môn tự chọn dễ thở hơn, hoặc ưu tiên chọn lớp của Giảng viên có Rating cao (như các môn đang được gợi ý bên dưới).";
            else
                response += "Hệ thống đã nhận được câu hỏi. Phân tích dữ liệu học tập cho thấy lộ trình tốt nhất của bạn hiện tại là theo sát danh sách 'Môn học được gợi ý' phía dưới vì chúng đã được AI lọc qua điều kiện ràng buộc.";

            TxtAiAdvice.Text += response;
            TxtChatInput.Text = "";
        }

        private async void BtnQuickRegister_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LopHocPhan lhp)
            {
                btn.IsEnabled = false; // Tránh click liên tục nhiều lần
                try
                {
                    long idSv = SessionManager.CurrentUser.IdAcc;
                    var (ok, msg) = await _courseBll.RegisterAsync(idSv, lhp.IdLopHp);

                    if (ok)
                    {
                        MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        await AnalyzeDataAsync(); // Cập nhật lại toàn bộ màn hình
                    }
                    else
                    {
                        MessageBox.Show(msg, "Thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi đăng ký: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    btn.IsEnabled = true;
                }
            }
        }
    }
}