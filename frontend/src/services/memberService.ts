import { api } from "./api";
import type {
  Member,
  MemberDetails,
  CreateMemberRequest,
  UpdateMemberRequest,
  MemberProfile,
  UpdateMemberProfileRequest,
} from "../types/member";
import type { PageResponse } from "../types/pagination";

// Matches PagedResult<T> from the backend (see Common/Models/PagedResult.cs)
interface BackendPage<T> {
  content: T[];
  totalElements: number;
  totalPages: number;
  number: number;
  size: number;
  first: boolean;
  last: boolean;
}

function toPageResponse<T>(page: BackendPage<T>): PageResponse<T> {
  return {
    content: page.content,
    totalElements: page.totalElements,
    totalPages: page.totalPages,
    number: page.number,
    size: page.size,
    first: page.first,
    last: page.last,
  };
}

export const memberService = {
  async getAllMembers(
    page: number,
    size: number = 20,
    search?: string,
  ): Promise<PageResponse<Member>> {
    const res = await api.get<BackendPage<Member>>("/members", {
      params: { page, size, search },
    });
    return toPageResponse(res.data);
  },

  getMemberById: async (id: string | number): Promise<MemberDetails | undefined> => {
    const res = await api.get(`/members/${id}`);
    return res.data;
  },

  createMember: async (member: CreateMemberRequest): Promise<number> => {
    const res = await api.post("/members", member);
    return res.data;
  },

  // Phone/name only - see UpdateMemberProfileRequest for fitness/health fields.
  updateMember: async (
    id: string | number,
    member: UpdateMemberRequest,
  ): Promise<void> => {
    await api.patch(`/members/${id}`, { ...member, memberId: Number(id) });
  },

  updateMemberProfile: async (
    id: string | number,
    profile: UpdateMemberProfileRequest,
  ): Promise<void> => {
    await api.put(`/members/${id}`, { ...profile, memberId: Number(id) });
  },

  deleteMember: async (id: string | number): Promise<void> => {
    await api.delete(`/members/${id}`);
  },

  uploadProfilePicture: async (id: string | number, file: File): Promise<string> => {
    const formData = new FormData();
    formData.append("file", file);
    const res = await api.post(`/members/${id}/profile-picture`, formData);
    return res.data.url;
  },

  getMyDetails: async (): Promise<MemberDetails> => {
    const res = await api.get("/members/me");
    return res.data;
  },

  getMyProfile: async (): Promise<MemberProfile | undefined> => {
    const res = await api.get("/members/me/profile");
    return res.data;
  },

  updateMyProfile: async (
    profile: UpdateMemberProfileRequest,
  ): Promise<void> => {
    await api.patch("/members/me/profile", profile);
  },

  changePassword: async (
    currentPassword: string,
    newPassword: string,
  ): Promise<void> => {
    await api.patch("/members/me/password", { currentPassword, newPassword });
  },

  getStats: async (): Promise<{ activeMembers: number; expiringSoon: number }> => {
    const res = await api.get("/members/stats");
    return res.data;
  },
};
