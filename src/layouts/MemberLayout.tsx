import { useState, useEffect } from "react";
import { Outlet, Navigate, NavLink, useNavigate } from "react-router-dom";
import { memberService } from "../services/memberService";
import { useAuth } from "../context/AuthContext";
import {
  Activity,
  LayoutDashboard,
  User,
  Dumbbell,
  Utensils,
  LogOut,
  History,
  Settings,
  CreditCard,
} from "lucide-react";

const navItems = [
  { to: "/member-dashboard", icon: LayoutDashboard, label: "Dashboard" },
  { to: "/member-workout-plan", icon: Dumbbell, label: "Workout Plan" },
  { to: "/member-nutrition-plan", icon: Utensils, label: "Nutrition Plan" },
  { to: "/member-plan-history", icon: History, label: "History" },
  { to: "/member-payment", icon: CreditCard, label: "Pay Online" },
  { to: "/member-profile", icon: User, label: "Profile" },
  { to: "/member-settings", icon: Settings, label: "Settings" },
];

export const MemberLayout: React.FC = () => {
  const { user, logout, isMember } = useAuth();
  const navigate = useNavigate();
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await memberService.getMyDetails();
        setData(res);
      } catch (err) {
        console.error(err);
      }
    };
    fetchData();
  }, []);

  // Auto-redirect to onboarding if profile has never been filled in.
  useEffect(() => {
    if (!user) return;

    memberService
      .getMyProfile()
      .then((profile) => {
        if (!profile?.trainingStyle) {
          navigate("/member-onboarding", { replace: true });
        }
      })
      .catch(() => {});
  }, [user, navigate]);

  if (!user) return <Navigate to="/login" replace />;
  if (!isMember()) return <Navigate to="/" replace />;

  return (
    <div className="min-h-screen bg-slate-50 flex flex-col">
      {/* ─── Top Header ─── */}
      <header className="sticky top-0 z-40 bg-white/80 backdrop-blur-md border-b border-slate-100 shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 h-16 flex items-center justify-between gap-4">
          {/* Logo */}
          <NavLink
            to="/member-dashboard"
            className="flex items-center gap-2.5 shrink-0"
          >
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-md shadow-indigo-500/25">
              <Activity className="w-4 h-4 text-white" />
            </div>
            <span className="text-xl font-bold tracking-tight bg-gradient-to-r from-indigo-600 to-violet-600 bg-clip-text text-transparent">
              FitMaster
            </span>
          </NavLink>

          {/* Desktop Nav */}
          <nav className="hidden md:flex items-center gap-1">
            {navItems.map(({ to, icon: Icon, label }) => (
              <NavLink
                key={to}
                to={to}
                className={({ isActive }) =>
                  `flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-medium transition-all duration-200 ${
                    isActive
                      ? "bg-indigo-50 text-indigo-600"
                      : "text-slate-500 hover:text-slate-800 hover:bg-slate-100"
                  }`
                }
              >
                <Icon className="w-4 h-4" />
                {label}
              </NavLink>
            ))}
          </nav>

          {/* Desktop Right Side */}
          <div className="hidden md:flex items-center gap-3">

            <button
              onClick={logout}
              className="flex items-center gap-2 px-4 py-2 rounded-xl border border-slate-200 text-sm font-medium text-slate-600 hover:bg-slate-50 hover:border-slate-300 transition-all duration-200"
            >
              <LogOut className="w-4 h-4" />
              Logout
            </button>
          </div>

          {/* Mobile Logout */}
          <button
            onClick={logout}
            className="md:hidden p-2 rounded-xl hover:bg-rose-50 text-slate-500 hover:text-rose-600 transition-colors"
            title="Log out"
          >
            <LogOut className="w-5 h-5" />
          </button>
        </div>
      </header>



      {/* ─── Page Content ─── */}
      <main className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 py-8 pb-24 md:pb-8">
        <Outlet />
      </main>

      {/* ─── Mobile Bottom Navigation ─── */}
      <nav className="md:hidden fixed bottom-0 left-0 right-0 z-40 bg-white/90 backdrop-blur-lg border-t border-slate-200 shadow-[0_-4px_20px_-10px_rgba(0,0,0,0.1)] pb-safe">
        <div className="flex items-center justify-around h-16 px-2">
          {navItems.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `flex flex-col items-center justify-center w-full h-full space-y-1 transition-colors duration-200 ${
                  isActive ? "text-indigo-600" : "text-slate-400 hover:text-slate-600"
                }`
              }
            >
              {({ isActive }) => (
                <>
                  <div
                    className={`relative flex items-center justify-center w-8 h-8 rounded-full transition-all duration-300 ${
                      isActive ? "bg-indigo-50 scale-110" : "bg-transparent scale-100"
                    }`}
                  >
                    <Icon className={`w-5 h-5 ${isActive ? "animate-in zoom-in" : ""}`} />
                  </div>
                  <span className="text-[10px] font-semibold tracking-wide">
                    {label}
                  </span>
                </>
              )}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  );
};
