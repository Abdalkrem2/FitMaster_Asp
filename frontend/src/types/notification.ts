export type NotificationType = "ACTIVITY_LOG";

// Matches NotificationDto
export interface Notification {
  notificationId: number;
  type: NotificationType;
  message: string;
  details?: string;
  read: boolean;
  createdAt: string;
}

// Matches NotificationsPageDto
export interface NotificationResponse {
  content: Notification[];
  pageNumber: number;
  pageSize: number;
  totalElements: number;
  totalPages: number;
  lastPage: boolean;
  unreadCount: number;
}
