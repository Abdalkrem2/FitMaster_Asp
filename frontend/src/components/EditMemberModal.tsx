import React, { useState, useEffect, useRef } from "react";
import { createPortal } from "react-dom";
import { memberService } from "@/services/memberService";
import CameraCaptureModal from "../components/CameraCaptureModal";
import { Camera, Upload, X, AlertCircle } from "lucide-react";
import type { MemberDetails } from "@/types/member";
import { Input } from "./ui/Input";

interface Props {
  open: boolean;
  onClose: () => void;
  member: MemberDetails | null;
  onUpdated: () => void;
}

export default function EditMemberModal({ open, onClose, member, onUpdated }: Props) {
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
  });

  useEffect(() => {
    if (member) {
      setFormData({
        fullName: member.fullName || "",
        phone: member.phone || "",
      });
      setPreview(member.profilePicture || null);
      setFile(null); // Reset any previously selected file
      setError(null);
    }
  }, [member, open]);

  if (!open || !member) return null;

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
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
        await memberService.uploadProfilePicture(member.memberId, file);
      }

      await memberService.updateMember(member.memberId, {
        memberId: member.memberId,
        fullName: formData.fullName,
        phone: formData.phone,
      });

      onUpdated();
      onClose();
    } catch (err) {
      console.error(err);
      setError("Failed to update member");
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
        className="bg-white rounded-2xl w-full max-w-md shadow-elevated relative animate-fade-in-up"
      >
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100">
          <h2 className="text-lg font-bold tracking-tight text-slate-900">
            Edit Member Details
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
                  <img src={preview} className="w-full h-full object-cover" alt="Profile preview" />
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
            />

            <Input
              name="phone"
              label="Phone"
              value={formData.phone}
              onChange={handleChange}
            />
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
              disabled={loading}
            >
              Cancel
            </button>
            <button
              onClick={handleSubmit}
              disabled={loading}
              className="flex-1 py-2.5 text-sm font-semibold text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200 disabled:opacity-70 flex justify-center items-center"
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
