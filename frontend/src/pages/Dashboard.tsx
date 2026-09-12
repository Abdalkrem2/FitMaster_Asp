import React, { useEffect, useState } from "react";
import {
  Users,
  DollarSign,
  AlertCircle,
  TrendingUp,
  TrendingDown,
  UserPlus,
  ActivityIcon,
  RefreshCw,
  Trash2,
  Calendar,
  Package,
  ArrowRight,
  Plus,
  Clock,
  Zap,
} from "lucide-react";
import {
  dashboardService,
  type DashboardStats,
} from "../services/dashboardService";
import type { ActivityLogItem } from "../types/activityLog";
import { Card } from "../components/ui/Card";
import { useGetLogs } from "@/hooks/useGetLogs";
import { useNavigate } from "react-router-dom";

/* ─── Stat Card ─────────────────────────────────────────── */
interface StatCardProps {
  title: string;
  value: string | number;
  icon: React.ReactNode;
  iconBgClass: string;
  trend?: { value: string; positive: boolean };
  delay?: number;
}

const StatCard: React.FC<StatCardProps> = ({
  title,
  value,
  icon,
  iconBgClass,
  trend,
  delay = 0,
}) => (
  <Card
    hover
    className="group relative overflow-hidden"
    // style={{ animationDelay: `${delay}ms` } as React.CSSProperties} بعطي عليها ايررور
  >
    {/* Decorative gradient orb */}
    <div className={`absolute -top-8 -right-8 w-24 h-24 rounded-full opacity-[0.07] ${iconBgClass} blur-2xl group-hover:opacity-[0.12] transition-opacity duration-500`} />

    <div className="flex items-start justify-between relative z-10">
      <div className="space-y-3">
        <p className="text-[13px] font-medium text-slate-500 tracking-wide">
          {title}
        </p>
        <h4 className="text-3xl font-bold text-slate-900 tracking-tight">
          {value}
        </h4>
        {trend && (
          <div className="flex items-center gap-1.5">
            {trend.positive ? (
              <div className="flex items-center gap-1 text-emerald-600 bg-emerald-50 px-2 py-0.5 rounded-full">
                <TrendingUp className="w-3.5 h-3.5" />
                <span className="text-xs font-semibold">{trend.value}</span>
              </div>
            ) : (
              <div className="flex items-center gap-1 text-rose-600 bg-rose-50 px-2 py-0.5 rounded-full">
                <TrendingDown className="w-3.5 h-3.5" />
                <span className="text-xs font-semibold">{trend.value}</span>
              </div>
            )}
            <span className="text-[11px] text-slate-400">vs last month</span>
          </div>
        )}
      </div>
      <div
        className={`p-3 rounded-xl ${iconBgClass} shadow-sm group-hover:scale-110 transition-transform duration-300`}
      >
        {icon}
      </div>
    </div>
  </Card>
);



/* ─── Dashboard Page ───────────────────────────────────── */
const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [activities, setActivities] = useState<ActivityLogItem[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const [statsData, activityData] = await Promise.all([
          dashboardService.getStats(),
          useGetLogs(null, null, 0, 50),
        ]);
        setStats(statsData);
        setActivities(activityData?.content ?? []);
      } catch (error) {
        console.error("Failed to load dashboard data");
      } finally {
        setLoading(false);
      }
    };
    fetchDashboardData();
  }, []);

  const getActivityIcon = (type: string) => {
    switch (type) {
      case "CREATE":
        return <UserPlus className="w-4 h-4 text-indigo-600" />;
      case "ADD":
        return <DollarSign className="w-4 h-4 text-emerald-600" />;
      case "UPDATE":
        return <RefreshCw className="w-4 h-4 text-violet-600" />;
      case "DELETE":
        return <Trash2 className="w-4 h-4 text-rose-600" />;
      default:
        return <ActivityIcon className="w-4 h-4 text-slate-600" />;
    }
  };

  const getActivityColor = (type: string) => {
    switch (type) {
      case "CREATE":
        return "bg-indigo-50 border-indigo-100 text-indigo-600";
      case "ADD":
        return "bg-emerald-50 border-emerald-100 text-emerald-600";
      case "UPDATE":
        return "bg-violet-50 border-violet-100 text-violet-600";
      case "DELETE":
        return "bg-rose-50 border-rose-100 text-rose-600";
      default:
        return "bg-slate-50 border-slate-100 text-slate-600";
    }
  };

  const getGreeting = () => {
    const hour = new Date().getHours();
    if (hour < 12) return "Good morning";
    if (hour < 17) return "Good afternoon";
    return "Good evening";
  };

  const formatDate = () => {
    return new Date().toLocaleDateString("en-US", {
      weekday: "long",
      month: "long",
      day: "numeric",
      year: "numeric",
    });
  };

  if (loading || !stats) {
    return (
      <div className="flex flex-col items-center justify-center h-64 gap-3">
        <div className="w-8 h-8 border-2 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
        <p className="text-sm text-slate-400 font-medium">Loading dashboard...</p>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      {/* ── Welcome header ─────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4">
        <div className="space-y-1">
          <h2 className="text-2xl sm:text-3xl font-bold text-slate-900 tracking-tight">
            {getGreeting()}, Admin 👋
          </h2>
          <div className="flex items-center gap-2 text-sm text-slate-400">
            <Calendar className="w-3.5 h-3.5" />
            {formatDate()}
          </div>
        </div>
      
      </div>

      {/* ── Stat cards ─────────────────────────────────── */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6 stagger-children  text-red-400">
        <StatCard
          title="Active Members"
          value={stats.activeMembers}
          icon={<Users className="w-5 h-5 text-indigo-600" />}
          iconBgClass="bg-indigo-100"
          trend={{ value: "+12%", positive: true }}
          delay={0}
        />
        <StatCard
          title="Monthly Revenue"
          value={`$${stats.monthlyRevenue.toLocaleString()}`}
          icon={<DollarSign className="w-5 h-5 text-emerald-600" />}
          iconBgClass="bg-emerald-100"
          trend={{ value: "+8.2%", positive: true }}
          delay={80}
        />
        <StatCard
          title="Expiring Soon"
          value={stats.expiringSoon}
          icon={<AlertCircle className="w-5 h-5 text-amber-600" />}
          iconBgClass="bg-amber-100"
          trend={
            stats.expiringSoon > 0
              ? { value: `${stats.expiringSoon} members`, positive: false }
              : undefined
          }
          delay={160}
        />
      </div>

      {/* ── Bottom section: Activity + Summary ──────── */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        {/* Activity feed — takes 2/3 */}
        <Card className="lg:col-span-2" padding="none">
          <div className="p-6 pb-0 flex items-center justify-between">
            <div className="flex items-center gap-2.5">
              <div className="p-2 rounded-lg bg-slate-100">
                <Clock className="w-4 h-4 text-slate-600" />
              </div>
              <div>
                <h3 className="text-[15px] font-semibold text-slate-900">
                  Recent Activity
                </h3>
                <p className="text-xs text-slate-400">
                  Latest actions & updates
                </p>
              </div>
            </div>
            <button
              onClick={() => navigate("/activity")}
              className="text-xs font-medium text-indigo-500 hover:text-indigo-700 flex items-center gap-1 transition-colors"
            >
              View all
              <ArrowRight className="w-3 h-3" />
            </button>
          </div>

          <div className="p-6">
            {activities.length === 0 ? (
              /* Empty state */
              <div className="flex flex-col items-center justify-center py-12 text-center">
                <div className="w-16 h-16 rounded-2xl bg-slate-100 flex items-center justify-center mb-4">
                  <Zap className="w-7 h-7 text-slate-300" />
                </div>
                <h4 className="text-sm font-semibold text-slate-700 mb-1">
                  No recent activity
                </h4>
                <p className="text-xs text-slate-400 max-w-[240px] mb-5">
                  When you add members, update packages, or make changes, 
                  they'll appear here.
                </p>
                <button
                  onClick={() => navigate("/members")}
                  className="inline-flex items-center gap-2 px-4 py-2 text-[13px] font-medium text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200"
                >
                  <UserPlus className="w-4 h-4" />
                  Add Your First Member
                </button>
              </div>
            ) : (
              /* Activity timeline */
              <div className="space-y-1 max-h-[420px] overflow-y-auto pr-2">
                {activities.slice(0, 8).map((activity, index) => (
                  <div
                    key={activity.id}
                    className="group flex items-start gap-3 p-3 rounded-xl hover:bg-slate-50 transition-colors duration-150"
                    style={{ animationDelay: `${index * 60}ms` }}
                  >
                    {/* Icon */}
                    <div
                      className={`w-9 h-9 rounded-lg flex items-center justify-center border flex-shrink-0 ${getActivityColor(
                        activity.actionType.toString()
                      )}`}
                    >
                      {getActivityIcon(activity.actionType.toString())}
                    </div>

                    {/* Content */}
                    <div className="flex-1 min-w-0">
                      <p className="text-[13px] font-medium text-slate-700 leading-snug">
                        <span className="font-semibold text-slate-900">
                          {activity.performedByName}
                        </span>{" "}
                        {activity.details}
                      </p>
                      <div className="flex items-center gap-1.5 mt-1 text-[11px] text-slate-400">
                        <Clock className="w-3 h-3" />
                        {activity.createdAt}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </Card>

       
      </div>
    </div>
  );
};

export default Dashboard;
