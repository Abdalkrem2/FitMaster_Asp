import type { MonthlyRevenue, RevenueByPeriod, RevenueStats } from "../types/revenue";
import { api } from "./api";

export const revenueService = {
  getStats: async (): Promise<RevenueStats> => {
    const response = await api.get("/revenue/stats");
    return response.data;
  },

  monthlyRevenue: async (year: number): Promise<MonthlyRevenue> => {
    const response = await api.get(`/revenue/monthly`, { params: { year } });
    return response.data;
  },

  revenueByPeriod: async (start: string, end: string): Promise<RevenueByPeriod> => {
    const response = await api.get("/revenue/period", { params: { start, end } });
    return response.data;
  },
};
