import { notificationService } from "@/services/notificationService";
import type { NotificationResponse } from "@/types/notification";
import { useEffect, useCallback } from "react";

const POLLING_INTERVAL = 10_000;

export function useNotifications(
  onFetch: (data: NotificationResponse) => void,
  onError: (message: string) => void,
) {
  const fetchNotifications = useCallback(async () => {
    try {
      const data = await notificationService.getMyNotifications(0, 20);
      onFetch(data);
      onError("");
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      console.error("Failed to fetch notifications:", error);
      onError(message);
    }
  }, [onFetch, onError]);

  useEffect(() => {
    fetchNotifications();
    const interval = setInterval(fetchNotifications, POLLING_INTERVAL);
    return () => clearInterval(interval);
  }, [fetchNotifications]);
}
