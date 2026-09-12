import { api } from "./api";

export interface DashboardStats {
  activeMembers: number;
  monthlyRevenue: number;
  expiringSoon: number;
}

export interface ActivityItem {
  actionType: string;
  entityType: string;
  entityId: number;
  performedByName: string;
  createdAt: string;
  details: string;
}

export const dashboardService = {
  getStats: async (): Promise<DashboardStats> => {
    const res = await api.get("/members/stats");
    const revenueResponse = await api.get("/revenue/stats");
    return {
      activeMembers: res.data.activeMembers ?? 0,
      monthlyRevenue: revenueResponse.data.thisMonth ?? 0,
      expiringSoon: res.data.expiringSoon ?? 0,
    };
  },

  // /revenue/* is Admin-only on the backend, so this must not call it -
  // used by EmployeeDashboard, which never shows revenue anyway.
  getEmployeeStats: async (): Promise<Pick<DashboardStats, "activeMembers" | "expiringSoon">> => {
    const res = await api.get("/members/stats");
    return {
      activeMembers: res.data.activeMembers ?? 0,
      expiringSoon: res.data.expiringSoon ?? 0,
    };
  },

  getRecentActivity: async (): Promise<ActivityItem[]> => {
    const res = await api.get("/logs", { params: { page: 0, size: 10 } });
    return res.data?.content ?? [];
  },
};
