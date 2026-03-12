import "./globals.css";

export const metadata = {
  title: "Everytime PBL3 | Hệ thống Quản lý Sinh viên",
  description: "Ứng dụng hỗ trợ học tập và đánh giá giảng viên",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="vi">
      <body className="bg-[#f8f9fa] min-h-screen text-gray-900 font-sans antialiased">
        {/*(Page-level Layout).
        */}
        <div className="relative flex flex-col min-h-screen">
          {children}
        </div>
        
        {/*Footer chung
        */}
        <footer className="py-6 text-center text-gray-400 text-[10px] font-medium uppercase tracking-widest">
          © 2026 PBL3 Project - Đại học Bách Khoa
        </footer>
      </body>
    </html>
  );
}