export type PackageStatus = "ACTIVE" | "INACTIVE";

export interface Package {
  id: string;
  name: string;
  price: number;
  durationInDays: number;
  status: PackageStatus | string;
  description: string;
}

export interface CreatePackageRequest {
  name: string;
  price: number;
  durationInDays: number;
  description: string;
  status: PackageStatus | string;
}

// PUT /api/packages/{id} requires the full object, not a partial patch.
export interface UpdatePackageRequest {
  id?: number;
  name: string;
  price: number;
  durationInDays: number;
  description: string;
  status: PackageStatus | string;
}
