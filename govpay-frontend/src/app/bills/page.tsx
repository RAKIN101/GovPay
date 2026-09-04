"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import { getStoredSession, request } from "@/lib/api";

type Bill = {
  id: number;
  billNumber: string;
  billType: string;
  amount: number;
  status: string;
  dueDate: string;
  description: string;
};

export default function BillsPage() {
  const router = useRouter();
  const [bills, setBills] = useState<Bill[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const session = getStoredSession();
    if (!session?.token) {
      router.push("/login");
      return;
    }

    (async () => {
      try {
        const data = await request<Bill[]>("/api/Bills", {}, true);
        setBills(data ?? []);
      } catch {
        router.push("/login");
      } finally {
        setLoading(false);
      }
    })();
  }, [router]);

  return (
    <main className="min-h-screen bg-slate-100 p-6">
      <div className="mx-auto max-w-5xl">
        <header className="mb-6 flex items-center justify-between">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-600">GovPay</p>
            <h1 className="mt-2 text-3xl font-bold text-slate-900">Bills</h1>
          </div>

          <Link
            href="/dashboard"
            className="rounded-xl border border-slate-300 bg-white px-4 py-2 font-medium text-slate-800 hover:border-slate-400"
          >
            Back to dashboard
          </Link>
        </header>

        {loading ? (
          <div className="rounded-2xl bg-white p-6 text-slate-600 shadow-sm ring-1 ring-slate-200">
            Loading bills...
          </div>
        ) : bills.length === 0 ? (
          <div className="rounded-2xl bg-white p-6 text-slate-600 shadow-sm ring-1 ring-slate-200">
            No bills available right now.
          </div>
        ) : (
          <div className="space-y-4">
            {bills.map((bill) => (
              <article key={bill.id} className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                  <div>
                    <div className="flex items-center gap-3">
                      <h2 className="text-xl font-bold text-slate-900">{bill.billNumber}</h2>
                      <span className="rounded-full bg-emerald-100 px-2 py-1 text-xs font-semibold text-emerald-700">
                        {bill.status}
                      </span>
                    </div>
                    <p className="mt-1 text-sm text-slate-500">{bill.billType}</p>
                    <p className="mt-3 text-slate-700">{bill.description}</p>
                  </div>

                  <div className="text-left md:text-right">
                    <p className="text-sm text-slate-500">Due date</p>
                    <p className="font-medium text-slate-800">{new Date(bill.dueDate).toLocaleDateString()}</p>
                    <p className="mt-3 text-2xl font-black text-slate-900">৳{bill.amount}</p>
                  </div>
                </div>

                <div className="mt-5 flex justify-end">
                  <Link
                    href={`/pay?billId=${bill.id}`}
                    className="rounded-xl bg-emerald-600 px-4 py-2 font-semibold text-white transition hover:bg-emerald-700"
                  >
                    {bill.status === "Paid" ? "View payment" : "Pay now"}
                  </Link>
                </div>
              </article>
            ))}
          </div>
        )}
      </div>
    </main>
  );
}
