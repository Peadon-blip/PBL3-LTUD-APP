"use client";
import { useEffect, useState } from "react";

export default function LogsPage() {
  const [logs, setLogs] = useState([]);

  useEffect(() => {
    fetch("http://localhost:5000/api/UserLog/11").then(res => res.json()).then(setLogs);
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold text-indigo-900 mb-6">📜 Nhật ký hoạt động</h1>
      <div className="bg-white rounded-2xl shadow-sm border overflow-hidden">
        <table className="w-full text-left border-collapse">
          <thead className="bg-gray-50 text-gray-600 text-sm font-semibold">
            <tr>
              <th className="p-4">Thời gian</th>
              <th className="p-4">Hành động</th>
              <th className="p-4">Địa chỉ IP</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {logs.map((log: any) => (
              <tr key={log.idLog} className="hover:bg-gray-50 transition">
                <td className="p-4 text-sm text-gray-500">{new Date(log.thoiGian).toLocaleString('vi-VN')}</td>
                <td className="p-4 text-gray-800 font-medium">{log.hanhDong}</td>
                <td className="p-4 text-gray-400 text-xs font-mono">{log.ipAddress}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}