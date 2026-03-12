"use client";
import { useEffect, useState } from "react";

export default function GiangVienPage() {
  const [list, setList] = useState([]);
  const [selectedGv, setSelectedGv] = useState<any>(null);
  const [form, setForm] = useState({ soSao: 5, noiDung: "", isAnonymous: false });

  useEffect(() => {
    fetch("http://localhost:5000/api/GiangVien").then(res => res.json()).then(setList);
  }, []);

  const submitDanhGia = async () => {
    const res = await fetch("http://localhost:5000/api/DanhGia", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ ...form, idGiangVien: selectedGv.idGiangVien, idAcc: 11, status: "Approved" })
    });
    if (res.ok) {
      alert("Đánh giá thành công!");
      setSelectedGv(null);
    }
  };

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold text-indigo-900 mb-6">👨‍🏫 Danh sách Giảng viên</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {list.map((gv: any) => (
          <div key={gv.idGiangVien} className="bg-white p-6 rounded-2xl shadow-sm border hover:shadow-md transition">
            <div className="w-12 h-12 bg-indigo-100 rounded-full flex items-center justify-center text-xl mb-4">👤</div>
            <h3 className="text-lg font-bold">{gv.tenGiangVien}</h3>
            <p className="text-gray-500 text-sm">{gv.khoa} • {gv.hocVi}</p>
            <button onClick={() => setSelectedGv(gv)} className="mt-4 w-full bg-indigo-600 text-white py-2 rounded-xl hover:bg-indigo-700">Đánh giá</button>
          </div>
        ))}
      </div>

      {selectedGv && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white p-8 rounded-3xl w-full max-w-md">
            <h2 className="text-xl font-bold mb-4">Đánh giá: {selectedGv.tenGiangVien}</h2>
            <div className="space-y-4">
              <select className="w-full border p-3 rounded-xl" onChange={e => setForm({...form, soSao: +e.target.value})}>
                {[5,4,3,2,1].map(s => <option key={s} value={s}>{s} Sao</option>)}
              </select>
              <textarea className="w-full border p-3 rounded-xl h-32" placeholder="Nhận xét của bạn..." onChange={e => setForm({...form, noiDung: e.target.value})} />
              <label className="flex items-center gap-2"><input type="checkbox" onChange={e => setForm({...form, isAnonymous: e.target.checked})} /> Gửi ẩn danh</label>
              <div className="flex gap-3">
                <button onClick={() => setSelectedGv(null)} className="flex-1 py-3 bg-gray-100 rounded-xl">Hủy</button>
                <button onClick={submitDanhGia} className="flex-1 py-3 bg-indigo-600 text-white rounded-xl">Gửi</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}