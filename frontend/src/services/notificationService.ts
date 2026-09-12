import { api } from "./api";
import type { NotificationResponse } from "@/types/notification";

export const notificationService = {
  getMyNotifications: (page = 0, size = 20): Promise<NotificationResponse> =>
    api
      .get("/notifications", { params: { page, size } })
      .then((res) => res.data),

  markAsRead: (id: number): Promise<void> =>
    api.put(`/notifications/${id}/read`),

  markAllAsRead: (): Promise<void> => api.put("/notifications/read-all"),

  deleteNotification: (id: number): Promise<void> =>
    api.delete(`/notifications/${id}`),

  clearAll: (): Promise<void> => api.delete("/notifications"),
};
