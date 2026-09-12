import { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { CheckCircle2, Loader2, Clock } from "lucide-react";
import { paymentService } from "../services/paymentService";

const POLL_INTERVAL_MS = 2000;
const MAX_ATTEMPTS = 10;

// Stripe's redirect here can land slightly before or after the webhook that
// actually confirms payment and provisions the membership - this page never
// creates anything itself, it just watches /payments/me until the matching
// payment shows up as Completed (or gives up gracefully after a while).
export default function PaymentSuccess() {
  const navigate = useNavigate();
  const [state, setState] = useState<"confirming" | "confirmed" | "pending">("confirming");
  const attempts = useRef(0);

  useEffect(() => {
    let cancelled = false;

    const poll = async () => {
      try {
        const payments = await paymentService.getMyPayments();
        const latest = payments[0];
        if (!cancelled && latest && String(latest.status).toUpperCase() === "COMPLETED") {
          setState("confirmed");
          return;
        }
      } catch {
        // ignore - retry on next tick
      }

      attempts.current += 1;
      if (cancelled) return;
      if (attempts.current >= MAX_ATTEMPTS) {
        setState("pending");
        return;
      }
      setTimeout(poll, POLL_INTERVAL_MS);
    };

    poll();
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div className="min-h-[70vh] flex items-center justify-center">
      <div className="max-w-md w-full text-center bg-white rounded-3xl border border-slate-100 shadow-sm p-10">
        {state === "confirming" && (
          <>
            <div className="w-16 h-16 mx-auto rounded-2xl bg-indigo-50 flex items-center justify-center mb-6">
              <Loader2 className="w-8 h-8 text-indigo-500 animate-spin" />
            </div>
            <h1 className="text-xl font-bold text-slate-800">Confirming your payment...</h1>
            <p className="text-slate-400 mt-2 text-sm">
              This usually takes just a few seconds.
            </p>
          </>
        )}

        {state === "confirmed" && (
          <>
            <div className="w-16 h-16 mx-auto rounded-2xl bg-emerald-50 flex items-center justify-center mb-6">
              <CheckCircle2 className="w-8 h-8 text-emerald-500" />
            </div>
            <h1 className="text-xl font-bold text-slate-800">Payment received!</h1>
            <p className="text-slate-400 mt-2 text-sm">
              Your membership has been activated. Welcome back!
            </p>
            <button
              onClick={() => navigate("/member-dashboard")}
              className="mt-6 w-full py-3 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white font-bold text-sm shadow-md shadow-indigo-500/25"
            >
              Go to Dashboard
            </button>
          </>
        )}

        {state === "pending" && (
          <>
            <div className="w-16 h-16 mx-auto rounded-2xl bg-amber-50 flex items-center justify-center mb-6">
              <Clock className="w-8 h-8 text-amber-500" />
            </div>
            <h1 className="text-xl font-bold text-slate-800">Still processing</h1>
            <p className="text-slate-400 mt-2 text-sm">
              Your payment is taking longer than usual to confirm. Check your
              payment history in a moment - your membership will activate as
              soon as it's confirmed.
            </p>
            <button
              onClick={() => navigate("/member-payment")}
              className="mt-6 w-full py-3 rounded-2xl border border-slate-200 text-slate-600 font-semibold text-sm hover:bg-slate-50"
            >
              View Payment History
            </button>
          </>
        )}
      </div>
    </div>
  );
}
