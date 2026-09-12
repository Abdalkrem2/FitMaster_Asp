  import { activityLogService } from "@/services/activityLogService";


  export const useGetLogs = async (performedBy: string | null, entityType: string | null, page: number, size: number) => {
    try {
      const performedById = performedBy ? parseInt(performedBy, 10) : null;
      const type = entityType ? entityType : null;
      const safePerformedById = isNaN(performedById as any) ? null : performedById;

      const data = await activityLogService.getLogs(safePerformedById, type, page, size);
      return data;
    } catch (err) {
      console.error("Failed to load activity log", err);
    }
  };