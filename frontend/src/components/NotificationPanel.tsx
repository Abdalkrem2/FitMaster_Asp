// src/components/NotificationPanel.tsx
import React from "react";
import { X, CheckCheck, Trash2, Bell } from "lucide-react";
import { useNotificationContext } from "../context/NotificationContext";

interface ActionConfig {
  label: string;
  color: string;
  bg: string;
}

const actionConfig: ActionConfig[] = [
  { label: "Created", color: "text-emerald-700", bg: "bg-emerald-100" },
  { label: "Added", color: "text-blue-700", bg: "bg-blue-100" },
  { label: "Updated", color: "text-amber-700", bg: "bg-amber-100" },
  { label: "Deleted", color: "text-rose-700", bg: "bg-rose-100" },
  { label: "Renewed", color: "text-purple-700", bg: "bg-purple-100" },
  { label: "Logged in", color: "text-slate-700", bg: "bg-slate-100" },
];

function getActionConfig(message: string): ActionConfig {
  const lower = message.toLowerCase();
  if (lower.includes("deleted") || lower.includes("delete"))
    return actionConfig[3];
  if (lower.includes("created") || lower.includes("create"))
    return actionConfig[0];
  if (lower.includes("added") || lower.includes("add")) return actionConfig[1];
  if (lower.includes("updated") || lower.includes("update"))
    return actionConfig[2];
  if (lower.includes("renewed") || lower.includes("renew"))
    return actionConfig[4];
  if (lower.includes("logged in") || lower.includes("login"))
    return actionConfig[5];
  return actionConfig[5];
}

function formatDate(dateStr: string): string {
  try {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;

    return date.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch {
    return dateStr;
  }
}

interface Props {
  onClose: () => void;
}

export const NotificationPanel: React.FC<Props> = ({ onClose }) => {
  const {
    notifications,
    unreadCount,
    error,
    markAsRead,
    markAllAsRead,
    clearNotification,
    clearAll,
  } = useNotificationContext();

  return (
    // Overlay — click outside to close
    <div className="fixed inset-0 z-40" onClick={onClose}>
      <div
        className="absolute top-16 right-4 w-80 sm:w-96 bg-white rounded-2xl shadow-2xl
                   border border-slate-200 overflow-hidden z-50"
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between px-4 py-3 border-b border-slate-100">
          <div className="flex items-center gap-2">
            <Bell className="h-4 w-4 text-slate-500" />
            <span className="font-semibold text-slate-700">Notifications</span>
            {unreadCount > 0 && (
              <span className="bg-rose-500 text-white text-[10px] font-bold px-1.5 py-0.5 rounded-full">
                {unreadCount}
              </span>
            )}
          </div>
          <div className="flex items-center gap-1">
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                title="Mark all as read"
                className="p-1.5 rounded-lg text-slate-400 hover:text-indigo-600
                           hover:bg-indigo-50 transition-colors"
              >
                <CheckCheck className="h-4 w-4" />
              </button>
            )}
            {notifications.length > 0 && (
              <button
                onClick={clearAll}
                title="Clear all"
                className="p-1.5 rounded-lg text-slate-400 hover:text-rose-500
                           hover:bg-rose-50 transition-colors"
              >
                <Trash2 className="h-4 w-4" />
              </button>
            )}
            <button
              onClick={onClose}
              className="p-1.5 rounded-lg text-slate-400 hover:text-slate-600
                         hover:bg-slate-100 transition-colors"
            >
              <X className="h-4 w-4" />
            </button>
          </div>
        </div>

        {/* List */}
        <div className="max-h-[420px] overflow-y-auto divide-y divide-slate-50">
          {error ? (
            <div className="p-4 text-sm text-rose-700 bg-rose-50 border-b border-rose-100">
              Unable to load notifications: {error}
            </div>
          ) : notifications.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-12 text-slate-400">
              <Bell className="h-8 w-8 mb-2 opacity-40" />
              <p className="text-sm">No notifications yet</p>
            </div>
          ) : (
            notifications.map((n: (typeof notifications)[number]) => {
              const config = getActionConfig(n.message);

              return (
                <div
                  key={n.notificationId}
                  onClick={() => markAsRead(n.notificationId)}
                  className={`flex gap-3 px-4 py-3 cursor-pointer transition-colors hover:bg-slate-50 ${
                    !n.read ? "bg-indigo-50/40" : "bg-white"
                  }`}
                >
                  <div className="flex-shrink-0 mt-0.5">
                    <span
                      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-[10px] font-bold ${config.color} ${config.bg}`}
                    >
                      {config.label}
                    </span>
                  </div>

                  <div className="flex-1 min-w-0">
                    <p className="text-sm text-slate-700 font-medium truncate">
                      {n.message}
                    </p>
                    {n.details && (
                      <p className="text-xs text-slate-400 mt-0.5">
                        {n.details}
                      </p>
                    )}
                    <p className="text-xs text-slate-400 mt-0.5">
                      {formatDate(n.createdAt)}
                    </p>
                  </div>

                  <div className="flex flex-col items-end gap-1 flex-shrink-0">
                    {!n.read && (
                      <span className="h-2 w-2 rounded-full bg-indigo-500 mt-1" />
                    )}
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        clearNotification(n.notificationId);
                      }}
                      className="p-1 rounded text-slate-400 hover:text-rose-500 hover:bg-rose-50 transition-colors"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </div>
                </div>
              );
            })
          )}
        </div>
      </div>
    </div>
  );
};
