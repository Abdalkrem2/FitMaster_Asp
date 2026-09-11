import React, { useEffect, useState, useMemo } from "react";
import { useNavigate } from "react-router-dom";
import {
  Activity as ActivityIcon,
  Plus,
  Pencil,
  Trash2,
  Calendar,
  Filter,
  Search,
  UserPlus,
  PackagePlus,
  Clock,
} from "lucide-react";

import { Card } from "../components/ui/Card";
import { Button } from "../components/ui/Button";
import { employeeService } from "../services/employeeService";
import type { ActivityLogItem } from "../types/activityLog";
import type { Employee } from "../types/employee";
import { useGetLogs } from "@/hooks/useGetLogs";

// Helper to format realtive times like "2 minutes ago" and group dates
const timeAgo = (dateStr: string) => {
  const date = new Date(dateStr);
  const now = new Date();
  const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (seconds < 60) return `${seconds} seconds ago`;
  const minutes = Math.floor(seconds / 60);
  if (minutes < 60) return `${minutes} minutes ago`;
  const hours = Math.floor(minutes / 60);
  if (hours < 24) return `${hours} hours ago`;
  const days = Math.floor(hours / 24);
  if (days < 30) return `${days} days ago`;

  return date.toLocaleDateString();
};

const getDateGroup = (dateStr: string) => {
  const date = new Date(dateStr);
  const today = new Date();
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);

  if (date.toDateString() === today.toDateString()) return "Today";
  if (date.toDateString() === yesterday.toDateString()) return "Yesterday";

  return date.toLocaleDateString("en-US", {
    month: "long",
    day: "numeric",
    year: "numeric",
  });
};

const ActivityLog: React.FC = () => {
  const navigate = useNavigate();
  const [activities, setActivities] = useState<ActivityLogItem[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [page, setPage] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [performedBy, setPerformedBy] = useState<string>("");
  const [entityType, setEntityType] = useState<string>("");
  const [searchQuery, setSearchQuery] = useState("");
  const [size, setSize] = useState(20);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchEmployees = async () => {
      try {
        const data = await employeeService.getAllEmployees(0, 1000);
        setEmployees(data.content);
      } catch (err) {
        console.error("Failed to load employees for filter", err);
      }
    };
    fetchEmployees();
  }, []);

  const fetchLogs = async () => {
    setLoading(true);
    const data = await useGetLogs(performedBy, entityType, page, size);
    if (data) {
      setActivities(data.content || []);
      setTotalPages(data.totalPages || 0);
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchLogs();
  }, [page, performedBy, entityType]);

  // Grouping logic based on JS dates
  const groupedActivities = useMemo(() => {
    // Optionally filter by text here on the client-side
    const filtered = activities.filter(
      (a) =>
        (a.details ?? "").toLowerCase().includes(searchQuery.toLowerCase()) ||
        a.performedByName.toLowerCase().includes(searchQuery.toLowerCase()),
    );

    const groups: Record<string, ActivityLogItem[]> = {};
    filtered.forEach((activity) => {
      const group = getDateGroup(activity.createdAt.toString());
      if (!groups[group]) {
        groups[group] = [];
      }
      groups[group].push(activity);
    });
    return groups;
  }, [activities, searchQuery]);

  const getActivityVisuals = (type: string) => {
    switch (type) {
      case "CREATE":
      case "MEMBERSHIP":
        return {
          badge: (
            <div className="absolute -bottom-1 -right-1 bg-emerald-500 text-white rounded-full p-1 border-2 border-white shadow-sm">
              <Plus className="w-2.5 h-2.5" />
            </div>
          ),
          accentColor: "border-l-emerald-500 bg-emerald-50 text-emerald-700",
        };
      case "UPDATE":
        return {
          badge: (
            <div className="absolute -bottom-1 -right-1 bg-blue-500 text-white rounded-full p-1 border-2 border-white shadow-sm">
              <Pencil className="w-2.5 h-2.5" />
            </div>
          ),
          accentColor: "border-l-blue-500 bg-blue-50 text-blue-700",
        };
      case "DELETE":
        return {
          badge: (
            <div className="absolute -bottom-1 -right-1 bg-rose-500 text-white rounded-full p-1 border-2 border-white shadow-sm">
              <Trash2 className="w-2.5 h-2.5" />
            </div>
          ),
          accentColor: "border-l-rose-500 bg-rose-50 text-rose-700",
        };
      default:
        return {
          badge: (
            <div className="absolute -bottom-1 -right-1 bg-slate-500 text-white rounded-full p-1 border-2 border-white shadow-sm">
              <ActivityIcon className="w-2.5 h-2.5" />
            </div>
          ),
          accentColor: "border-l-slate-500 bg-slate-50 text-slate-700",
        };
    }
  };

  // Helper to parse the action "details" to make entities bold
  // Example: "Added member John Doe" -> make "John Doe" bold if possible.
  // We'll wrap words matching entityType rules
  const renderRichText = (details: string, type: string) => {
    return <span className="text-slate-600 font-medium">{details}</span>;
  };

  return (
    <div className="space-y-6 max-w-5xl mx-auto">
      {/* Header Area */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight">
            Activity Log
          </h2>
          <p className="text-sm text-slate-500 mt-1">
            Real-time audit trail of all system activities.
          </p>
        </div>
      </div>

      {/* Pill-style Filters */}
      <div className="flex flex-wrap justify-end gap-3 items-center bg-white p-3 rounded-2xl border border-slate-200 shadow-sm">
        <div className="flex items-center gap-2 border-l border-slate-100 pl-3">
          <div className="relative">
            <select
              value={performedBy}
              onChange={(e) => {
                setPerformedBy(e.target.value);
                setPage(0);
              }}
              className="appearance-none pl-4 pr-9 py-2 bg-slate-50 hover:bg-slate-100 border border-slate-200 focus:border-indigo-400 focus:ring-2 focus:ring-indigo-500/20 rounded-full text-sm font-medium text-slate-700 cursor-pointer transition-all outline-none"
            >
              <option value="">All Employees</option>
              {employees.map((emp) => (
                <option key={emp.id} value={emp.id.toString()}>
                  {emp.fullName}
                </option>
              ))}
            </select>
            <Filter className="absolute right-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" />
          </div>

          <div className="relative">
            <select
              value={entityType}
              onChange={(e) => {
                setEntityType(e.target.value);
                setPage(0);
              }}
              className="appearance-none pl-4 pr-9 py-2 bg-slate-50 hover:bg-slate-100 border border-slate-200 focus:border-indigo-400 focus:ring-2 focus:ring-indigo-500/20 rounded-full text-sm font-medium text-slate-700 cursor-pointer transition-all outline-none"
            >
              <option value="">All Entities</option>
              <option value="MEMBER">Member</option>
              <option value="EMPLOYEE">Employee</option>
              <option value="MEMBERSHIP">Membership</option>
            </select>
            <Filter className="absolute right-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-slate-400 pointer-events-none" />
          </div>
        </div>
      </div>

      {/* Main Content Card */}
      <Card padding="lg" className="border border-slate-200 min-h-[500px]">
        {loading ? (
          <div className="flex flex-col items-center justify-center h-[400px] gap-4 animate-fade-in">
            <div className="relative">
              <div className="w-12 h-12 border-2 border-indigo-100 rounded-full animate-spin" />
              <div className="w-12 h-12 border-2 border-indigo-600 border-t-transparent rounded-full animate-spin absolute inset-0 style={{ animationDirection: 'reverse' }}" />
            </div>
            <span className="text-sm font-semibold text-slate-500">
              Syncing live activities...
            </span>
          </div>
        ) : Object.keys(groupedActivities).length === 0 ? (
          // Rich Empty State
          <div className="flex flex-col items-center justify-center py-20 text-center animate-fade-in-up">
            <div className="w-24 h-24 bg-slate-50 rounded-full flex items-center justify-center border-4 border-white shadow-sm ring-1 ring-slate-100 mb-6">
              <Clock className="w-10 h-10 text-slate-300" />
            </div>
            <h3 className="text-xl font-bold text-slate-900 tracking-tight">
              No activity yet
            </h3>
            <p className="text-sm text-slate-500 mt-2 max-w-sm mb-8 leading-relaxed">
              When actions occur, like adding members or updating packages, they
              will appear chronologically here.
            </p>
            <div className="flex items-center gap-3">
              <Button
                onClick={() => navigate("/members")}
                className="bg-indigo-600 hover:bg-indigo-700 text-white shadow-sm hover:shadow-md rounded-xl"
              >
                <UserPlus className="w-4 h-4 mr-2" /> Add Member
              </Button>
              <Button
                onClick={() => navigate("/packages")}
                variant="outline"
                className="bg-white rounded-xl"
              >
                <PackagePlus className="w-4 h-4 mr-2" /> Create Package
              </Button>
            </div>
          </div>
        ) : (
          <div className="space-y-8 animate-fade-in relative">
            {/* Master timeline line running behind all groups */}
            <div className="absolute left-[20px] top-4 bottom-4 w-px bg-slate-200/60 z-0 hidden sm:block" />

            {Object.entries(groupedActivities).map(([dateLabel, items]) => (
              <div key={dateLabel} className="relative z-10">
                {/* Date Group Header */}
                <div className="flex items-center gap-4 mb-5">
                  <div className="w-10 flex justify-center hidden sm:flex">
                    <div className="w-2.5 h-2.5 rounded-full bg-slate-300 ring-4 ring-white" />
                  </div>
                  <h3 className="text-[13px] font-bold text-slate-800 uppercase tracking-wider bg-slate-100 px-3 py-1 rounded-full">
                    {dateLabel}
                  </h3>
                </div>

                <div className="space-y-4">
                  {items.map((activity) => {
                    const { badge, accentColor } = getActivityVisuals(
                      activity.actionType.toString(),
                    );
                    const initials = activity.performedByName
                      .slice(0, 2)
                      .toUpperCase();

                    return (
                      <div
                        key={activity.id}
                        className="flex group items-start relative pb-2 sm:pl-[24px]"
                      >
                        {/* User Avatar with Badge */}
                        <div className="relative mr-4 mt-0.5 z-10 flex-shrink-0">
                          <div
                            className={`w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold shadow-sm ${accentColor} border-l-0`}
                          >
                            {initials}
                          </div>
                          {badge}
                        </div>

                        {/* Interactive Log Card */}
                        <div className="flex-1">
                          <div className="bg-white rounded-2xl p-4 border border-slate-100 shadow-sm hover:shadow-md hover:border-slate-200 transition-all duration-200 group-hover:-translate-y-[1px]">
                            <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-2">
                              <p className="text-[14px] leading-relaxed text-slate-700">
                                <span className="font-bold text-slate-900">
                                  {activity.performedByName}
                                </span>{" "}
                                {renderRichText(
                                  (activity.details ?? "").toString(),
                                  activity.actionType.toString(),
                                )}
                              </p>
                              <span className="text-[11px] font-bold text-slate-400 whitespace-nowrap bg-slate-50 px-2 py-1 rounded-md sm:ml-4 self-start">
                                {timeAgo(activity.createdAt.toString())}
                              </span>
                            </div>

                            <div className="flex items-center gap-2 mt-2">
                              <span
                                className={`text-[10px] font-extrabold uppercase tracking-widest px-2 py-0.5 rounded-sm border-l-2 ${accentColor}`}
                              >
                                {activity.entityType}
                              </span>
                              <span className="text-[11px] text-slate-400 font-medium">
                                #{activity.entityId}
                              </span>
                            </div>
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            ))}
          </div>
        )}

        {!loading && totalPages > 1 && (
          <div className="flex justify-center mt-10 pt-6 border-t border-slate-100">
            <Button
              variant="outline"
              disabled={page >= totalPages - 1}
              onClick={() => setPage((p) => p + 1)}
              className="w-full sm:w-auto bg-white border-slate-200 shadow-sm hover:shadow-md border-2 hover:bg-slate-50 text-slate-600 rounded-xl px-8"
            >
              {page >= totalPages - 1
                ? "No More Activities"
                : "Load Older Activities"}
            </Button>
          </div>
        )}
      </Card>
    </div>
  );
};

export default ActivityLog;
