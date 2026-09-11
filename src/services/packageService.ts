import type { Package, CreatePackageRequest, UpdatePackageRequest } from "../types/package";
import { api } from "./api";

export const packageService = {
  getAllPackages: async (includeInactive = false): Promise<Package[]> => {
    const res = await api.get("/packages", { params: { includeInactive } });
    return res.data ?? [];
  },

  createPackage: async (pkg: CreatePackageRequest): Promise<number> => {
    const res = await api.post("/packages", pkg);
    return res.data;
  },

  // No dedicated status-only endpoint on the backend - PUT requires the full
  // object, so the caller must pass the package's current fields alongside
  // the new status.
  updatePackageStatus: async (id: string, pkg: UpdatePackageRequest, status: string): Promise<void> => {
    await api.put(`/packages/${id}`, { ...pkg, id: Number(id), status });
  },

  updatePackage: async (id: string, pkg: UpdatePackageRequest): Promise<void> => {
    await api.put(`/packages/${id}`, { ...pkg, id: Number(id) });
  },

  deletePackage: async (id: string): Promise<void> => {
    await api.delete(`/packages/${id}`);
  },
};
