import React, { useState, useEffect, useRef } from "react";
import { createPortal } from "react-dom";
import { employeeService } from "@/services/employeeService";
import CameraCaptureModal from "./CameraCaptureModal";
import { Camera, Upload, X, AlertCircle } from "lucide-react";
import type { Employee, AppRole } from "../types/employee";
import { Input } from "./ui/Input";
import { Switch } from "@/components/ui/switch";

interface Props {
  open: boolean;
  onClose: () => void;
  employee: Employee | null;
  onUpdated: () => void;
}

export default function EditEmployeeModal({
  open,
  onClose,
  employee,
  onUpdated,
}: Props) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const cameraInputRef = useRef<HTMLInputElement>(null);
  const [showCamera, setShowCamera] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(null);
  const [file, setFile] = useState<File | null>(null);

  const [formData, setFormData] = useState({
    fullName: "",
    phone: "",
    password: "",
    role: "EMPLOYEE" as AppRole,
    isActivated: true,
  });

  useEffect(() => {
    if (employee) {
      const currentRole = employee.roles?.find(
        (r) => r === "ADMIN" || r === "EMPLOYEE",
      );
      setFormData({
        fullName: employee.fullName || "",
        phone: employee.phone || "",
        password: "",
        role: (currentRole as AppRole) ?? "EMPLOYEE",
        isActivated: employee.isActivated ?? true,
      });
      setPreview(employee.profilePicture || null);
      setFile(null);
      setError(null);
    }
  }, [employee, open]);

  if (!open || !employee) return null;

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>,
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleToggleActive = (checked: boolean) => {
    setFormData((prev) => ({ ...prev, isActivated: checked }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const selectedFile = e.target.files[0];
      setFile(selectedFile);
      setPreview(URL.createObjectURL(selectedFile));
    }
  };

  const validate = () => {
    if (!formData.fullName) return "Full name is required";
    if (!formData.phone) return "Phone is required";
    if (formData.password && formData.password.length < 8)
      return "Password must be at least 8 characters";
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      if (file) {
        await employeeService.uploadProfilePicture(employee.id, file);
      }

      await employeeService.updateEmployee(employee.id, {
        fullName: formData.fullName,
        phone: formData.phone,
        ...(formData.password ? { password: formData.password } : {}),
        role: formData.role,
        isActivated: formData.isActivated,
      });

      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      setError("Failed to update employee");
    } finally {
      setLoading(false);
    }
  };

  return createPortal(
    <div 
      onClick={onClose} 
      className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-[100] p-4 transition-opacity"
    >
      <div 
        onClick={(e) => e.stopPropagation()} 
        className="bg-white rounded-2xl w-full max-w-md shadow-elevated relative animate-fade-in-up max-h-[95vh] overflow-y-auto"
      >
        <div className="sticky top-0 bg-white/90 backdrop-blur-sm z-20 flex items-center justify-between px-6 py-4 border-b border-slate-100">
          <h2 className="text-lg font-bold tracking-tight text-slate-900">
            Edit Employee
          </h2>
          <button
            onClick={onClose}
            className="text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition-colors p-1.5 rounded-lg focus:outline-none"
          >
            <X size={18} />
          </button>
        </div>

        <div className="p-6">
          <div className="flex flex-col items-center justify-center mb-6">
            <div className="relative group mb-3">
              <div 
                className={`w-20 h-20 rounded-full border-2 border-dashed flex items-center justify-center overflow-hidden transition-all duration-200 cursor-pointer
                  ${preview ? 'border-indigo-200' : 'border-slate-200 hover:border-indigo-400 bg-slate-50'}`}
                onClick={() => fileInputRef.current?.click()}
              >
                {preview ? (
                  <img src={preview} className="w-full h-full object-cover" alt="Preview" />
                ) : (
                  <Upload className="w-6 h-6 text-slate-300 group-hover:text-indigo-400 transition-colors" />
                )}
              </div>
              
              <button
                type="button"
                onClick={(e) => { 
                  e.stopPropagation(); 
                  const isMobile = /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
                  if (isMobile) {
                    cameraInputRef.current?.click(); 
                  } else {
                    setShowCamera(true);
                  }
                }}
                className="absolute bottom-0 right-0 p-1.5 bg-white border border-slate-200 text-slate-500 rounded-full shadow-sm hover:text-indigo-600 hover:border-indigo-200 transition-colors"
                title="Use Camera"
              >
                <Camera size={14} />
              </button>
            </div>
            
            <p className="text-[11px] font-medium text-slate-400">Click to update photo or use camera</p>
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*"
              className="hidden"
              onChange={handleFileChange}
            />
            <input
              ref={cameraInputRef}
              type="file"
              accept="image/*"
              capture="environment"
              className="hidden"
              onChange={handleFileChange}
            />
          </div>

          <div className="space-y-4">
            <Input
              name="fullName"
              label="Full Name"
              value={formData.fullName}
              onChange={handleChange}
              autoComplete="off"
            />

            <Input
              name="phone"
              label="Phone"
              value={formData.phone}
              onChange={handleChange}
              autoComplete="off"
            />

            <Input
              name="password"
              type="password"
              label="New Password"
              placeholder="(Leave blank to keep current password)"
              value={formData.password}
              onChange={handleChange}
              autoComplete="new-password"
            />

            <div>
               <label className="block text-sm font-medium text-slate-700 mb-1">
                 Role
               </label>
               <select
                 name="role"
                 value={formData.role}
                 className="w-full px-3 py-2 text-sm border border-slate-200 bg-slate-50 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-400 focus:bg-white transition-all"
                 onChange={handleChange}
               >
                 <option value="EMPLOYEE">Employee</option>
                 <option value="ADMIN">Admin</option>
               </select>
            </div>

            {/* Active Toggle */}
            <div className="flex items-center justify-between px-3 py-2.5 border border-slate-200 bg-slate-50 rounded-lg mt-2">
              <div>
                <span className="text-sm font-medium text-slate-800 tracking-tight block">
                  Account Status
                </span>
                <span className="text-[11px] text-slate-500 font-medium">Control system access</span>
              </div>
              <div className="flex items-center gap-3">
                 <span className={`text-xs font-bold uppercase tracking-wider ${formData.isActivated ? 'text-emerald-600' : 'text-slate-400'}`}>
                    {formData.isActivated ? "Active" : "Inactive"}
                 </span>
                 <Switch 
                   checked={formData.isActivated} 
                   onCheckedChange={handleToggleActive} 
                   className="data-[state=checked]:bg-emerald-500"
                 />
              </div>
            </div>
          </div>

          {error && (
            <div className="mt-4 p-3 rounded-lg bg-rose-50 border border-rose-100 flex items-start gap-2">
              <AlertCircle className="w-4 h-4 text-rose-500 mt-0.5 flex-shrink-0" />
              <p className="text-sm font-medium text-rose-600">{error}</p>
            </div>
          )}

          <div className="flex gap-3 mt-8">
            <button
              onClick={onClose}
              className="flex-1 py-2.5 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-xl hover:bg-slate-50 transition-all duration-200"
            >
              Cancel
            </button>
            <button
              onClick={handleSubmit}
              disabled={loading}
              className="flex-1 py-2.5 text-sm font-semibold text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200 disabled:opacity-70 flex items-center justify-center"
            >
              {loading ? (
                <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              ) : "Save Changes"}
            </button>
          </div>
        </div>
      </div>

      <CameraCaptureModal
        open={showCamera}
        onClose={() => setShowCamera(false)}
        onCapture={(capturedFile) => {
          setFile(capturedFile);
          setPreview(URL.createObjectURL(capturedFile));
        }}
      />
    </div>,
    document.body
  );
}
