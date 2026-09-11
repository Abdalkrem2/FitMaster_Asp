export type MembershipStatus = "ACTIVE" | "EXPIRED" | "FROZEN" | "CANCELED";

// Matches CreateMembershipCommand (POST /api/memberships)
export interface MembershipRequest {
  memberId: number;
  packageId: number;
  startDate: string;
  price?: number;
  debt?: number;
  description?: string;
}

// Matches MembershipDto (GET /api/memberships/{id}, GET /api/memberships/by-member/{memberId})
export interface Membership {
  id: number;
  memberId: number;
  memberFullName: string;
  packageId: number;
  packageName: string;
  status: MembershipStatus | string;
  startDate: string;
  endDate: string;
  price: number;
  debt?: number;
  description?: string;
}

export interface MembershipHistory {
  id: number;
  price: number;
  debt?: number;
  packageName: string;
  description?: string;
  timestamp: string;
}
