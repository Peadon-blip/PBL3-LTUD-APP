"use client";
import { useEffect, useState } from "react";

export default function AnnouncementPage() {
  const [news, setNews] = useState([]);

  useEffect(() => {
    fetch("http://localhost:5000/api/Announcement").then(res => res.json()).then(setNews);
  }, []);

  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-2xl font-bold mb-8 flex items-center gap-2">📢 Thông báo mới nhất</h1>
      <div className="space-y-6">
        {news.length > 0 ? news.map((n: any) => (
          <div key={n.idAnnouncement} className="bg-white p-6 rounded-2xl shadow-sm border-l-4 border-indigo-600">
            <h2 className="text-xl font-bold text-gray-800">{n.title}</h2>
            <p className="text-gray-600 mt-2">{n.description}</p>
            <div className="mt-4 text-xs text-gray-400 flex justify-between">
              <span>Phạm vi: {n.phamVi}</span>
              <span>Ngày đăng: {new Date(n.ngayDang).toLocaleDateString('vi-VN')}</span>
            </div>
          </div>
        )) : <p className="text-center text-gray-400 italic">Hiện chưa có thông báo nào.</p>}
      </div>
    </div>
  );
}