import React, { useEffect, useState } from "react";
import { createPortal } from "react-dom";
import { useParams, useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  User,
  Phone,
  Calendar,
  CreditCard,
  Activity,
  Edit3,
  Trash2,
  AlertCircle,
  Plus
} from "lucide-react";
import { memberService } from "../services/memberService";
import { membershipService } from "../services/membershipService";
import type { MemberDetails } from "../types/member";
import { Button } from "../components/ui/Button";
import { Card } from "../components/ui/Card";
import { Input } from "../components/ui/Input";
import { Table, type Column } from "../components/ui/Table";
import { packageService } from "../services/packageService";
import type { Package } from "@/types/package";
import type { Membership } from "@/types/membership";
import EditMemberModal from "../components/EditMemberModal";

const MemberDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [member, setMember] = useState<MemberDetails | null>(null);
  const [memberships, setMemberships] = useState<Membership[]>([]);
  const [loading, setLoading] = useState(true);

  const [price, setPrice] = useState("");
  const [amountPaid, setAmountPaid] = useState("");
  const [description, setDescription] = useState("");
  const [addingMembership, setAddingMembership] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);

  const [selectedPackageId, setSelectedPackageId] = useState("");
  const [packages, setPackages] = useState<Package[]>([]);

  useEffect(() => {
    const fetchPackages = async () => {
      const packagesData = await packageService.getAllPackages();
      setPackages(packagesData);
    };
    fetchPackages();
  }, []);

  const handleAddMembership = async (e: React.FormEvent) => {
    if (!member) return;
    e.preventDefault();

    setAddingMembership(true);

    try {
      await membershipService.addMembership(String(id), {
        startDate: new Date().toISOString().slice(0, 10),
        price: Number(price),
        amountPaid: Number(amountPaid),
        description: description,
        packageId: Number(selectedPackageId),
      });
      const updatedMemberData = await memberService.getMemberById(id as string);
      if (updatedMemberData) {
        setMember(updatedMemberData);
      }
      const updatedMemberships = await membershipService.getByMember(id as string);
      setMemberships(updatedMemberships);

      setPrice("");
      setAmountPaid("");
      setDescription("");
      setSelectedPackageId("");
    } catch (err) {
      console.log(err);
    } finally {
      setAddingMembership(false);
    }
  };

  const loadMemberData = async () => {
    if (!id) return;
    try {
      const memberData = await memberService.getMemberById(id);
      if (memberData) {
        setMember(memberData);
      }
      const membershipsData = await membershipService.getByMember(id);
      setMemberships(membershipsData);
    } catch (err) {
      console.error("Error fetching data:", err);
    }
  };

  useEffect(() => {
    if (!id) return;

    const fetchData = async () => {
      try {
        const memberData = await memberService.getMemberById(id);
        if (memberData) {
          setMember(memberData);
        }
        const membershipsData = await membershipService.getByMember(id);
        setMemberships(membershipsData);
      } catch (err) {
        console.error("Error fetching data:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [id]);

  const handleDeleteMember = async () => {
    if (!member) return;
    try {
      await memberService.deleteMember(String(id));
      navigate("/members");
    } catch (err) {
      console.log(err);
    }
  };

  if (loading)
    return (
      <div className="flex flex-col items-center justify-center py-24 gap-3">
        <div className="w-8 h-8 border-2 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
        <p className="text-sm text-slate-400 font-medium">Loading details...</p>
      </div>
    );
    
  if (!member)
    return (
      <div className="text-center py-24">
         <div className="w-16 h-16 bg-rose-50 rounded-full flex items-center justify-center mx-auto mb-4 border border-rose-100">
             <AlertCircle className="text-rose-400 w-8 h-8" />
          </div>
          <h3 className="text-sm font-semibold text-slate-800">Member not found</h3>
          <p className="text-xs text-slate-500 mt-1">The requested member ID does not exist.</p>
          <Button onClick={() => navigate("/members")} variant="outline" className="mt-4">
             Back to Members
          </Button>
      </div>
    );

  const billColumns: Column<Membership>[] = [
    { key: "startDate", header: "Date" },
    { key: "packageName", header: "Package" },
    {
      key: "price",
      header: "Price",
      render: (row) => <span className="font-semibold text-emerald-600">${row.price}</span>
    },
    {
      key: "debt",
      header: "Debt",
      render: (row) =>
        (row.debt ?? 0) > 0 ? (
          <span className="inline-flex px-2 py-0.5 rounded text-[11px] font-semibold bg-rose-50 text-rose-600 border border-rose-200/50">
            ${row.debt}
          </span>
        ) : (
          <span className="text-slate-400 font-medium">-</span>
        ),
    },
    { key: "description", header: "Notes" },
  ];

  return (
    <div className="space-y-6 max-w-7xl mx-auto">
      {/* Header Actions */}
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <button
            onClick={() => navigate("/members")}
            className="p-2 border border-slate-200 bg-white rounded-xl shadow-sm text-slate-500 hover:text-slate-900 hover:bg-slate-50 transition-all focus:outline-none"
            title="Go Back"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div>
            <h2 className="text-2xl font-bold text-slate-900 tracking-tight">Member Details</h2>
            <p className="text-sm text-slate-500 mt-1">View profile and billing history.</p>
          </div>
        </div>
        <div className="flex gap-2">
           <button
             onClick={() => setIsEditModalOpen(true)}
             className="inline-flex items-center justify-center gap-1.5 px-4 py-2.5 text-[13px] font-semibold bg-white text-indigo-600 border border-indigo-200 shadow-sm rounded-xl hover:bg-indigo-50 hover:border-indigo-300 transition-all focus:outline-none"
           >
             <Edit3 className="w-3.5 h-3.5" /> Edit Profile
           </button>
           <button
             onClick={() => setDeleteConfirmOpen(true)}
             className="inline-flex items-center justify-center gap-1.5 px-4 py-2.5 text-[13px] font-semibold bg-white text-rose-600 border border-rose-200 shadow-sm rounded-xl hover:bg-rose-50 hover:border-rose-300 transition-all focus:outline-none"
           >
             <Trash2 className="w-3.5 h-3.5" /> Delete
           </button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Profile Card */}
        <div className="lg:col-span-1 space-y-6">
          <Card className="flex flex-col items-center text-center p-8 relative overflow-hidden">
            {/* Background decorative header */}
            <div className="absolute top-0 left-0 right-0 h-32 bg-gradient-to-br from-indigo-50 via-slate-50 to-white -z-10 border-b border-slate-100" />
            
            <div className="w-28 h-28 bg-white p-1.5 rounded-full shadow-sm border border-slate-100 mb-4 flex items-center justify-center text-slate-400 z-10">
              <div className="w-full h-full rounded-full bg-slate-100 flex items-center justify-center overflow-hidden">
                {member.profilePicture ? (
                  <img
                    src={member.profilePicture}
                    className="w-full h-full object-cover"
                    alt={member.fullName}
                  />
                ) : (
                  <User size={48} />
                )}
              </div>
            </div>

            <h3 className="text-xl font-bold text-slate-900 tracking-tight">
              {member.fullName}
            </h3>
            <span className="inline-flex px-2.5 py-1 rounded-md bg-slate-100 text-slate-600 font-medium text-[11px] uppercase tracking-wider mt-2 border border-slate-200">
               {member.gender}
            </span>

            <div className="w-full mt-6 space-y-3 text-left">
              <div className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                <div className="flex items-center text-slate-600 text-[13px] font-medium">
                  <Phone className="w-4 h-4 mr-2.5 text-slate-400" /> Phone
                </div>
                <span className="text-[13px] font-bold text-slate-800">{member.phone}</span>
              </div>
              
              <div className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                <div className="flex items-center text-slate-600 text-[13px] font-medium">
                  <Calendar className="w-4 h-4 mr-2.5 text-slate-400" /> Registered
                </div>
                <span className="text-[13px] font-bold text-slate-800">
                  {new Date(member.createdAt).toLocaleDateString()}
                </span>
              </div>

              <div className="flex items-center justify-between p-3 bg-slate-50 rounded-xl border border-slate-100">
                <div className="flex items-center text-slate-600 text-[13px] font-medium">
                  <Activity className="w-4 h-4 mr-2.5 text-slate-400" /> End Date
                </div>
                <span className="text-[13px] font-bold text-slate-800">{member.endDate || "-"}</span>
              </div>
              
              <div className="flex items-center justify-between p-3 bg-rose-50 rounded-xl border border-rose-100 mt-2">
                <div className="flex items-center text-rose-600 text-[13px] font-medium">
                  <CreditCard className="w-4 h-4 mr-2.5 opacity-80" /> Total Debt
                </div>
                <span className="text-[14px] font-bold text-rose-600">${member.debt}</span>
              </div>
            </div>
          </Card>
        </div>

        {/* Billing & Forms */}
        <div className="lg:col-span-2 space-y-6">
          <Card padding="lg" className="border border-indigo-100 shadow-sm relative overflow-hidden">
             {/* Subtle gradient overlay */}
             <div className="absolute top-0 right-0 w-64 h-64 bg-indigo-50/50 rounded-full blur-3xl -z-10 -mr-16 -mt-16 pointer-events-none" />

            <div className="mb-6">
              <h3 className="text-lg font-bold text-slate-900">Add New Subscription</h3>
              <p className="text-sm text-slate-500 mt-0.5">Enroll member into a new package plan</p>
            </div>

            <form
              onSubmit={handleAddMembership}
              className="grid grid-cols-1 md:grid-cols-2 gap-x-5 gap-y-4"
            >
              <div className="col-span-1 border-b border-slate-200/60 pb-2 md:border-b-0 md:pb-0">
                <label className="block text-sm font-medium text-slate-700 mb-1">
                  Select Package
                </label>
                <select
                   value={selectedPackageId}
                   onChange={(e) => {
                     const pkgId = e.target.value;
                     setSelectedPackageId(pkgId);
                     const pkg = packages.find((p) => String(p.id) === pkgId);
                     if (pkg) {
                       setPrice(String(pkg.price));
                       setAmountPaid(String(pkg.price));
                     }
                   }}
                   required
                   className="w-full px-3 py-2 text-sm border border-slate-200 bg-slate-50 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-400 focus:bg-white transition-all h-10"
                >
                  <option value="" disabled>Select Package</option>
                  {packages
                    .filter((p) => p.status === "ACTIVE")
                    .map((pkg) => (
                      <option key={pkg.id} value={String(pkg.id)}>
                        {pkg.name} (${pkg.price})
                      </option>
                  ))}
                </select>

                <div className="mt-4 grid grid-cols-2 gap-3">
                  <Input
                    label="Duration (Read-only)"
                    type="text"
                    value={
                      selectedPackageId
                        ? `${packages.find((p) => String(p.id) === selectedPackageId)?.durationInDays ?? 0} Days`
                        : "—"
                    }
                    disabled
                    className="bg-slate-100 text-slate-500 border-slate-200 mb-0"
                  />
                  <Input
                    label="Debt (Calculated)"
                    type="text"
                    value={`$${Math.max(Number(price || 0) - Number(amountPaid || 0), 0)}`}
                    disabled
                    className="bg-slate-100 text-rose-600 font-semibold border-slate-200 mb-0"
                  />
                </div>
              </div>

              <div className="col-span-1">
                 <Input
                    label="Price ($)"
                    type="number"
                    value={price}
                    onChange={(e) => setPrice(e.target.value)}
                    placeholder="E.g. 150"
                    required
                  />
                 <Input
                    label="Amount Paid ($)"
                    type="number"
                    value={amountPaid}
                    onChange={(e) => setAmountPaid(e.target.value)}
                    placeholder="E.g. 40"
                    required
                  />
                 <Input
                    label="Extra Notice (Optional)"
                    placeholder="Enter any notes"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    className="mb-0"
                  />
              </div>

              <div className="md:col-span-2 flex justify-end pt-4 mt-2 border-t border-slate-100">
                <button 
                  type="submit" 
                  disabled={addingMembership}
                  className="inline-flex items-center gap-2 px-5 py-2.5 text-[13px] font-semibold text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200 focus:outline-none disabled:opacity-70 disabled:cursor-not-allowed"
                >
                  {addingMembership ? (
                     <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                  ) : (
                    <Plus className="w-4 h-4" />
                  )}
                  {addingMembership ? "Enrolling..." : "Add Subscription"}
                </button>
              </div>
            </form>
          </Card>

          <Card padding="lg" className="border border-slate-200">
            <div className="mb-4">
               <h3 className="text-lg font-bold text-slate-900">Billing History</h3>
            </div>
            
            {memberships.length === 0 ? (
               <div className="py-12 border-2 border-dashed border-slate-200 rounded-xl flex flex-col items-center justify-center text-slate-400">
                  <CreditCard className="w-8 h-8 mb-2 text-slate-300" />
                  <p className="text-sm font-medium">No past subscriptions found</p>
               </div>
            ) : (
              <Table
                data={memberships}
                columns={billColumns}
                keyExtractor={(row) => row.id}
              />
            )}
          </Card>
        </div>
      </div>

      <EditMemberModal
        open={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
        member={member}
        onUpdated={loadMemberData}
      />

      {/* Delete Confirm Dialog */}
      {deleteConfirmOpen && createPortal(
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-[100] animate-in fade-in duration-200">
          <div className="bg-white rounded-2xl p-6 w-[380px] shadow-elevated relative animate-fade-in-up">
            <div className="w-12 h-12 bg-rose-50 border-8 border-rose-50/50 rounded-full flex items-center justify-center mx-auto mb-4 text-rose-500">
              <Trash2 className="w-6 h-6" />
            </div>
            <h3 className="text-lg font-bold text-slate-900 text-center mb-1">
              Delete Member
            </h3>
            <p className="text-sm text-slate-500 text-center mb-6">
              Are you sure you want to delete this member? All associated records will be removed.
            </p>
            <div className="flex gap-3">
               <button
                onClick={() => setDeleteConfirmOpen(false)}
                className="flex-1 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-xl py-2.5 hover:bg-slate-50 transition-colors"
               >
                Cancel
               </button>
               <button
                onClick={handleDeleteMember}
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

export default MemberDetails;
