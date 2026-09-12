import React, { useEffect, useState } from 'react';
import { createPortal } from "react-dom";
import { Package as PackageIcon, Plus, Calendar, DollarSign, BookText, Edit3, Trash2 } from 'lucide-react';
import { packageService } from '../services/packageService';
import EditPackageModal from '../components/EditPackageModal';
import type { Package } from '../types/package';
import { Button } from '../components/ui/Button';
import { Card } from '../components/ui/Card';
import { Input } from '../components/ui/Input';
import { Modal } from '../components/ui/Modal';
import { Switch } from "@/components/ui/switch"

const Packages: React.FC = () => {
  const [packages, setPackages] = useState<Package[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingPackage, setEditingPackage] = useState<Package | null>(null);
  const [deleteConfirmId, setDeleteConfirmId] = useState<string | null>(null);
  
  // Form State
  const [name, setName] = useState('');
  const [price, setPrice] = useState('');
  const [durationInDays, setdurationInDays] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<{
  name?: string;
  price?: string;
  durationInDays?: string;
}>({});

  useEffect(() => {
    fetchPackages();
  }, []);

  const fetchPackages = async () => {
    try {
      const data = await packageService.getAllPackages();
      setPackages(data);
    } catch (err) {
      console.error('Failed to load packages');
    } finally {
      setLoading(false);
    }
  };

  const validate = () => {
    const newErrors: any = {};

    if (!name) newErrors.name = "Name is required";
    if (!price) newErrors.price = "Price is required";
    if (!durationInDays) newErrors.durationInDays = "Duration is required";

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
  };

  const handleCreatePackage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    try {
      setIsSubmitting(true);
      await packageService.createPackage({
        name,
        price: Number(price),
        durationInDays: Number(durationInDays),
        description: '',
        status:'ACTIVE'
      });
      await fetchPackages();
      setIsModalOpen(false);
      setName(''); setPrice(''); setdurationInDays('');
    } catch (error) {
      console.error("Failed to create package",error);
    } finally {
      setIsSubmitting(false);
    }
  };



  const handleDelete = async (id: string)=>{
    const pkg = packages.find(pkg => pkg.id === id);
    if(!pkg) return;
    try {
      await packageService.deletePackage(id);
      setPackages(prev => prev.filter(pkg => pkg.id !== id));
      setDeleteConfirmId(null);
    } catch (error) {
      console.error("Failed to delete package",error);
    }
  };
  
  const handleStatusToggle = async (id: string, currentStatus: string) => {
    const pkg = packages.find(p => p.id === id);
    if (!pkg) return;
    const newStatus = currentStatus === 'ACTIVE' ? 'INACTIVE' : 'ACTIVE';
    try {
      await packageService.updatePackageStatus(id, {
        name: pkg.name,
        price: pkg.price,
        durationInDays: pkg.durationInDays,
        description: pkg.description,
        status: newStatus,
      }, newStatus);
      setPackages(prev => prev.map(p => p.id === id ? { ...p, status: newStatus } : p));
    } catch (error) {
      console.error("Failed to update status", error);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight">Membership Packages</h2>
          <p className="text-sm text-slate-500 mt-1">Manage subscription plans and pricing.</p>
        </div>
        <button 
          onClick={() => setIsModalOpen(true)}
          className="inline-flex items-center gap-2 px-4 py-2.5 text-[13px] font-medium text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200"
        >
          <Plus className="w-4 h-4" /> 
          Create New Package
        </button>
      </div>

      {loading ? (
        <div className="flex flex-col items-center justify-center py-12 gap-3">
          <div className="w-8 h-8 border-2 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
          <p className="text-sm text-slate-400 font-medium">Loading packages...</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {packages.map((pkg) => (
            <Card key={pkg.id} hover padding="none" className="relative overflow-hidden group flex flex-col h-full border-t-4 border-t-indigo-500">
              {/* Decorative faint icon */}
              <div className="absolute top-4 right-4 text-slate-100 group-hover:text-indigo-50 transition-colors pointer-events-none transform translate-x-4 -translate-y-4">
                <PackageIcon size={120} />
              </div>
              
              <div className="p-6 pb-5 flex-1 relative z-10 flex flex-col">
                <div className="flex items-start justify-between mb-2">
                  <h3 className="text-[17px] font-bold text-slate-900 leading-tight pr-4">{pkg.name}</h3>
                  <div className="flex items-center mt-1">
                    <Switch 
                      id={pkg.id} 
                      checked={pkg.status === 'ACTIVE'}
                      onCheckedChange={() => handleStatusToggle(pkg.id, pkg.status)}
                      className="data-[state=checked]:bg-emerald-500"
                    />
                  </div>
                </div>
                
                <div className="flex items-start text-indigo-600 mb-6">
                  <span className="text-lg font-bold mt-1.5 mr-0.5">$</span>
                  <span className="text-4xl font-extrabold tracking-tight">{pkg.price}</span>
                </div>
                
                <div className="space-y-3 mt-auto flex-1">
                  <div className="flex items-center text-[13px] text-slate-600 font-medium">
                    <Calendar className="w-4 h-4 mr-2.5 text-slate-400" />
                    {pkg.durationInDays} days duration
                  </div>
                  {pkg.description && (
                     <div className="flex items-start text-[13px] text-slate-600 font-medium">
                       <BookText className="w-4 h-4 mr-2.5 mt-0.5 text-slate-400 flex-shrink-0" /> 
                       <span className="leading-snug">{pkg.description}</span>
                     </div>
                  )}
                </div>
              </div>

              {/* Actions Footer */}
              <div className="px-5 py-3.5 bg-slate-50 border-t border-slate-100 flex gap-2 relative z-10">
                <button 
                  onClick={() => {
                    setEditingPackage(pkg);
                    setIsEditModalOpen(true);
                  }}
                  className="flex-1 inline-flex items-center justify-center gap-1.5 py-2 text-[13px] font-semibold text-slate-600 bg-white border border-slate-200 rounded-lg shadow-sm hover:text-indigo-600 hover:border-indigo-200 transition-colors"
                >
                  <Edit3 className="w-3.5 h-3.5" />
                  Edit
                </button>
                <button 
                  onClick={() => setDeleteConfirmId(pkg.id)}
                  className="inline-flex items-center justify-center p-2 text-slate-400 bg-white border border-slate-200 rounded-lg shadow-sm hover:text-rose-600 hover:border-rose-200 hover:bg-rose-50 transition-colors"
                  title="Delete Package"
                >
                  <Trash2 className="w-4 h-4" />
                </button>
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Create Package Modal */}
      <Modal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)}
        title="Create New Package"
      >
        <form onSubmit={handleCreatePackage} className="space-y-4">
          <Input 
            label="Plan Name" 
            placeholder="e.g. 1 Year Premium"
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={errors.name}
          />
          
          <div className="grid grid-cols-2 gap-4">
            <Input 
              label="Price ($)" 
              type="number" 
              placeholder="e.g. 200"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
              error={errors.price}
            />
            
            <Input 
              label="Duration (days)" 
              type="number" 
              placeholder="e.g. 30"
              value={durationInDays}
              onChange={(e) => setdurationInDays(e.target.value)}
              error={errors.durationInDays}
            />
          </div>
          
          <div className="flex gap-3 pt-6 flex-row-reverse">
            <button 
              type="submit" 
              disabled={isSubmitting}
              className="flex-1 py-2.5 text-sm font-semibold text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm hover:shadow-md transition-all duration-200 disabled:opacity-70"
            >
              {isSubmitting ? 'Creating...' : 'Create Package'}
            </button>
            <button 
              type="button" 
              onClick={() => setIsModalOpen(false)}
              className="flex-1 py-2.5 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-xl hover:bg-slate-50 transition-colors"
            >
              Cancel
            </button>
          </div>
        </form>
      </Modal>

      <EditPackageModal 
        isOpen={isEditModalOpen} 
        onClose={() => {
          setIsEditModalOpen(false);
          setEditingPackage(null);
        }} 
        packageItem={editingPackage}
        onUpdate={(updatedPkg) => {
          setPackages(prev => prev.map(p => p.id === updatedPkg.id ? updatedPkg : p));
        }}
      />

      {/* Delete Confirm Dialog */}
      {deleteConfirmId && createPortal(
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-[100] animate-in fade-in duration-200">
          <div className="bg-white rounded-2xl p-6 w-[380px] shadow-elevated relative animate-fade-in-up">
            <div className="w-12 h-12 bg-rose-50 border-8 border-rose-50/50 rounded-full flex items-center justify-center mx-auto mb-4 text-rose-500">
              <Trash2 className="w-6 h-6" />
            </div>
            <h3 className="text-lg font-bold text-slate-900 text-center mb-1">
              Delete Package
            </h3>
            <p className="text-sm text-slate-500 text-center mb-6">
              Are you sure you want to delete this package? This action cannot be undone.
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

export default Packages;
