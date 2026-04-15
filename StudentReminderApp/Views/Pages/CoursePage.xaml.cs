using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using StudentReminderApp.BLL;
using StudentReminderApp.Helpers;
using StudentReminderApp.Models;

namespace StudentReminderApp.Views.Pages
{
    public partial class CoursePage : Page
    {
        private readonly CourseBLL _courseBll = new CourseBLL();

        public CoursePage()
        {
            InitializeComponent();
            Loaded += CoursePage_Loaded;
        }

        private async void CoursePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async void BtnLoadCourses_Click(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private bool _isLoading = false;

        private async Task LoadDataAsync()
        {
            if (!SessionManager.IsLoggedIn || _isLoading) return;
            _isLoading = true;
            LoadingOverlay.Visibility = Visibility.Visible;

            try
            {
                int hocKy = CmbHocKy.SelectedIndex + 1;
                string namHoc = ((ComboBoxItem)CmbNamHoc.SelectedItem).Content.ToString();
                long idSv = SessionManager.CurrentUser.IdAcc; // Đồng bộ dùng CurrentUser

                // Đẩy hoàn toàn tác vụ nặng sang luồng nền để giải phóng UI
                var courses = await Task.Run(() => _courseBll.GetAvailableAsync(hocKy, namHoc, idSv));
                DgCourses.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lớp học phần: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LopHocPhan lhp)
            {
                btn.IsEnabled = false; // Ngăn người dùng click đúp liên tục
                try
                {
                    long idSv = SessionManager.CurrentUser.IdAcc;
                    var (ok, msg) = await Task.Run(() => _courseBll.RegisterAsync(idSv, lhp.IdLopHp));

                    if (ok)
                        MessageBox.Show(msg, "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    else
                        MessageBox.Show(msg, "Thất bại", MessageBoxButton.OK, MessageBoxImage.Warning);

                    await LoadDataAsync(); // Cập nhật lại danh sách (để đổi nút Đăng ký thành Hủy)
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

        private async void BtnUnregister_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is LopHocPhan lhp)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn hủy đăng ký lớp {lhp.TenMonHoc}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    btn.IsEnabled = false;
                    try
                    {
                        long idSv = SessionManager.CurrentUser.IdAcc;
                        await Task.Run(() => _courseBll.UnregisterAsync(idSv, lhp.IdLopHp));
                        await LoadDataAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi hủy: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        btn.IsEnabled = true;
                    }
                }
            }
        }
    }
}