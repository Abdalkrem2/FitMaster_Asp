import { api } from "./api";

export const authService = {
  login: async (phone: string, password: string) => {
    try {
      const res = await api.post("/auth/login", { phone, password });
      return {
        token: res.data.token,
        user: {
          id: res.data.userId,
          phone,
          name: res.data.fullName,
          roles: res.data.roles,
          isActivated: true,
        },
      };
    } catch (err) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      throw err;
    }
  },
};
