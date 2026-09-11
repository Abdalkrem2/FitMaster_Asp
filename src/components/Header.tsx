import React from "react";
import { Bell, LogOut, Menu } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { useNotificationContext } from "@/context/NotificationContext";
import { NotificationPanel } from "@/components/NotificationPanel";
import { useState } from "react";

interface HeaderProps {
  onMenuClick?: () => void;
}

export const Header: React.FC<HeaderProps> = ({ onMenuClick }) => {
  const navigate = useNavigate();
  const { user, logout, isAdmin, isEmployee } = useAuth();
  const { unreadCount } = useNotificationContext();
  const [showNotifications, setShowNotifications] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const displayName = user?.name || "User";
  let roleBadge = "";
  if (isAdmin()) {
    roleBadge = "Administrator";
  } else if (isEmployee()) {
    roleBadge = "Staff";
  } else {
    roleBadge = "Member";
  }

  return (
    <header className="h-16 glass-strong border-b border-slate-200/60 flex items-center justify-between px-4 sm:px-8 z-10 sticky top-0 transition-all">
      {/* Left side — hamburger + search */}
      <div className="flex items-center gap-3">
        {/* Mobile menu toggle */}
        {/* Mobile menu toggle — hidden on mobile since we have bottom bar */}
        {onMenuClick && (
          <button
            onClick={onMenuClick}
            className="hidden lg:hidden p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition-colors"
          >
            <Menu className="h-5 w-5" />
          </button>
        )}
      </div>

      {/* Right side — notifications + profile + logout */}
      <div className="flex items-center gap-1 sm:gap-2">
        {/* Notification bell */}
        <div className="relative">
          <button
            onClick={() => setShowNotifications((v) => !v)}
            className="relative p-2.5 rounded-xl text-slate-400 hover:text-slate-600
                       hover:bg-slate-100 transition-all duration-200"
            title="Notifications"
          >
            <Bell className="h-[18px] w-[18px]" />
            {unreadCount > 0 && (
              <span
                className="absolute top-1.5 right-1.5 h-4 w-4 flex items-center
                               justify-center rounded-full bg-rose-500 text-white
                               text-[9px] font-bold ring-2 ring-white"
              >
                {unreadCount > 9 ? "9+" : unreadCount}
              </span>
            )}
          </button>

          {showNotifications && (
            <NotificationPanel onClose={() => setShowNotifications(false)} />
          )}
        </div>

        {/* Divider */}
        <div className="h-6 w-px bg-slate-200 mx-1 hidden sm:block" />

        {/* Profile */}
        <div className="flex items-center gap-3 cursor-pointer group px-2 py-1.5 rounded-xl hover:bg-slate-50 transition-all duration-200">
          <div className="h-8 w-8 rounded-lg bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center text-white text-sm font-semibold shadow-sm shadow-indigo-500/20 group-hover:shadow-md group-hover:shadow-indigo-500/30 transition-shadow">
            {displayName[0] || "U"}
          </div>
          <div className="hidden sm:block">
            <p className="text-sm font-semibold text-slate-700 leading-tight">
              {displayName}
            </p>
            <p className="text-[11px] text-slate-400 leading-tight">
              {roleBadge}
            </p>
          </div>
        </div>

        {/* Logout */}
        <button
          onClick={handleLogout}
          title="Logout"
          className="p-2.5 rounded-xl text-slate-400 hover:text-rose-500 hover:bg-rose-50 transition-all duration-200"
        >
          <LogOut className="h-[18px] w-[18px]" />
        </button>
      </div>
    </header>
  );
};
