import { memberService } from "./memberService";
import { employeeService } from "./employeeService";

// The backend has no generic upload endpoint - each use case has its own
// specific endpoint (POST /api/members/{id}/profile-picture,
// POST /api/exercises/{id}/media), both requiring an existing entity id.
// Callers that need to attach a picture to a not-yet-created entity must
// create it first, then upload against the returned id.
export const uploadService = {
  uploadMemberProfilePicture: (memberId: number, file: File): Promise<string> =>
    memberService.uploadProfilePicture(memberId, file),

  uploadEmployeeProfilePicture: (employeeId: number, file: File): Promise<string> =>
    employeeService.uploadProfilePicture(employeeId, file),
};
