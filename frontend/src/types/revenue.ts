export interface RevenueStats {
  today: number;
  thisMonth: number;
  thisYear: number;
  debt: number;
}

export interface MonthlyRevenue {
  yearTotal: number;
  months: Record<number, number>;
}

export interface RevenueRow {
  id: number;
  addedByName: string;
  memberName: string;
  amount: number;
  pkg: string;
  debt?: number;
  description?: string;
  createdAt: string;
}

export interface RevenueByPeriod {
  periodTotal: number;
  periodDebt: number;
  revenues: RevenueRow[];
}
