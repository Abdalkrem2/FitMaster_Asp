import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { LogIn, Activity, AlertCircle, Eye, EyeOff } from "lucide-react";
import { Button } from "../components/ui/Button";
import { authService } from "../services/authService";
import { useAuth } from "../context/AuthContext";

const Login: React.FC = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth(); //form context

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !password) {
      setError("Please enter both username and password.");
      return;
    }

    try {
      setLoading(true);
      setError("");
      const { token, user } = await authService.login(username, password);

      login(token, user); //to stor token && user & roles

      if (user.roles.includes("EMPLOYEE")) {
        navigate("/e-dashboard"); // Employee Dashboard
      } else if (user.roles.includes("ADMIN")) {
        navigate("/"); //Admin Dashboard
      } else if (user.roles.includes("MEMBER")) {
        navigate("/member-dashboard"); //Member Dashboard
      }
    } catch (err: any) {
      const backendErrors = err?.response?.data;
      const message = Array.isArray(backendErrors) ? backendErrors[0] : null;
      setError(message || "Invalid username or password.");
    } finally {
      setLoading(false);
    }
  };

  const getInputBorderClass = (hasError: boolean) =>
    hasError
      ? "border-rose-400 focus:border-rose-400 focus:ring-rose-500/20 dark:border-rose-500/50 dark:focus:border-rose-400"
      : "border-slate-200 dark:border-slate-700 focus:border-indigo-500 focus:ring-indigo-500/20 dark:focus:border-indigo-400";

  return (
    <div className="min-h-screen w-full flex bg-slate-50 dark:bg-slate-900 overflow-hidden font-sans selection:bg-indigo-500/30">
      {/* Left Panel - Image Background */}
      <div className="hidden lg:flex lg:w-1/2 relative bg-slate-950 items-center justify-center border-r border-slate-200/20 dark:border-slate-800">
        {/* Sleek dark gradient overlays for cinematic effect */}
        <div className="absolute inset-0 bg-gradient-to-t from-slate-950 via-slate-900/60 to-transparent" />
        <div className="absolute inset-0 bg-indigo-950/30 mix-blend-multiply" />
        <div className="absolute inset-0 bg-black/10" />

        <div className="relative z-10 px-12 text-center animate-fade-in-up">
          <div className="w-16 h-16 bg-white/10 backdrop-blur-md rounded-2xl border border-white/20 flex items-center justify-center mx-auto mb-8 shadow-2xl">
            <Activity className="w-8 h-8 text-white" />
          </div>
          <h1 className="text-5xl lg:text-6xl font-black text-white tracking-tight drop-shadow-xl mb-6">
            Push Your
            <br />
            <span className="text-transparent bg-clip-text bg-gradient-to-r from-indigo-400 to-violet-400 leading-tight">
              Limits.
            </span>
          </h1>
          <p className="text-lg text-slate-300 font-medium max-w-md mx-auto leading-relaxed drop-shadow-sm">
            The ultimate digital management platform for premium fitness centers
            and exclusive members.
          </p>
        </div>
      </div>

      {/* Right Panel - Login Form */}
      <div className="w-full lg:w-1/2 flex items-center justify-center p-6 sm:p-12 relative bg-white dark:bg-slate-900">
        {/* Subtle top decoration */}
        <div className="absolute top-0 right-0 w-full h-64 bg-gradient-to-b from-indigo-50/50 to-transparent dark:from-indigo-900/10 pointer-events-none" />

        <div className="w-full max-w-[420px] relative z-10 animate-in fade-in slide-in-from-bottom-8 duration-700">
          {/* Mobile Logo */}
          <div className="flex lg:hidden flex-col items-center mb-10">
            <div className="w-16 h-16 bg-gradient-to-br from-indigo-500 to-violet-600 rounded-2xl flex items-center justify-center shadow-xl shadow-indigo-500/20 mb-5 relative overflow-hidden">
              <div className="absolute inset-0 bg-white/20 blur-xl rounded-full" />
              <Activity className="w-8 h-8 text-white relative z-10" />
            </div>
            <h1 className="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight">
              FitMaster
            </h1>
          </div>

          <div className="mb-10 lg:text-left text-center">
            <h2 className="text-3xl sm:text-4xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-3">
              Welcome back
            </h2>
            <p className="text-base text-slate-500 dark:text-slate-400 font-medium">
              Enter your credentials to access your account.
            </p>
          </div>

          <form onSubmit={handleLogin} className="space-y-6">
            <div className="space-y-2">
              <label className="block text-sm font-bold text-slate-700 dark:text-slate-300 ml-0.5">
                Username or Phone
              </label>
              <input
                type="text"
                placeholder="Enter your username or phone"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className={`w-full h-12 text-[15px] px-4 bg-slate-50 dark:bg-slate-800 border ${getInputBorderClass(!!error)} text-slate-900 dark:text-white rounded-xl focus:bg-white dark:focus:bg-slate-900 focus:outline-none focus:ring-2 transition-all placeholder:text-slate-400 dark:placeholder:text-slate-500`}
                autoComplete="username"
              />
            </div>

            <div className="space-y-2 relative">
              <label className="block text-sm font-bold text-slate-700 dark:text-slate-300 ml-0.5">
                Password
              </label>
              <div className="relative">
                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className={`w-full h-12 text-[15px] px-4 pr-12 bg-slate-50 dark:bg-slate-800 border ${getInputBorderClass(!!error)} text-slate-900 dark:text-white rounded-xl focus:bg-white dark:focus:bg-slate-900 focus:outline-none focus:ring-2 transition-all placeholder:text-slate-400 dark:placeholder:text-slate-500`}
                  autoComplete="current-password"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 p-2 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 transition-colors focus:outline-none bg-transparent"
                  tabIndex={-1}
                >
                  {showPassword ? (
                    <EyeOff className="w-5 h-5" />
                  ) : (
                    <Eye className="w-5 h-5" />
                  )}
                </button>
              </div>
            </div>

            {error && (
              <div className="animate-in fade-in slide-in-from-top-1 px-4 py-3.5 rounded-xl bg-rose-50 dark:bg-rose-500/10 border border-rose-200/80 dark:border-rose-500/20 flex items-start gap-3 shadow-sm">
                <AlertCircle className="w-5 h-5 text-rose-500 flex-shrink-0 mt-0.5" />
                <p className="text-sm font-semibold text-rose-600 dark:text-rose-400 leading-snug">
                  {error}
                </p>
              </div>
            )}

            <div className="pt-2">
              <Button
                type="submit"
                fullWidth
                disabled={loading}
                className="h-12 text-[15px] font-extrabold bg-gradient-to-r from-indigo-500 to-violet-600 hover:from-indigo-600 hover:to-violet-700 text-white rounded-xl shadow-lg shadow-indigo-500/30 hover:shadow-xl hover:shadow-indigo-500/40 transform hover:-translate-y-0.5 transition-all duration-300 group disabled:opacity-70 disabled:transform-none disabled:cursor-not-allowed border-none"
              >
                {loading ? (
                  <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                ) : (
                  <div className="flex items-center justify-center">
                    <LogIn className="w-5 h-5 mr-2.5 group-hover:scale-110 transition-transform duration-300" />
                    Sign In
                  </div>
                )}
              </Button>
            </div>
          </form>

          <div className="mt-10 text-center">
            <p className="text-[13px] font-semibold text-slate-400 dark:text-slate-500 uppercase tracking-widest flex items-center justify-center gap-2">
              <span className="w-8 h-px bg-slate-200 dark:bg-slate-800" />
              Secure Access
              <span className="w-8 h-px bg-slate-200 dark:bg-slate-800" />
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
