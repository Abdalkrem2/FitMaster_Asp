import React from "react";
import { NavLink } from "react-router-dom";
import { 
  X, 
  SquareActivity, 
  IdCard,
  ChevronRight
} from "lucide-react";
import { useAuth } from "../context/AuthContext";

interface MobileMoreMenuProps {
  isOpen: boolean;
  onClose: () => void;
}

export const MobileMoreMenu: React.FC<MobileMoreMenuProps> = ({ isOpen, onClose }) => {
  const { isAdmin } = useAuth();

  if (!isOpen) return null;

  const adminItems = [
    { name: "Activity Log", path: "/activity", icon: SquareActivity },
    { name: "Employees", path: "/employees", icon: IdCard },
  ];

  return (
    <div className="fixed inset-0 z-50 lg:hidden">
      {/* Backdrop */}
      <div 
        className="absolute inset-0 bg-slate-900/40 backdrop-blur-sm animate-in fade-in duration-300"
        onClick={onClose}
      />
      
      {/* Sheet */}
      <div className="absolute bottom-0 left-0 right-0 bg-white rounded-t-[32px] shadow-2xl animate-in slide-in-from-bottom duration-300 ease-out border-t border-slate-100 overflow-hidden">
        {/* Handle */}
        <div className="flex justify-center pt-3 pb-2">
          <div className="w-12 h-1.5 bg-slate-200 rounded-full" />
        </div>

        {/* Header */}
        <div className="px-6 py-4 flex items-center justify-between border-b border-slate-50">
          <h2 className="text-lg font-bold text-slate-800">More Options</h2>
          <button 
            onClick={onClose}
            className="p-2 bg-slate-50 rounded-full text-slate-400 hover:text-slate-600 transition-colors"
          >
            <X className="w-5 h-5" />
          </button>
        </div>

        <div className="px-4 py-6 space-y-6 max-h-[70vh] overflow-y-auto">
          {/* Main Actions */}
          {isAdmin() && (
            <div className="space-y-1">
              <p className="px-4 text-[11px] font-bold text-slate-400 uppercase tracking-wider mb-2">
                Management
              </p>
              {adminItems.map((item) => (
                <NavLink
                  key={item.name}
                  to={item.path}
                  onClick={onClose}
                  className="flex items-center justify-between p-4 rounded-2xl hover:bg-slate-50 transition-all group"
                >
                  <div className="flex items-center gap-4">
                    <div className="w-10 h-10 rounded-xl bg-indigo-50 flex items-center justify-center text-indigo-600 group-hover:scale-110 transition-transform">
                      <item.icon className="w-5 h-5" />
                    </div>
                    <span className="font-semibold text-slate-700">{item.name}</span>
                  </div>
                  <ChevronRight className="w-5 h-5 text-slate-300" />
                </NavLink>
              ))}
            </div>
          )}

        </div>

        {/* Footer padding for safe area */}
        <div className="h-10" />
      </div>
    </div>
  );
};
