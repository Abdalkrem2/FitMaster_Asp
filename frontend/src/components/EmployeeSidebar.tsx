import React from "react";
import { NavLink } from "react-router-dom";
import { Users, Settings, Activity, LayoutDashboard, X } from "lucide-react";

const navItems = [
  { name: "Dashboard", path: "/e-dashboard", icon: LayoutDashboard },
  { name: "Members", path: "/members", icon: Users },
];

interface EmployeeSidebarProps {
  isOpen?: boolean;
  onClose?: () => void;
}

export const EmployeeSidebar: React.FC<EmployeeSidebarProps> = ({
  isOpen = true,
  onClose,
}) => {
  return (
    <>
      {/* Mobile overlay */}
      {isOpen && onClose && (
        <div
          className="fixed inset-0 bg-black/20 backdrop-blur-sm z-30 lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`fixed inset-y-0 left-0 w-[260px] bg-white border-r border-slate-200/60 flex flex-col z-40 transition-transform duration-300 ease-out
          ${isOpen ? "translate-x-0" : "-translate-x-full"}
          lg:translate-x-0`}
      >
        {/* Logo */}
        <div className="h-16 flex items-center justify-between px-6 border-b border-slate-100">
          <h1 className="flex items-center gap-2 text-xl font-bold tracking-tight">
            <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-indigo-500 to-violet-600 flex items-center justify-center shadow-md shadow-indigo-500/20">
              <Activity className="w-4.5 h-4.5 text-white" />
            </div>
            <span className="bg-gradient-to-r from-indigo-600 to-violet-600 bg-clip-text text-transparent">
              FitMaster
            </span>
          </h1>
          {onClose && (
            <button
              onClick={onClose}
              className="lg:hidden p-1 rounded-lg hover:bg-slate-100 text-slate-400"
            >
              <X className="w-5 h-5" />
            </button>
          )}
        </div>

        {/* Navigation */}
        <nav className="flex-1 px-3 py-4 overflow-y-auto space-y-1">
          <p className="px-4 text-[11px] font-semibold text-slate-400 uppercase tracking-wider mb-2">
            Main
          </p>
          {navItems.map((item) => (
            <NavLink
              key={item.name}
              to={item.path}
              className={({ isActive }) =>
                `group flex items-center gap-3 px-4 py-2.5 text-[13px] font-medium rounded-xl transition-all duration-200 relative ${
                  isActive
                    ? "bg-indigo-50 text-indigo-600"
                    : "text-slate-500 hover:bg-slate-50 hover:text-slate-900"
                }`
              }
            >
              {({ isActive }) => (
                <>
                  {isActive && (
                    <div className="absolute left-0 top-1/2 -translate-y-1/2 w-[3px] h-5 bg-indigo-500 rounded-r-full" />
                  )}
                  <item.icon
                    className={`h-[18px] w-[18px] flex-shrink-0 transition-colors ${
                      isActive
                        ? "text-indigo-500"
                        : "text-slate-400 group-hover:text-slate-600"
                    }`}
                  />
                  {item.name}
                </>
              )}
            </NavLink>
          ))}
        </nav>


      </aside>
    </>
  );
};
