import { useNavigate } from "react-router-dom";
import { XCircle } from "lucide-react";

export default function PaymentCancelled() {
  const navigate = useNavigate();

  return (
    <div className="min-h-[70vh] flex items-center justify-center">
      <div className="max-w-md w-full text-center bg-white rounded-3xl border border-slate-100 shadow-sm p-10">
        <div className="w-16 h-16 mx-auto rounded-2xl bg-slate-50 flex items-center justify-center mb-6">
          <XCircle className="w-8 h-8 text-slate-400" />
        </div>
        <h1 className="text-xl font-bold text-slate-800">Checkout cancelled</h1>
        <p className="text-slate-400 mt-2 text-sm">
          No charge was made. You can pick a package and try again whenever
          you're ready.
        </p>
        <button
          onClick={() => navigate("/member-payment")}
          className="mt-6 w-full py-3 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white font-bold text-sm shadow-md shadow-indigo-500/25"
        >
          Back to Packages
        </button>
      </div>
    </div>
  );
}
