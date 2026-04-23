using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using StudentReminderApp.Helpers;
using StudentReminderApp.ViewModels;

namespace StudentReminderApp.Models
{
    public class Post : BaseViewModel
    {
        public long IdPost { get; set; }
        public long IdAcc { get; set; }
        public long? IdOriginalPost { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsAnonymous { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorAvatar { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;

        // --- Cập nhật ngay lập tức nhờ OnPropertyChanged ---

        private int _likes;
        public int Likes
        {
            get => _likes;
            set { _likes = value; OnPropertyChanged(); }
        }

        private bool _isLiked;
        public bool IsLiked
        {
            get => _isLiked;
            set { _isLiked = value; OnPropertyChanged(); }
        }

        private int _commentCount;
        public int CommentCount
        {
            get => _commentCount;
            set 
            { 
                _commentCount = value; 
                OnPropertyChanged(); // Tự động lấy tên property là CommentCount
            }
        }

        private int _shareCount;
        public int ShareCount
        {
            get => _shareCount;
            set 
            { 
                _shareCount = value; 
                OnPropertyChanged(); 
            }
        }

        // --- Xử lý hình ảnh và File ---
        private List<string> _filePaths = new List<string>();
        public List<string> FilePaths
        {
            get => _filePaths ?? new List<string>();
            set
            {
                _filePaths = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ImagePaths));
            }
        }

        public List<string> ImagePaths
        {
            get
            {
                if (FilePaths == null) return new List<string>();
                string[] imgExtensions = { ".jpg", ".png", ".jpeg", ".bmp", ".gif" };
                return FilePaths.Where(path =>
                    !string.IsNullOrEmpty(path) &&
                    imgExtensions.Contains(Path.GetExtension(path).ToLower()))
                    .ToList();
            }
        }

        // --- Hiển thị giao diện ---
        private string _backgroundColor = "Transparent";
        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsColored));
                OnPropertyChanged(nameof(PostForeground));
            }
        }

        public bool IsColored => !string.IsNullOrEmpty(BackgroundColor) &&
                                 BackgroundColor != "Transparent" &&
                                 BackgroundColor.ToLower() != "#ffffffff";

        public string PostForeground => IsColored ? "White" : "#050505";
        public bool IsShared => IdOriginalPost.HasValue && IdOriginalPost.Value > 0;

        private Post? _originalPost;
        public Post? OriginalPost
        {
            get => _originalPost;
            set
            {
                _originalPost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsShared));
            }
        }

        private bool _isPublic = true;
        public bool IsPublic
        {
            get => _isPublic;
            set { _isPublic = value; OnPropertyChanged(); OnPropertyChanged(nameof(PrivacyIcon)); }
        }

        public string PrivacyIcon => IsPublic ? "🌎" : "🔒";
        public bool IsMyPost => SessionManager.CurrentUser != null && IdAcc == SessionManager.CurrentUser.IdAcc;

        public string DisplayName => IsAnonymous ? "Người dùng ẩn danh" : (string.IsNullOrWhiteSpace(AuthorName) ? "Thành viên" : AuthorName);

        public string DisplayAvatar
        {
            get
            {
                string defaultImg = "pack://application:,,,/Resources/Images/user.png";
                if (IsAnonymous || string.IsNullOrWhiteSpace(AuthorAvatar)) return defaultImg;
                return AuthorAvatar;
            }
        }

        public string TimeAgo
        {
            get
            {
                TimeSpan span = DateTime.Now - CreatedAt;
                if (span.TotalMinutes < 1) return "Vừa xong";
                if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} phút trước";
                if (span.TotalHours < 24) return $"{(int)span.TotalHours} giờ trước";
                return CreatedAt.ToString("dd/MM/yyyy");
            }
        }
    }
}