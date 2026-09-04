import Link from "next/link";

const featureCards = [
  "Secure citizen billing",
  "Two-factor authentication",
  "Payment history tracking",
];

export default function Home() {
  return (
    <main className="min-h-screen bg-[radial-gradient(circle_at_top,_#ecfdf5_0%,_#f8fafc_35%,_#eef2ff_100%)] text-slate-900">
      <section className="mx-auto flex min-h-screen max-w-7xl items-center px-6 py-16">
        <div className="w-full overflow-hidden rounded-[2rem] border border-slate-200/80 bg-white/80 shadow-[0_25px_80px_-30px_rgba(15,23,42,0.35)] backdrop-blur-sm">
          <div className="grid gap-10 px-6 py-8 md:grid-cols-2 md:px-10 lg:px-14 lg:py-12">
            <div className="flex flex-col justify-center">
              <div className="mb-5 inline-flex w-fit items-center gap-2 rounded-full border border-emerald-200 bg-emerald-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] text-emerald-700">
                <span className="h-2 w-2 rounded-full bg-emerald-500" />
                GovPay
              </div>

              <h1 className="max-w-xl text-4xl font-black tracking-[-0.05em] text-slate-900 md:text-5xl lg:text-6xl">
                Modern public finance, built for trust.
              </h1>

              <p className="mt-6 max-w-lg text-lg leading-8 text-slate-600">
                Manage government bills, verify secure access, and keep a clear record of every payment in one place.
              </p>

              <div className="mt-8 flex flex-wrap gap-4">
                <Link
                  href="/login"
                  className="rounded-xl bg-gradient-to-r from-emerald-600 to-teal-500 px-6 py-3.5 font-semibold text-white shadow-lg shadow-emerald-500/20 transition hover:-translate-y-0.5 hover:shadow-xl"
                >
                  Login
                </Link>
                <Link
                  href="/register"
                  className="rounded-xl border border-slate-200 bg-white px-6 py-3.5 font-semibold text-slate-800 shadow-sm transition hover:-translate-y-0.5 hover:border-slate-300 hover:bg-slate-50"
                >
                  Register
                </Link>
              </div>
            </div>

            <div className="rounded-[1.75rem] bg-slate-950 p-6 text-white shadow-[0_24px_60px_-20px_rgba(15,23,42,0.9)] md:p-8">
              <div className="mb-6 flex items-center justify-between rounded-2xl bg-white/5 px-4 py-3 ring-1 ring-white/10">
                <div>
                  <p className="text-sm text-slate-300">Active account</p>
                  <p className="text-xl font-bold">Citizen Portal</p>
                </div>
                <span className="rounded-full bg-emerald-500/20 px-3 py-1 text-xs font-semibold text-emerald-300 ring-1 ring-emerald-400/20">
                  Secure
                </span>
              </div>

              <div className="space-y-4">
                {featureCards.map((item) => (
                  <div key={item} className="flex items-center gap-3 rounded-2xl border border-white/10 bg-white/5 px-4 py-3">
                    <span className="flex h-7 w-7 items-center justify-center rounded-full bg-emerald-500/20 text-sm text-emerald-300">
                      ✓
                    </span>
                    <span className="font-medium text-slate-100">{item}</span>
                  </div>
                ))}
              </div>

              <div className="mt-8 rounded-2xl bg-gradient-to-r from-emerald-400 via-teal-400 to-cyan-300 p-4 text-slate-900 shadow-lg shadow-emerald-500/30">
                <p className="text-xs font-semibold uppercase tracking-[0.26em] text-slate-800/80">This month</p>
                <p className="mt-3 text-3xl font-black tracking-tight">৳ 18,450</p>
                <p className="mt-1 text-sm text-slate-800">Total payable balance</p>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
