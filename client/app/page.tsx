"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";

export default function Home() {
  const router = useRouter();
  const [user, setUser] = useState<any>(null);

  // Dữ liệu cho bảng lịch
  const days = ["Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"];
  const hours = [8, 9, 10, 11, 12, 13, 14, 15, 16];

  useEffect(() => {
    const storedUser = localStorage.getItem("user");
    if (!storedUser) {
      router.push("/login");
    } else {
      setUser(JSON.parse(storedUser));
    }
  }, []);

  if (!user) return null;

  return (
    <div className="max-w-4xl mx-auto space-y-6 pb-10">
      {/* 1. Header & Thông tin User */}
      <div className="flex justify-between items-end px-2">
        <div>
          <h2 className="text-2xl font-black text-gray-800 italic uppercase">Thời khóa biểu</h2>
          <p className="text-gray-400 text-xs font-bold uppercase tracking-widest">Học kỳ 2 - 2024</p>
        </div>
        <div className="text-right">
          <span className="text-sm font-bold text-gray-700 block">{user.fullName}</span>
          <button 
            onClick={() => { localStorage.removeItem("user"); router.push("/login"); }}
            className="text-[10px] font-bold text-[#c62917] hover:underline uppercase"
          >
            Đăng xuất
          </button>
        </div>
      </div>

      {/* 2. Menu chức năng icon màu sắc phía trên */}
      <div className="grid grid-cols-4 gap-2 bg-white p-4 rounded-3xl shadow-sm border border-gray-100">
        {[
          { name: "Giảng viên", icon: "👨‍🏫", color: "text-blue-500", path: "/giangvien" },
          { name: "Nhắc lịch", icon: "🔔", color: "text-red-500", path: "/reminder" },
          { name: "Thông báo", icon: "📢", color: "text-orange-500", path: "/announcement" },
          { name: "Nhật ký", icon: "📜", color: "text-green-500", path: "/logs" }
        ].map((item, idx) => (
          <Link href={item.path} key={idx} className="flex flex-col items-center group">
            <div className="text-3xl mb-1 group-hover:scale-110 transition">{item.icon}</div>
            <span className={`text-[10px] font-extrabold ${item.color}`}>{item.name}</span>
          </Link>
        ))}
      </div>

      {/* 3. Bảng Thời khóa biểu Everytime Style */}
      <div className="bg-white rounded-2xl border border-gray-200 overflow-hidden shadow-sm">
        <table className="w-full border-collapse table-fixed">
          <thead>
            <tr className="bg-gray-50/50">
              <th className="w-10 border-r border-b"></th>
              {days.map(d => (
                <th key={d} className="p-2 border-r border-b text-[10px] text-gray-400 font-bold">{d}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {hours.map(h => (
              <tr key={h}>
                <td className="text-[9px] text-gray-300 text-center border-r border-b h-14 font-bold">{h}</td>
                {days.map((_, i) => (
                  <td key={i} className="border-r border-b p-[2px] relative group hover:bg-gray-50/50">
                    {/* Ô Môn học mẫu 1: Pastel Xanh */}
                    {h === 9 && i === 0 && (
                      <div className="absolute inset-[2px] p-1.5 rounded-lg bg-emerald-50 border border-emerald-100 text-emerald-700 flex flex-col justify-between shadow-sm z-10 cursor-pointer">
                        <span className="text-[10px] font-black leading-tight">Lập trình .NET</span>
                        <span className="text-[8px] font-bold opacity-60 italic">P.302</span>
                      </div>
                    )}
                    
                    {/* Ô Môn học mẫu 2: Pastel Cam */}
                    {h === 13 && i === 2 && (
                      <div className="absolute inset-[2px] p-1.5 rounded-lg bg-amber-50 border border-amber-100 text-amber-700 flex flex-col justify-between shadow-sm z-10 cursor-pointer">
                        <span className="text-[10px] font-black leading-tight">Cơ sở dữ liệu</span>
                        <span className="text-[8px] font-bold opacity-60 italic">Hội trường A</span>
                      </div>
                    )}

                    {/* Ô Môn học mẫu 3: Pastel Tím */}
                    {h === 15 && i === 4 && (
                      <div className="absolute inset-[2px] p-1.5 rounded-lg bg-indigo-50 border border-indigo-100 text-indigo-700 flex flex-col justify-between shadow-sm z-10 cursor-pointer">
                        <span className="text-[10px] font-black leading-tight">Tiếng Anh</span>
                        <span className="text-[8px] font-bold opacity-60 italic">Online</span>
                      </div>
                    )}
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}