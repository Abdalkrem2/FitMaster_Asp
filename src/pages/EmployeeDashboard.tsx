import React, { useEffect, useState } from "react";
import { Users, AlertCircle } from "lucide-react";
import {
  dashboardService,
  type DashboardStats,
} from "../services/dashboardService";
import { Card } from "../components/ui/Card";

const StatCard: React.FC<{
  title: string;
  value: string | number;
  icon: React.ReactNode;
  colorClass: string;
}> = ({ title, value, icon, colorClass }) => (
  <Card className="flex items-center p-6 space-x-4 hover:-translate-y-1 transition-transform">
    <div className={`p-4 rounded-full ${colorClass}`}>{icon}</div>
    <div>
      <p className="text-sm font-medium text-gray-500">{title}</p>
      <h4 className="text-2xl font-bold text-gray-900">{value}</h4>
    </div>
  </Card>
);

const EmployeeDashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchDashboardData = async () => {
      try {
        const statsData = await dashboardService.getStats();
        setStats(statsData);
      } catch (error) {
        console.error("Failed to load dashboard data");
      } finally {
        setLoading(false);
      }
    };
    fetchDashboardData();
  }, []);

  if (loading || !stats) {
    return (
      <div className="flex h-64 items-center justify-center">
        Loading dashboard...
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-end">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">
            Dashboard Overview
          </h2>
          <p className="text-sm text-gray-500 mt-1">Welcome back, Employee</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <StatCard
          title="Active Members"
          value={stats.activeMembers}
          icon={<Users className="w-6 h-6 text-blue-600" />}
          colorClass="bg-blue-100"
        />

        <StatCard
          title="Expiring Soon"
          value={stats.expiringSoon}
          icon={<AlertCircle className="w-6 h-6 text-amber-600" />}
          colorClass="bg-amber-100"
        />
      </div>


    </div>
  );
};

export default EmployeeDashboard;
