export type MembershipStatus = "ACTIVE" | "EXPIRED" | "FROZEN" | "CANCELED";

// Matches CreateMembershipCommand (POST /api/memberships)
// Debt is never sent directly - the backend always derives it from
// price - amountPaid, so a client can't submit an inconsistent value.
export interface MembershipRequest {
  memberId: number;
  packageId: number;
  startDate: string;
  price?: number;
  amountPaid?: number;
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
