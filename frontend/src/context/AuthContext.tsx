import React, { createContext, useContext, useState } from "react";

interface AuthUser {
  id: number;
  phone: string;
  name: string;
  roles: string[];
  isActivated?: boolean;
}

interface AuthContextType {
  user: AuthUser | null;
  token: string | null;
  login: (token: string, user: AuthUser) => void;
  logout: () => void;
  isAdmin: () => boolean;
  isEmployee: () => boolean;
  isMember: () => boolean;
}

//Context

const AuthContext = createContext<AuthContextType | null>(null);

//provider

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  // get user from localStorage if exist protect him form refresh)
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem("token"),
  );
  const [user, setUser] = useState<AuthUser | null>(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  const login = (newToken: string, newUser: AuthUser) => {
    // stor in localStorage to save it after refresh
    localStorage.setItem("token", newToken);
    localStorage.setItem("user", JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setToken(null);
    setUser(null);
  };

  const isAdmin = () => user?.roles?.includes("ADMIN") ?? false;

  const isEmployee = () => user?.roles?.includes("EMPLOYEE") ?? false;

  const isMember = () => user?.roles?.includes("MEMBER") ?? false;

  return (
    <AuthContext.Provider
      value={{ user, token, login, logout, isAdmin, isEmployee, isMember }}
    >
      {children}
    </AuthContext.Provider>
  );
};

//Hook
export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside AuthProvider");
  return ctx;
};
