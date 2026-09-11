import {
  useState,
  useCallback,
  createContext,
  useContext,
  type ReactNode,
} from "react";
import { useNotifications } from "@/hooks/useNotifications";
import { notificationService } from "@/services/notificationService";
import type { Notification, NotificationResponse } from "@/types/notification";
interface NotificationContextType {
  notifications: Notification[];
  unreadCount: number;
  error: string | null;
  markAsRead: (id: number) => void;
  markAllAsRead: () => void;
  clearNotification: (id: number) => void;
  clearAll: () => void;
}

const NotificationContext = createContext<NotificationContextType | undefined>(
  undefined,
);

export function NotificationProvider({ children }: { children: ReactNode }) {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [unreadCount, setUnreadCount] = useState(0); //
  const [error, setError] = useState<string | null>(null);

  const handleFetch = useCallback((data: NotificationResponse) => {
    setNotifications(data.content);
    setUnreadCount(data.unreadCount);
    setError(null);
  }, []);

  useNotifications(handleFetch, setError);

  const markAsRead = useCallback(async (id: number) => {
    try {
      await notificationService.markAsRead(id);
      setNotifications((prev) =>
        prev.map((n) => (n.notificationId === id ? { ...n, read: true } : n)),
      );
      setUnreadCount((prev) => Math.max(0, prev - 1));
    } catch (e) {
      console.error(e);
    }
  }, []);

  const markAllAsRead = useCallback(async () => {
    try {
      await notificationService.markAllAsRead();
      setNotifications((prev) => prev.map((n) => ({ ...n, read: true })));
      setUnreadCount(0);
    } catch (e) {
      console.error(e);
    }
  }, []);

  const clearNotification = useCallback(async (id: number) => {
    try {
      await notificationService.deleteNotification(id);
      setNotifications((prev) => prev.filter((n) => n.notificationId !== id));
    } catch (e) {
      console.error(e);
    }
  }, []);

  const clearAll = useCallback(async () => {
    try {
      await notificationService.clearAll();
      setNotifications([]);
      setUnreadCount(0);
    } catch (e) {
      console.error(e);
    }
  }, []);

  return (
    <NotificationContext.Provider
      value={{
        notifications,
        unreadCount,
        error,
        markAsRead,
        markAllAsRead,
        clearNotification,
        clearAll,
      }}
    >
      {children}
    </NotificationContext.Provider>
  );
}

export function useNotificationContext() {
  const context = useContext(NotificationContext);
  if (!context) {
    throw new Error(
      "useNotificationContext must be used within a NotificationProvider",
    );
  }
  return context;
}
