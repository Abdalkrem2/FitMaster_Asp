import { useState } from "react";
import type { CreateMemberRequest } from "../types/member";
import { memberService } from "../services/memberService";

export const useAddMember = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const addMember = async (request: CreateMemberRequest): Promise<number> => {
    setLoading(true);
    setError("");
    try {
      const newMemberId = await memberService.createMember(request);
      return newMemberId;
    } catch (err) {
      console.log(err, "faild to create member!");
      throw err;
    } finally {
      setLoading(false);
    }
  };
  return { addMember, loading, error };
};
