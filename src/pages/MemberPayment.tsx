import { useEffect, useState } from "react";
import {
  CreditCard,
  CheckCircle2,
  AlertCircle,
  Clock,
  XCircle,
  Loader2,
  Sparkles,
  History,
} from "lucide-react";
import { packageService } from "../services/packageService";
import { paymentService } from "../services/paymentService";
import type { Package } from "../types/package";
import type { Payment } from "../types/payment";

const statusStyle: Record<string, { label: string; icon: typeof Clock; color: string; bg: string }> = {
  PENDING: { label: "Pending", icon: Clock, color: "text-amber-600", bg: "bg-amber-50 border-amber-100" },
  COMPLETED: { label: "Completed", icon: CheckCircle2, color: "text-emerald-600", bg: "bg-emerald-50 border-emerald-100" },
  FAILED: { label: "Failed", icon: XCircle, color: "text-red-600", bg: "bg-red-50 border-red-100" },
  CANCELLED: { label: "Cancelled", icon: XCircle, color: "text-slate-500", bg: "bg-slate-50 border-slate-100" },
};

export default function MemberPayment() {
  const [packages, setPackages] = useState<Package[]>([]);
  const [payments, setPayments] = useState<Payment[]>([]);
  const [loading, setLoading] = useState(true);
  const [payingId, setPayingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const [allPackages, myPayments] = await Promise.all([
          packageService.getAllPackages(false),
          paymentService.getMyPayments(),
        ]);
        setPackages(allPackages.filter((p) => String(p.status).toUpperCase() === "ACTIVE"));
        setPayments(myPayments);
      } catch {
        setError("Unable to load packages. Please try again.");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const handlePay = async (pkg: Package) => {
    setError(null);
    setPayingId(pkg.id);
    try {
      const session = await paymentService.createCheckoutSession(Number(pkg.id));
      window.location.href = session.checkoutUrl;
    } catch {
      setError("Something went wrong starting checkout. Please try again.");
      setPayingId(null);
    }
  };

  if (loading) {
    return (
      <div className="flex h-64 items-center justify-center text-slate-400">
        Loading packages...
      </div>
    );
  }

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div>
        <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
          <CreditCard className="w-6 h-6 text-indigo-500" />
          Pay Online
        </h1>
        <p className="text-slate-400 mt-1">
          Choose a package and pay securely by card - your membership activates
          automatically as soon as payment is confirmed.
        </p>
      </div>

      {error && (
        <div className="flex items-center gap-3 px-4 py-3 rounded-xl bg-red-50 border border-red-100 text-red-700 text-sm">
          <AlertCircle className="w-4 h-4 shrink-0" />
          {error}
        </div>
      )}

      {/* Packages */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {packages.length === 0 && (
          <p className="text-slate-400 text-sm col-span-full">
            No packages are available to purchase right now.
          </p>
        )}
        {packages.map((pkg) => (
          <div
            key={pkg.id}
            className="bg-white rounded-2xl border border-slate-100 shadow-sm p-6 flex flex-col gap-4 hover:shadow-md transition-shadow"
          >
            <div className="flex items-start justify-between">
              <div className="w-11 h-11 rounded-xl bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-sm">
                <Sparkles className="w-5 h-5 text-white" />
              </div>
              <span className="text-2xl font-black text-slate-800">
                ${pkg.price.toFixed(2)}
              </span>
            </div>
            <div>
              <p className="font-bold text-slate-800">{pkg.name}</p>
              <p className="text-sm text-slate-400 mt-1">{pkg.description}</p>
              <p className="text-xs text-slate-400 mt-2">
                {pkg.durationInDays} days of access
              </p>
            </div>
            <button
              onClick={() => handlePay(pkg)}
              disabled={payingId === pkg.id}
              className="mt-auto w-full flex items-center justify-center gap-2 py-3 rounded-xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-sm font-bold shadow-md shadow-indigo-500/25 hover:shadow-lg transition-all disabled:opacity-60 disabled:cursor-not-allowed"
            >
              {payingId === pkg.id ? (
                <>
                  <Loader2 className="w-4 h-4 animate-spin" />
                  Redirecting to checkout...
                </>
              ) : (
                <>
                  <CreditCard className="w-4 h-4" />
                  Pay with Card
                </>
              )}
            </button>
          </div>
        ))}
      </div>

      {/* Payment history */}
      <div className="bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
        <div className="px-6 py-4 border-b border-slate-50 flex items-center gap-2">
          <div className="w-7 h-7 rounded-lg bg-slate-50 flex items-center justify-center">
            <History className="w-3.5 h-3.5 text-slate-500" />
          </div>
          <h2 className="text-sm font-semibold text-slate-700">Payment History</h2>
        </div>
        {payments.length === 0 ? (
          <p className="text-sm text-slate-400 p-6">No online payments yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="text-left text-xs font-semibold text-slate-400 uppercase tracking-wider">
                  <th className="px-6 py-3">Package</th>
                  <th className="px-6 py-3">Amount</th>
                  <th className="px-6 py-3">Status</th>
                  <th className="px-6 py-3">Date</th>
                </tr>
              </thead>
              <tbody>
                {payments.map((p) => {
                  const s = statusStyle[String(p.status).toUpperCase()] ?? statusStyle.PENDING;
                  const Icon = s.icon;
                  return (
                    <tr key={p.id} className="border-t border-slate-50">
                      <td className="px-6 py-3.5 font-medium text-slate-700">{p.packageName}</td>
                      <td className="px-6 py-3.5 text-slate-600">${p.amount.toFixed(2)}</td>
                      <td className="px-6 py-3.5">
                        <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${s.bg} ${s.color}`}>
                          <Icon className="w-3 h-3" />
                          {s.label}
                        </span>
                      </td>
                      <td className="px-6 py-3.5 text-slate-400">
                        {new Date(p.createdAt).toLocaleDateString("en-US", {
                          month: "short",
                          day: "numeric",
                          year: "numeric",
                        })}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
