import { Modal } from "@/components/ui/Modal";
import { Input } from "@/components/ui/Input";
import { packageService } from "@/services/packageService";
import type { Package } from "@/types/package";
import { useState, useEffect } from "react";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  packageItem: Package | null;
  onUpdate: (updatedPkg: Package) => void;
}

const EditPackageModal = ({
  isOpen,
  onClose,
  packageItem,
  onUpdate,
}: Props) => {
  const [price, setPrice] = useState("");
  const [durationInDays, setDurationInDays] = useState("");
  const [name, setName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (packageItem) {
      setName(packageItem.name);
      setPrice(packageItem.price.toString());
      setDurationInDays(packageItem.durationInDays.toString());
      console.log(durationInDays);
    } else {
      setName("");
      setPrice("");
      setDurationInDays("");
    }
  }, [packageItem]);

  const handleEditPackage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!packageItem) return;

    try {
      setIsSubmitting(true);
      const updatedPackage: Package = {
        ...packageItem,
        name,
        price: Number(price),
        durationInDays: Number(durationInDays),
      };
      await packageService.updatePackage(packageItem.id, {
        name: updatedPackage.name,
        price: updatedPackage.price,
        durationInDays: updatedPackage.durationInDays,
        description: updatedPackage.description,
        status: updatedPackage.status,
      });
      onUpdate(updatedPackage);
      onClose();
    } catch (error) {
      console.error(error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Edit Package">
      <form onSubmit={handleEditPackage} className="space-y-4">
        <Input
          label="Plan Name"
          placeholder="e.g. 1 Year Premium"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        
        <div className="grid grid-cols-2 gap-4">
          <Input
            label="Price ($)"
            type="number"
            placeholder="e.g. 200"
            value={price}
            onChange={(e) => setPrice(e.target.value)}
            required
            className="mb-0"
          />
          <Input
            label="Duration (days)"
            type="number"
            placeholder="e.g. 30"
            value={durationInDays}
            onChange={(e) => setDurationInDays(e.target.value)}
            required
            className="mb-0"
          />
        </div>

        <div className="flex gap-3 pt-6 mt-2 border-t border-slate-100">
          <button
            type="button"
            className="flex-1 py-2.5 text-sm font-semibold text-slate-600 bg-white border border-slate-200 rounded-xl hover:bg-slate-50 transition-all duration-200"
            onClick={onClose}
            disabled={isSubmitting}
          >
            Cancel
          </button>
          <button 
            type="submit" 
            disabled={isSubmitting}
            className="flex-1 py-2.5 text-sm font-semibold text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200 flex items-center justify-center disabled:opacity-70"
          >
            {isSubmitting ? (
              <div className="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            ) : "Save Changes"}
          </button>
        </div>
      </form>
    </Modal>
  );
};

export default EditPackageModal;
