"use client";

import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { request, setStoredSession } from "@/lib/api";

export default function VerifyOtpPage() {
  const router = useRouter();
  const [username, setUsername] = useState("");
  const [code, setCode] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const storedUsername = window.localStorage.getItem("govpay_pending_username") ?? "";
    setUsername(storedUsername);
  }, []);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError("");

    try {
      const result = await request<{ id: number; username: string; email: string; role: string; token: string }>(
        "/api/Auth/verify-2fa",
        {
          method: "POST",
          body: JSON.stringify({ username, code }),
        },
      );

      setStoredSession({
        username: result.username,
        email: result.email,
        role: result.role,
        token: result.token,
      });

      window.localStorage.removeItem("govpay_pending_username");
      router.push("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Verification failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="flex min-h-screen items-center justify-center bg-[radial-gradient(circle_at_top,_#ecfdf5_0%,_#f8fafc_35%,_#eef2ff_100%)] px-4">
      <div className="w-full max-w-md overflow-hidden rounded-[2rem] border border-slate-200 bg-white/90 shadow-[0_25px_80px_-30px_rgba(15,23,42,0.4)] backdrop-blur-sm">
        <div className="bg-gradient-to-r from-emerald-600 to-teal-500 px-6 pb-16 pt-8 text-white">
          <p className="text-xs font-semibold uppercase tracking-[0.26em] text-emerald-100">Step 2</p>
          <h1 className="mt-3 text-3xl font-black tracking-[-0.04em]">Two-factor verification</h1>
        </div>

        <div className="-mt-10 rounded-[1.5rem] bg-white p-6 shadow-lg shadow-slate-200/80">
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">Username</label>
              <input
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full rounded-xl border border-slate-200 bg-slate-50 px-3 py-2.5 text-slate-900 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-4 focus:ring-emerald-100"
                required
              />
            </div>

            <div>
              <label className="mb-2 block text-sm font-medium text-slate-700">OTP code</label>
              <input
                value={code}
                onChange={(e) => setCode(e.target.value)}
                className="w-full rounded-xl border border-slate-200 bg-slate-50 px-3 py-2.5 text-slate-900 outline-none transition focus:border-emerald-500 focus:bg-white focus:ring-4 focus:ring-emerald-100"
                placeholder="Enter 6-digit code"
                required
              />
            </div>

            {error ? <p className="text-sm text-red-600">{error}</p> : null}

            <button
              type="submit"
              disabled={loading}
              className="w-full rounded-xl bg-gradient-to-r from-emerald-600 to-teal-500 px-4 py-3 font-semibold text-white shadow-lg shadow-emerald-500/20 transition hover:-translate-y-0.5 hover:shadow-xl disabled:opacity-70"
            >
              {loading ? "Verifying..." : "Verify"}
            </button>
          </form>
        </div>
      </div>
    </main>
  );
}
