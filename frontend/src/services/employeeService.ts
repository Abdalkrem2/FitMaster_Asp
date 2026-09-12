import { api } from "./api";
import type {
  Employee,
  CreateEmployeeRequest,
  UpdateEmployeeRequest,
} from "../types/employee";

export interface EmployeePageResponse {
  content: Employee[];
  pageNumber: number;
  pageSize: number;
  totalElements: number;
  totalPages: number;
  lastPage: boolean;
}

export const employeeService = {
  getAllEmployees: async (
    page = 0,
    size = 10,
  ): Promise<EmployeePageResponse> => {
    const res = await api.get("/users", { params: { page, size } });
    const data = res.data;

    return {
      content: (data.content ?? []).map((e: any) => ({
        id: e.id,
        fullName: e.fullName,
        phone: e.phone,
        gender: e.gender,
        isActivated: e.isActivated,
        profilePicture: e.profilePicture,
        roles: e.roles ?? [],
      })),
      pageNumber: data.number ?? 0,
      pageSize: data.size ?? size,
      totalElements: data.totalElements ?? 0,
      totalPages: data.totalPages ?? 1,
      lastPage: data.last ?? true,
    };
  },

  createEmployee: async (
    employee: CreateEmployeeRequest,
  ): Promise<number> => {
    const res = await api.post("/users", employee);
    return res.data;
  },

  updateEmployee: async (
    id: number,
    employee: UpdateEmployeeRequest,
  ): Promise<void> => {
    await api.patch(`/users/${id}`, { ...employee, userId: id });
  },

  deleteEmployee: async (id: number): Promise<void> => {
    await api.delete(`/users/${id}`);
  },

  uploadProfilePicture: async (id: number, file: File): Promise<string> => {
    const formData = new FormData();
    formData.append("file", file);
    // Same generic upload endpoint as members - it updates User.ProfilePicture
    // regardless of role, so it works for staff accounts too.
    const res = await api.post(`/members/${id}/profile-picture`, formData);
    return res.data.url;
  },
};
