"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { clearStoredSession, getStoredSession, request } from "@/lib/api";

type Bill = {
  id: number;
  billNumber: string;
  billType: string;
  amount: number;
  status: string;
  dueDate: string;
  description: string;
};

type Payment = {
  id: number;
  userId: number;
  billId: number;
  amount: number;
  paymentMethod: string;
  status: string;
  paidAt: string;
  transactionReference: string;
};

type UserSummary = {
  id: number;
  username: string;
  email: string;
  role: string;
};

export default function DashboardPage() {
  const router = useRouter();
  const [session, setSession] = useState(getStoredSession());
  const [bills, setBills] = useState<Bill[]>([]);
  const [history, setHistory] = useState<Payment[]>([]);
  const [allUsers, setAllUsers] = useState<UserSummary[]>([]);
  const [allPayments, setAllPayments] = useState<Payment[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const activeSession = getStoredSession();
    if (!activeSession?.token) {
      router.push("/login");
      return;
    }

    setSession(activeSession);

    const loadData = async () => {
      try {
        const billData = await request<Bill[]>("/api/Bills", {}, true);
        const paymentHistory = await request<Payment[]>("/api/Payment/history", {}, true);

        setBills(billData ?? []);
        setHistory(paymentHistory ?? []);

        if (activeSession.role === "Admin") {
          const usersData = await request<UserSummary[]>("/api/User", {}, true);
          const allPaymentsData = await request<Payment[]>("/api/Payment/all", {}, true);

          setAllUsers(usersData ?? []);
          setAllPayments(allPaymentsData ?? []);
        }
      } catch {
        clearStoredSession();
        router.push("/login");
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [router]);

  if (!session) {
    return null;
  }

  return (
    <main className="min-h-screen bg-slate-100 p-6">
      <div className="mx-auto max-w-6xl">
        <header className="mb-8 flex items-center justify-between rounded-2xl bg-slate-900 px-6 py-5 text-white shadow-lg">
          <div>
            <p className="text-sm uppercase tracking-[0.2em] text-emerald-400">GovPay</p>
            <h1 className="mt-1 text-2xl font-bold">Dashboard</h1>
          </div>

          <div className="flex items-center gap-3">
            <Link href="/bills" className="rounded-lg bg-white/10 px-4 py-2 font-medium text-white hover:bg-white/20">
              Bills
            </Link>
            <button
              onClick={() => {
                clearStoredSession();
                router.push("/login");
              }}
              className="rounded-lg border border-white/20 px-4 py-2 font-medium text-white hover:bg-white/10"
            >
              Logout
            </button>
          </div>
        </header>

        <div className="grid gap-4 md:grid-cols-3">
          <div className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
            <p className="text-sm text-slate-500">Welcome</p>
            <p className="mt-2 text-2xl font-bold text-slate-900">{session.username}</p>
          </div>
          <div className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
            <p className="text-sm text-slate-500">Role</p>
            <p className="mt-2 text-2xl font-bold text-slate-900">{session.role}</p>
          </div>
          <div className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
            <p className="text-sm text-slate-500">Bills</p>
            <p className="mt-2 text-2xl font-bold text-slate-900">{bills.length}</p>
          </div>
        </div>

        <div className="mt-8 grid gap-6 lg:grid-cols-2">
          <section className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
            <h2 className="mb-4 text-xl font-bold text-slate-900">Recent bills</h2>
            {loading ? (
              <p className="text-slate-500">Loading...</p>
            ) : bills.length === 0 ? (
              <p className="text-slate-500">No bills found.</p>
            ) : (
              <div className="space-y-3">
                {bills.map((bill) => (
                  <div key={bill.id} className="rounded-xl border border-slate-200 p-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <p className="font-semibold text-slate-900">{bill.billNumber}</p>
                        <p className="text-sm text-slate-500">{bill.billType}</p>
                      </div>
                      <span className="rounded-full bg-emerald-100 px-2 py-1 text-xs font-semibold text-emerald-700">
                        {bill.status}
                      </span>
                    </div>
                    <div className="mt-3 flex items-center justify-between text-sm text-slate-600">
                      <span>{new Date(bill.dueDate).toLocaleDateString()}</span>
                      <span className="font-semibold text-slate-900">৳{bill.amount}</span>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </section>

          <section className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
            <h2 className="mb-4 text-xl font-bold text-slate-900">Payment history</h2>
            {loading ? (
              <p className="text-slate-500">Loading...</p>
            ) : history.length === 0 ? (
              <p className="text-slate-500">No payment history yet.</p>
            ) : (
              <div className="space-y-3">
                {history.map((payment) => (
                  <div key={payment.id} className="rounded-xl border border-slate-200 p-4">
                    <div className="flex items-center justify-between">
                      <p className="font-semibold text-slate-900">Bill #{payment.billId}</p>
                      <span className="rounded-full bg-blue-100 px-2 py-1 text-xs font-semibold text-blue-700">
                        {payment.status}
                      </span>
                    </div>
                    <div className="mt-2 text-sm text-slate-600">
                      <p>Method: {payment.paymentMethod}</p>
                      <p>Amount: ৳{payment.amount}</p>
                      <p>Date: {new Date(payment.paidAt).toLocaleString()}</p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </section>
        </div>

        {session.role === "Admin" ? (
          <div className="mt-8 grid gap-6 lg:grid-cols-2">
            <section className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
              <h2 className="mb-4 text-xl font-bold text-slate-900">All users</h2>
              {loading ? (
                <p className="text-slate-500">Loading...</p>
              ) : allUsers.length === 0 ? (
                <p className="text-slate-500">No users found.</p>
              ) : (
                <div className="space-y-3">
                  {allUsers.map((user) => (
                    <div key={user.id} className="rounded-xl border border-slate-200 p-4">
                      <div className="flex items-center justify-between">
                        <div>
                          <p className="font-semibold text-slate-900">{user.username}</p>
                          <p className="text-sm text-slate-500">{user.email}</p>
                        </div>
                        <span className="rounded-full bg-violet-100 px-2 py-1 text-xs font-semibold text-violet-700">
                          {user.role}
                        </span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </section>

            <section className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-slate-200">
              <h2 className="mb-4 text-xl font-bold text-slate-900">All payment activity</h2>
              {loading ? (
                <p className="text-slate-500">Loading...</p>
              ) : allPayments.length === 0 ? (
                <p className="text-slate-500">No payment activity yet.</p>
              ) : (
                <div className="space-y-3">
                  {allPayments.map((payment) => (
                    <div key={payment.id} className="rounded-xl border border-slate-200 p-4">
                      <div className="flex items-center justify-between">
                        <p className="font-semibold text-slate-900">User #{payment.userId} • Bill #{payment.billId}</p>
                        <span className="rounded-full bg-blue-100 px-2 py-1 text-xs font-semibold text-blue-700">
                          {payment.status}
                        </span>
                      </div>
                      <div className="mt-2 text-sm text-slate-600">
                        <p>Method: {payment.paymentMethod}</p>
                        <p>Amount: ৳{payment.amount}</p>
                        <p>Date: {new Date(payment.paidAt).toLocaleString()}</p>
                        <p>Ref: {payment.transactionReference}</p>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </section>
          </div>
        ) : null}
      </div>
    </main>
  );
}
