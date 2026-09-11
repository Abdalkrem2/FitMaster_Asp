import React, { useEffect, useState, useCallback } from "react";
import { createPortal } from "react-dom";
import { Search, Plus, Pencil, Trash2, ShieldCheck, User } from "lucide-react";
import { employeeService } from "../services/employeeService";
import { Input } from "../components/ui/Input";
import { Table, type Column } from "../components/ui/Table";
import { Card } from "../components/ui/Card";
import type { Employee } from "../types/employee";
import AddEmployeeModal from "../components/AddEmployeeModal";
import EditEmployeeModal from "../components/EditEmplyoeeModal";

const Employees: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [loading, setLoading] = useState(true);
  const [addOpen, setAddOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<Employee | null>(null);
  const [deleteConfirmId, setDeleteConfirmId] = useState<number | null>(null);

  const fetchEmployees = useCallback(async () => {
    setLoading(true);
    try {
      const data = await employeeService.getAllEmployees();
      setEmployees(data.content);
    } catch (err) {
      console.error("Failed to fetch employees");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees]);

  const handleDelete = async (id: number) => {
    try {
      await employeeService.deleteEmployee(id);
      setDeleteConfirmId(null);
      fetchEmployees();
    } catch (err) {
      console.error("Failed to delete employee");
    }
  };

  const filteredEmployees = employees.filter(
    (e) =>
      e.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      e.phone.includes(searchTerm),
  );

  const columns: Column<Employee>[] = [
    { key: "id", header: "ID" },
    {
      key: "fullName",
      header: "Employee Name",
      render: (row) => (
        <div className="flex items-center gap-3">
          {row.profilePicture ? (
            <img
              src={row.profilePicture}
              className="w-8 h-8 rounded-full object-cover border border-slate-200 shadow-sm"
              alt={row.fullName}
            />
          ) : (
            <div className="w-8 h-8 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center text-slate-400">
              <User size={14} />
            </div>
          )}
          <span className="font-semibold text-slate-800">{row.fullName}</span>
        </div>
      ),
    },
    { key: "phone", header: "Phone" },
    { key: "gender", header: "Gender" },
    {
      key: "roles",
      header: "Role",
      render: (row) => {
        const isAdmin = row.roles?.some((r) => r === "ADMIN");
        return (
          <span
            className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md text-[11px] font-bold uppercase tracking-wider ${
              isAdmin
                ? "bg-violet-50 text-violet-700 border border-violet-200/50"
                : "bg-indigo-50 text-indigo-700 border border-indigo-200/50"
            }`}
          >
            <ShieldCheck size={12} />
            {isAdmin ? "Admin" : "Employee"}
          </span>
        );
      },
    },
    {
      key: "isActivated",
      header: "Status",
      render: (row) => (
        <span
          className={`px-2.5 py-1 rounded-md text-[11px] font-bold uppercase tracking-wider ${
            row.isActivated
              ? "bg-emerald-50 text-emerald-600 border border-emerald-200/50"
              : "bg-rose-50 text-rose-600 border border-rose-200/50"
          }`}
        >
          {row.isActivated ? "Active" : "Inactive"}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      render: (row) => (
        <div className="flex items-center gap-2">
          <button
            onClick={() => setEditTarget(row)}
            className="inline-flex items-center justify-center gap-1.5 px-3 py-1.5 text-xs font-semibold bg-white text-indigo-600 border border-indigo-200 shadow-sm rounded-lg hover:bg-indigo-50 hover:border-indigo-300 transition-all focus:outline-none"
          >
            <Pencil className="w-3 h-3" /> Edit
          </button>
          <button
            onClick={() => setDeleteConfirmId(row.id)}
            className="inline-flex items-center justify-center gap-1.5 px-3 py-1.5 text-xs font-semibold bg-white text-rose-600 border border-rose-200 shadow-sm rounded-lg hover:bg-rose-50 hover:border-rose-300 transition-all focus:outline-none"
          >
            <Trash2 className="w-3 h-3" /> Delete
          </button>
        </div>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight">Employees</h2>
          <p className="text-sm text-slate-500 mt-1">
            Manage your staff, permissions, and profiles.
          </p>
        </div>
        <button 
          onClick={() => setAddOpen(true)}
          className="inline-flex items-center gap-2 px-4 py-2.5 text-[13px] font-medium text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200"
        >
          <Plus className="w-4 h-4" /> Add Employee
        </button>
      </div>

      {/* Table Card */}
      <Card padding="md" className="space-y-4 border border-slate-200">
        <div className="flex items-center">
          <div className="relative flex-1 max-w-md group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
              <Search className="h-[18px] w-[18px] text-slate-400 group-focus-within:text-indigo-500 transition-colors" />
            </div>
            <input
              type="text"
              placeholder="Search employees by name or phone..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2.5 text-sm bg-slate-50 border border-slate-200 rounded-xl focus:bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-400 transition-all placeholder:text-slate-400"
            />
          </div>
        </div>

        {loading ? (
           <div className="flex flex-col items-center justify-center py-12 gap-3">
             <div className="w-8 h-8 border-2 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
             <p className="text-sm text-slate-400 font-medium">Loading employees...</p>
           </div>
        ) : (
          <Table
            data={filteredEmployees}
            columns={columns}
            keyExtractor={(row) => row.id}
          />
        )}
      </Card>

      {/* Add Modal */}
      <AddEmployeeModal
        open={addOpen}
        onClose={() => setAddOpen(false)}
        onCreated={fetchEmployees}
      />

      {/* Edit Modal */}
      <EditEmployeeModal
        open={!!editTarget}
        onClose={() => setEditTarget(null)}
        employee={editTarget}
        onUpdated={() => {
          setEditTarget(null);
          fetchEmployees();
        }}
      />

      {/* Delete Confirm Dialog */}
      {deleteConfirmId !== null && createPortal(
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-[100] animate-in fade-in duration-200">
          <div className="bg-white rounded-2xl p-6 w-[380px] shadow-elevated relative animate-fade-in-up">
            <div className="w-12 h-12 bg-rose-50 border-8 border-rose-50/50 rounded-full flex items-center justify-center mx-auto mb-4 text-rose-500">
              <Trash2 className="w-6 h-6" />
            </div>
            <h3 className="text-lg font-bold text-slate-900 text-center mb-1">
              Delete Employee
            </h3>
            <p className="text-sm text-slate-500 text-center mb-6">
              Are you sure you want to delete this employee? This action cannot
              be undone.
            </p>
            <div className="flex gap-3">
               <button
                onClick={() => setDeleteConfirmId(null)}
                className="flex-1 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-xl py-2.5 hover:bg-slate-50 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={() => handleDelete(deleteConfirmId)}
                className="flex-1 text-sm font-semibold text-white bg-rose-600 rounded-xl py-2.5 shadow-sm hover:shadow-md hover:bg-rose-700 transition-all font-medium"
              >
                Delete
              </button>
            </div>
          </div>
        </div>,
        document.body
      )}
    </div>
  );
};

export default Employees;
