export interface ActivityLogItem {
  id: number;
  actionType: string;
  entityType?: string;
  entityId: number;
  performedByName: string;
  createdAt: string;
  details?: string;
}

export interface ActivityLogPageResponse {
  content: ActivityLogItem[];
  totalElements: number;
  totalPages: number;
  number: number;
  size: number;
  first: boolean;
  last: boolean;
}
