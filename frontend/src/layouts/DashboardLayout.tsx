import React, { useState } from "react";
import { Outlet, Navigate, NavLink } from "react-router-dom";
import { Sidebar } from "../components/Sidebar";
import { EmployeeSidebar } from "../components/EmployeeSidebar";
import { Header } from "../components/Header";
import { useAuth } from "../context/AuthContext";
import { NotificationProvider } from "@/context/NotificationContext";
import {
  LayoutDashboard,
  Users,
  Package,
  DollarSign,
  MoreHorizontal
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

interface NavItem {
  name: string;
  path?: string;
  icon: LucideIcon;
  onClick?: () => void;
}

import { MobileMoreMenu } from "../components/MobileMoreMenu";

export const DashboardLayout: React.FC = () => {
  const { user, isAdmin } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [moreMenuOpen, setMoreMenuOpen] = useState(false);

  if (!user) return <Navigate to="/login" replace />;

  const toggleSidebar = () => setSidebarOpen((prev) => !prev);
  const closeSidebar = () => setSidebarOpen(false);

  const adminNav: NavItem[] = [
    { name: "Overview", path: "/", icon: LayoutDashboard },
    { name: "Members", path: "/members", icon: Users },
    { name: "Packages", path: "/packages", icon: Package },
    { name: "Revenue", path: "/revenue", icon: DollarSign },
    { name: "More", onClick: () => setMoreMenuOpen(true), icon: MoreHorizontal },
  ];

  const employeeNav: NavItem[] = [
    { name: "Dashboard", path: "/e-dashboard", icon: LayoutDashboard },
    { name: "Members", path: "/members", icon: Users },
  ];

  const currentNav = isAdmin() ? adminNav : employeeNav;

  return (
    <div className="flex h-screen bg-slate-50">
      {/* Sidebar — always visible on lg+, toggleable on mobile */}
      {isAdmin() ? (
        <Sidebar isOpen={sidebarOpen} onClose={closeSidebar} />
      ) : (
        <EmployeeSidebar isOpen={sidebarOpen} onClose={closeSidebar} />
      )}

      {/* Main content area */}
      <div className="flex-1 lg:ml-[260px] flex flex-col min-h-screen overflow-hidden transition-all duration-300">
        <NotificationProvider>
          <Header onMenuClick={toggleSidebar} />
        </NotificationProvider>

        <main className="flex-1 overflow-x-hidden overflow-y-auto bg-slate-50 p-4 sm:p-6 lg:p-8 pb-24 lg:pb-8">
          <div className="mx-auto max-w-7xl animate-fade-in-up">
            <Outlet />
          </div>
        </main>

        {/* Mobile Bottom Navigation */}
        <nav className="lg:hidden fixed bottom-0 left-0 right-0 z-40 bg-white/90 backdrop-blur-lg border-t border-slate-200 shadow-[0_-4px_20px_-10px_rgba(0,0,0,0.1)] pb-safe">
          <div className="flex items-center justify-around h-16 px-2">
            {currentNav.map((item) => {
              const Icon = item.icon;
              const isMore = !!item.onClick;
              
              if (isMore) {
                return (
                  <button
                    key="more"
                    onClick={item.onClick}
                    className="flex flex-col items-center justify-center w-full h-full space-y-1 text-slate-400 hover:text-slate-600 transition-colors"
                  >
                    <div className="flex items-center justify-center w-8 h-8 rounded-full bg-transparent">
                      <Icon className="w-5 h-5" />
                    </div>
                    <span className="text-[10px] font-semibold tracking-wide">
                      {item.name}
                    </span>
                  </button>
                );
              }

              return (
                <NavLink
                  key={item.name}
                  to={item.path!}
                  end={item.path === "/"}
                  className={({ isActive }: { isActive: boolean }) =>
                    `flex flex-col items-center justify-center w-full h-full space-y-1 transition-colors duration-200 ${
                      isActive ? "text-indigo-600" : "text-slate-400 hover:text-slate-600"
                    }`
                  }
                >
                  {({ isActive }: { isActive: boolean }) => (
                    <>
                      <div
                        className={`relative flex items-center justify-center w-8 h-8 rounded-full transition-all duration-300 ${
                          isActive ? "bg-indigo-50 scale-110" : "bg-transparent scale-100"
                        }`}
                      >
                        <Icon className={`w-5 h-5 ${isActive ? "animate-in zoom-in" : ""}`} />
                      </div>
                      <span className="text-[10px] font-semibold tracking-wide">
                        {item.name}
                      </span>
                    </>
                  )}
                </NavLink>
              );
            })}
          </div>
        </nav>

        {/* Modern Mobile More Menu (Bottom Sheet) */}
        <MobileMoreMenu 
          isOpen={moreMenuOpen} 
          onClose={() => setMoreMenuOpen(false)} 
        />
      </div>
    </div>
  );
};
