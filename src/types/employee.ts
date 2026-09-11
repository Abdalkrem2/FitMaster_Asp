export type AppRole = "EMPLOYEE" | "ADMIN";

// Matches StaffUserDto (GET /api/users)
export interface Employee {
  id: number;
  fullName: string;
  phone: string;
  gender?: string;
  isActivated: boolean;
  profilePicture?: string;
  roles: string[];
}

// Matches CreateStaffUserCommand (POST /api/users)
export interface CreateEmployeeRequest {
  fullName: string;
  phone: string;
  gender?: string;
  password: string;
  role: AppRole;
}

// Matches UpdateStaffUserCommand (PATCH /api/users/{id})
export interface UpdateEmployeeRequest {
  userId?: number;
  fullName: string;
  phone: string;
  gender?: string;
  password?: string;
  role?: AppRole;
  isActivated?: boolean;
}
