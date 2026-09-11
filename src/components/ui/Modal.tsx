import React, { useEffect } from 'react';
import { createPortal } from 'react-dom';
import { X } from 'lucide-react';

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  title?: string;
  children: React.ReactNode;
}

export const Modal: React.FC<ModalProps> = ({ isOpen, onClose, title, children }) => {
  // Close on Escape key
  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose();
    };
    window.addEventListener('keydown', handleEsc);
    return () => window.removeEventListener('keydown', handleEsc);
  }, [onClose]);

  if (!isOpen) return null;

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* Backdrop */}
      <div 
        className="fixed inset-0 bg-black/40 backdrop-blur-sm transition-opacity" 
        onClick={onClose}
        aria-hidden="true"
      />
      
      {/* Modal Dialog */}
      <div className="bg-white rounded-2xl shadow-elevated w-full max-w-md max-h-[90vh] overflow-y-auto relative z-10 animate-fade-in-up">
        {title && (
          <div className="sticky top-0 bg-white/90 backdrop-blur-sm border-b border-slate-100/60 px-6 py-4 flex items-center justify-between z-20">
            <h3 className="text-lg font-bold tracking-tight text-slate-900">{title}</h3>
            <button 
              onClick={onClose}
              className="text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition-colors rounded-xl p-1.5 focus:outline-none"
            >
              <X size={18} />
            </button>
          </div>
        )}
        
        <div className="p-6">
          {children}
        </div>
      </div>
    </div>,
    document.body
  );
};
