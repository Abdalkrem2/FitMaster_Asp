import { createBrowserRouter, Navigate, Outlet } from "react-router-dom";
import { DashboardLayout } from "../layouts/DashboardLayout";
import { useAuth } from "../context/AuthContext";

// Pages
import Login from "../pages/Login";
import Dashboard from "../pages/Dashboard";
import Members from "../pages/Members";

import Packages from "../pages/Packages";
import Revenue from "../pages/Revenue";
import ActivityLog from "../pages/ActivityLog";
import Employees from "../pages/Employees";
import EmployeeDashboard from "@/pages/EmployeeDashboard";
import MemberDetails from "@/pages/MemberDetails";
import MemberProfile from "../pages/MemberProfile";
import { MemberLayout } from "../layouts/MemberLayout";
import MemberDashboard from "../pages/MemberDashboard";
import MemberOnboarding from "@/pages/MemberOnboarding";
import MemberWorkoutPlan from "../pages/MemberWorkoutPlan";
import MemberNutritionPlan from "../pages/MemberNutritionPlan";
import MemberPlanHistory from "../pages/MemberPlanHistory";
import MemberSettings from "../pages/MemberSettings";

//Guard: ADMIN only
// if not admin return to tha main page
const AdminOnly = () => {
  const { isAdmin } = useAuth();
  return isAdmin() ? <Outlet /> : <Navigate to="/" replace />;
};

//Guard: Render the dashboard based on user role
const DashboardRouter = () => {
  const { isAdmin } = useAuth();
  return isAdmin() ? <Dashboard /> : <EmployeeDashboard />;
};

//Router
export const router = createBrowserRouter([
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/member-onboarding", // ← ADD THIS
    element: <MemberOnboarding />,
  },
  {
    path: "/member-dashboard",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberDashboard /> }],
  },
  {
    path: "/member-profile",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberProfile /> }],
  },
  {
    path: "/member-workout-plan",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberWorkoutPlan /> }],
  },
  {
    path: "/member-nutrition-plan",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberNutritionPlan /> }],
  },
  {
    path: "/member-plan-history",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberPlanHistory /> }],
  },
  {
    path: "/member-settings",
    element: <MemberLayout />,
    children: [{ index: true, element: <MemberSettings /> }],
  },
  {
    path: "/",
    element: <DashboardLayout />,
    children: [
      // pages for employee & admin
      { index: true, element: <DashboardRouter /> },
      { path: "e-dashboard", element: <EmployeeDashboard /> }, //employee Dashbord
      { path: "members", element: <Members /> },
      { path: "members/:id", element: <MemberDetails /> },

      // pages for admin only
      {
        element: <AdminOnly />,
        children: [
          { path: "packages", element: <Packages /> },
          { path: "revenue", element: <Revenue /> },
          { path: "activity", element: <ActivityLog /> },
          { path: "employees", element: <Employees /> },
        ],
      },
    ],
  },
  {
    path: "*",
    element: <Navigate to="/" replace />,
  },
]);
