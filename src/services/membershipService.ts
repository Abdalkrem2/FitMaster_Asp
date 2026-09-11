import type { Membership, MembershipRequest } from "@/types/membership";
import { api } from "./api";

export const membershipService = {
  addMembership: async (memberId: string | number, membership: Omit<MembershipRequest, "memberId">): Promise<number> => {
    const res = await api.post("/memberships", { ...membership, memberId: Number(memberId) });
    return res.data;
  },

  getById: async (id: string | number): Promise<Membership> => {
    const res = await api.get(`/memberships/${id}`);
    return res.data;
  },

  getByMember: async (memberId: string | number): Promise<Membership[]> => {
    const res = await api.get(`/memberships/by-member/${memberId}`);
    return res.data;
  },

  renew: async (id: string | number, startDate?: string): Promise<void> => {
    await api.post(`/memberships/${id}/renew`, startDate ?? null);
  },
};
