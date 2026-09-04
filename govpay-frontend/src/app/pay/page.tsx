"use client";

import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { FormEvent, Suspense, useEffect, useMemo, useState } from "react";
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

function PayPageContent() {
  const router = useRouter();
  const params = useSearchParams();
  const billIdFromUrl = Number(params.get("billId") ?? "0");

  const [bill, setBill] = useState<Bill | null>(null);
  const [paymentMethod, setPaymentMethod] = useState("Bank Transfer");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const session = getStoredSession();
    if (!session?.token) {
      router.push("/login");
      return;
    }

    (async () => {
      try {
        const bills = await request<Bill[]>("/api/Bills", {}, true);
        const selectedBill = bills.find((item) => item.id === billIdFromUrl) ?? null;
        setBill(selectedBill);
      } catch {
        router.push("/login");
      }
    })();
  }, [billIdFromUrl, router]);

  const amountText = useMemo(() => {
    if (!bill) return "0";
    return `৳${bill.amount}`;
  }, [bill]);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (!bill) return;

    setLoading(true);
    setError("");

    try {
      await request("/api/Payment", {
        method: "POST",
        body: JSON.stringify({
          billId: bill.id,
          paymentMethod,
        }),
      }, true);

      router.push("/bills");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Payment failed.");
    } finally {
      setLoading(false);
    }
  };

  if (!bill) {
    return (
      <main className="flex min-h-screen items-center justify-center bg-slate-100 p-6">
        <div className="rounded-2xl bg-white p-8 text-slate-600 shadow-sm ring-1 ring-slate-200">
          Loading bill details...
        </div>
      </main>
    );
  }

  return (
    <main className="min-h-screen bg-slate-100 p-6">
      <div className="mx-auto max-w-3xl rounded-2xl bg-white p-8 shadow-lg ring-1 ring-slate-200">
        <div className="mb-6 flex items-center justify-between gap-4">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-emerald-600">GovPay</p>
            <h1 className="mt-2 text-3xl font-bold text-slate-900">Payment</h1>
          </div>

          <Link href="/bills" className="rounded-xl border border-slate-300 px-4 py-2 font-medium text-slate-800 hover:border-slate-400">
            Back to bills
          </Link>
        </div>

        <div className="mb-6 rounded-2xl bg-slate-900 p-5 text-white">
          <p className="text-sm text-slate-300">Selected bill</p>
          <p className="mt-2 text-2xl font-bold">{bill.billNumber}</p>
          <div className="mt-3 flex items-center justify-between text-sm text-slate-300">
            <span>{bill.billType}</span>
            <span>{amountText}</span>
          </div>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label className="mb-2 block text-sm font-medium text-slate-700">Payment method</label>
            <select
              value={paymentMethod}
              onChange={(event) => setPaymentMethod(event.target.value)}
              className="w-full rounded-xl border border-slate-300 bg-slate-50 px-3 py-2.5 outline-none transition focus:border-emerald-500"
            >
              <option>Bank Transfer</option>
              <option>Mobile Wallet</option>
              <option>Card</option>
            </select>
          </div>

          {error ? <p className="text-sm text-red-600">{error}</p> : null}

          <button
            type="submit"
            disabled={loading}
            className="w-full rounded-xl bg-emerald-600 px-4 py-3 font-semibold text-white transition hover:bg-emerald-700 disabled:opacity-60"
          >
            {loading ? "Processing payment..." : `Pay ${amountText}`}
          </button>
        </form>
      </div>
    </main>
  );
}

export default function PayPage() {
  return (
    <Suspense fallback={<div className="flex min-h-screen items-center justify-center bg-slate-100 p-6 text-slate-600">Loading payment page...</div>}>
      <PayPageContent />
    </Suspense>
  );
}
