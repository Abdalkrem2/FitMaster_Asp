import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Calendar,
  Dumbbell,
  ChevronRight,
  Plus,
  Clock,
  Zap,
  BarChart3,
  Flame,
  Target,
  CheckCircle2,
  Award,
  Utensils,
} from "lucide-react";
import { memberService } from "../services/memberService";

const MemberDashboard: React.FC = () => {
  const navigate = useNavigate();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await memberService.getMyDetails();
        setData(res);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  const daysRemaining = data?.endDate
    ? Math.max(
        0,
        Math.ceil(
          (new Date(data.endDate).getTime() - new Date().getTime()) /
            (1000 * 60 * 60 * 24),
        ),
      )
    : 0;

  const isActive = daysRemaining > 0;
  const lastMembership = data?.memberships?.[0];
  const packageName = lastMembership?.packageName ?? "—";

  if (loading)
    return (
      <div className="flex h-64 items-center justify-center text-slate-400">
        Loading...
      </div>
    );

  return (
    <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      {/* ─── Greeting Banner ─── */}
      <div className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-indigo-600 via-indigo-500 to-violet-600 p-6 sm:p-8 shadow-lg shadow-indigo-500/25">
        <div className="absolute top-0 right-0 w-48 h-48 bg-white/5 rounded-full -translate-y-1/2 translate-x-1/4" />
        <div className="absolute bottom-0 left-12 w-32 h-32 bg-white/5 rounded-full translate-y-1/2" />
        <div className="relative z-10 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <p className="text-indigo-200 text-sm font-medium mb-1">
              Good morning
            </p>
            <h1 className="text-2xl sm:text-3xl uppercase font-bold text-white">
              {data?.fullName}
            </h1>
            <p className="text-indigo-200 text-sm mt-1.5 flex items-center gap-1.5">
              <Flame className="w-4 h-4 text-orange-300" />
              Ready to crush your goals today?
            </p>
          </div>
          <div className="flex items-center gap-2 bg-white/15 backdrop-blur-sm rounded-2xl px-4 py-3 self-start sm:self-auto border border-white/20">
            <Target className="w-5 h-5 text-indigo-200" />
            <div>
              <p className="text-white text-sm font-semibold">
                {data?.profile?.goal?.replace(/_/g, " ") ?? "General Fitness"}
              </p>
              <p className="text-indigo-200 text-xs">Current goal</p>
            </div>
          </div>
        </div>
      </div>

      {/* ─── Stats Row ─── */}
      <div className="grid grid-cols-2 gap-4">
        {[
          {
            icon: Flame,
            label: "Day Streak",
            value: "—",
            color: "text-orange-500",
            bg: "bg-orange-50",
            border: "border-orange-100",
          },
          {
            icon: Clock,
            label: "Days Remaining",
            value: `${daysRemaining}`,
            color: isActive ? "text-emerald-500" : "text-red-500",
            bg: isActive ? "bg-emerald-50" : "bg-red-50",
            border: isActive ? "border-emerald-100" : "border-red-100",
          },
        ].map(({ icon: Icon, label, value, color, bg, border }) => (
          <div
            key={label}
            className={`bg-white rounded-2xl border ${border} p-4 flex flex-col gap-3 shadow-sm hover:shadow-md transition-shadow`}
          >
            <div
              className={`w-9 h-9 rounded-xl ${bg} flex items-center justify-center`}
            >
              <Icon className={`w-4 h-4 ${color}`} />
            </div>
            <div>
              <p className="text-2xl font-bold text-slate-800">{value}</p>
              <p className="text-xs text-slate-400 font-medium mt-0.5">
                {label}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* ─── Two Column: Plans + Membership ─── */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* My Plans Card */}
        <div className="order-2 lg:order-1 lg:col-span-2 bg-white rounded-2xl border border-slate-100 shadow-sm overflow-hidden">
          <div className="px-6 py-5 border-b border-slate-50 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-lg bg-indigo-50 flex items-center justify-center">
                <BarChart3 className="w-4 h-4 text-indigo-600" />
              </div>
              <h2 className="text-base font-semibold text-slate-800">
                My Plans
              </h2>
            </div>
       
          </div>

          <div className="p-4 space-y-2">
            {[
              {
                icon: Dumbbell,
                label: "Workout Plan",
                desc: "View your workout plan",
                color: "text-indigo-600",
                bg: "bg-indigo-50",
                to: "/member-workout-plan",
              },
              {
                icon: Utensils,
                label: "Nutrition Plan",
                desc: "View your nutrition plan",
                color: "text-green-600",
                bg: "bg-green-50",
                to: "/member-nutrition-plan",
              },
            ].map(({ icon: Icon, label, desc, color, bg, to }) => (
              <button
                key={label}
                onClick={() => navigate(to)}
                className="w-full flex items-center justify-between p-4 rounded-xl hover:bg-slate-50 transition-colors group border border-transparent hover:border-slate-100"
              >
                <div className="flex items-center gap-3">
                  <div
                    className={`w-10 h-10 rounded-xl ${bg} flex items-center justify-center shrink-0`}
                  >
                    <Icon className={`w-5 h-5 ${color}`} />
                  </div>
                  <div className="text-left">
                    <p className="text-sm font-semibold text-slate-800">
                      {label}
                    </p>
                    <p className="text-xs text-slate-400 mt-0.5">{desc}</p>
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <span
                    className={`hidden sm:inline-flex items-center gap-1 text-xs font-medium px-2.5 py-1 rounded-full ${bg} ${color}`}
                  >
                    <CheckCircle2 className="w-3 h-3" />
                    View
                  </span>
                  <ChevronRight className="w-4 h-4 text-slate-300 group-hover:text-slate-500 transition-colors" />
                </div>
              </button>
            ))}
          </div>

          <div className="px-6 py-4 bg-slate-50/50 border-t border-slate-100">
            <button
              onClick={() => navigate("/member-plan-history")}
              className="w-full flex items-center justify-center gap-2 py-2.5 rounded-xl text-sm font-medium text-slate-500 hover:text-slate-700 hover:bg-slate-100 transition-all"
            >
              View all plans
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </div>

        {/* Membership + Subscription Sidebar */}
        <div className="order-1 lg:order-2 space-y-4">
          {/* Membership Status */}
          <div
            className={`relative overflow-hidden rounded-2xl p-6 border shadow-sm ${
              isActive
                ? "bg-gradient-to-br from-emerald-50 to-teal-50 border-emerald-100"
                : "bg-gradient-to-br from-red-50 to-rose-50 border-red-100"
            }`}
          >
            <div
              className={`absolute top-0 right-0 w-20 h-20 rounded-full opacity-30 ${
                isActive ? "bg-emerald-200" : "bg-red-200"
              } -translate-y-1/2 translate-x-1/3`}
            />
            <div className="flex items-center gap-2 mb-4">
              <Calendar
                className={`w-4 h-4 ${isActive ? "text-emerald-600" : "text-red-500"}`}
              />
              <p className="text-sm font-medium text-slate-500">Membership</p>
            </div>
            <p
              className={`text-3xl font-bold mb-1 ${isActive ? "text-emerald-600" : "text-red-500"}`}
            >
              {isActive ? "Active" : "Expired"}
            </p>
            <p className="text-sm text-slate-500">
              {isActive
                ? `${daysRemaining} days remaining`
                : "Please renew your membership"}
            </p>
            {isActive && (
              <div className="mt-4">
                <div className="flex justify-between text-xs text-slate-400 mb-1.5">
                  <span>Time left</span>
                  <span>
                    {Math.min(100, Math.round((daysRemaining / 30) * 100))}%
                  </span>
                </div>
                <div className="h-1.5 bg-emerald-100 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-emerald-500 rounded-full transition-all"
                    style={{
                      width: `${Math.min(100, Math.round((daysRemaining / 30) * 100))}%`,
                    }}
                  />
                </div>
              </div>
            )}
          </div>

          {/* Subscription Info */}
          <div className="bg-white rounded-2xl border border-slate-100 p-6 shadow-sm">
            <div className="flex items-center gap-2 mb-4">
              <Award className="w-4 h-4 text-indigo-500" />
              <p className="text-sm font-medium text-slate-500">Subscription</p>
            </div>
            <p className="text-2xl font-bold text-indigo-600 mb-1">
              {packageName}
            </p>
            <p className="text-xs text-slate-400 mt-1">
              {lastMembership?.timestamp
                ? `Registered on: ${new Date(
                    lastMembership.timestamp,
                  ).toLocaleDateString("en-US", {
                    month: "long",
                    day: "numeric",
                    year: "numeric",
                  })}`
                : "No subscription yet"}
            </p>
          </div>

          {/* Quick Action */}
          <button
            onClick={() => navigate("/member-workout-plan")}
            className="w-full flex items-center justify-center gap-2 px-4 py-3.5 rounded-2xl bg-gradient-to-r from-indigo-600 to-violet-600 text-white text-sm font-semibold shadow-md shadow-indigo-500/25 hover:shadow-lg hover:shadow-indigo-500/30 transition-all duration-200 hover:-translate-y-0.5"
          >
            <Zap className="w-4 h-4" />
            Start Today's Workout
          </button>
        </div>
      </div>
    </div>
  );
};

export default MemberDashboard;
