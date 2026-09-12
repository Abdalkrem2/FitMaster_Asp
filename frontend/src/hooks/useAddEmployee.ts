import { useState } from "react";
import type { CreateEmployeeRequest } from "../types/employee";
import { employeeService } from "../services/employeeService.ts";

export const useAddEmployee = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const addEmployee = async (
    request: CreateEmployeeRequest,
  ): Promise<number> => {
    setLoading(true);
    setError("");
    try {
      const newEmployeeId = await employeeService.createEmployee(request);
      return newEmployeeId;
    } catch (err) {
      console.error(err, "failed to create employee!");
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { addEmployee, loading, error };
};
