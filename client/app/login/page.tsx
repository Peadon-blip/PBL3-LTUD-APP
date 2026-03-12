"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const router = useRouter();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      const res = await fetch("http://localhost:5000/api/Auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
      });

      if (res.ok) {
        const data = await res.json();
        localStorage.setItem("user", JSON.stringify(data));
        router.push("/"); 
      } else {
        setError("Tên đăng nhập hoặc mật khẩu không đúng!");
      }
    } catch (err) {
      setError("Không thể kết nối Backend (Port 5000). Hãy chạy API trước!");
    }
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-white">
      <div className="w-full max-w-sm p-6">
        <div className="text-center mb-10">
          <h1 className="text-[#c62917] text-6xl font-black italic tracking-tighter">everytime</h1>
          <p className="text-gray-400 mt-2 font-bold text-[10px] uppercase">Cổng thông tin sinh viên PBL3</p>
        </div>

        <form onSubmit={handleLogin} className="space-y-3">
          <input
            type="text"
            placeholder="Tên đăng nhập"
            className="w-full p-4 bg-gray-100 rounded-2xl border-none outline-none focus:ring-2 ring-red-100"
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <input
            type="password"
            placeholder="Mật khẩu"
            className="w-full p-4 bg-gray-100 rounded-2xl border-none outline-none focus:ring-2 ring-red-100"
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          {error && <p className="text-[#c62917] text-xs font-bold text-center">{error}</p>}

          <button
            type="submit"
            className="w-full py-4 bg-[#c62917] text-white rounded-full font-bold text-lg hover:bg-red-700 transition-all shadow-lg shadow-red-100 mt-4"
          >
            Đăng nhập
          </button>
        </form>

        <div className="flex justify-center gap-4 mt-8 text-sm text-gray-400 font-bold">
          <span>Tìm tài khoản</span>
          <span className="text-gray-200">|</span>
          <span>Đăng ký</span>
        </div>
      </div>
    </div>
  );
}