import { api } from "./api";
import type { ActivityLogPageResponse } from "../types/activityLog";

export const activityLogService = {
  getLogs: async (
    performedBy: number | null,
    entityType: string | null,
    page: number,
    size: number,
  ): Promise<ActivityLogPageResponse> => {
    const params: any = { page, size };
    if (performedBy !== null && performedBy !== undefined)
      params.performedBy = performedBy;
    if (entityType !== null && entityType !== undefined && entityType !== "")
      params.entityType = entityType;

    const res = await api.get("/logs", { params });
    return res.data;
  },
};
