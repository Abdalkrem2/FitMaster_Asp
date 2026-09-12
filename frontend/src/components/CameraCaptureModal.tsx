import { useRef, useState, useEffect } from 'react';
import { Camera, X, RefreshCcw } from 'lucide-react';

interface Props {
  open: boolean;
  onClose: () => void;
  onCapture: (file: File) => void;
}

export default function CameraCaptureModal({ open, onClose, onCapture }: Props) {
  const videoRef = useRef<HTMLVideoElement>(null);
  const streamRef = useRef<MediaStream | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [facingMode, setFacingMode] = useState<'user' | 'environment'>('user');

  useEffect(() => {
    if (open) {
      startCamera();
    } else {
      stopCamera();
    }
    return () => {
      stopCamera();
    };
  }, [open, facingMode]);

  const startCamera = async () => {
    try {
      stopCamera();
      setError(null);
      const stream = await navigator.mediaDevices.getUserMedia({ 
        video: { facingMode } 
      });
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        videoRef.current.play();
      }
      streamRef.current = stream;
    } catch (err) {
      console.error("Error accessing camera:", err);
      setError("Could not access camera. Please check permissions.");
    }
  };

  const stopCamera = () => {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach(track => track.stop());
      streamRef.current = null;
    }
  };

  const handleCapture = () => {
    if (videoRef.current) {
      const canvas = document.createElement('canvas');
      canvas.width = videoRef.current.videoWidth;
      canvas.height = videoRef.current.videoHeight;
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.drawImage(videoRef.current, 0, 0, canvas.width, canvas.height);
        canvas.toBlob((blob) => {
          if (blob) {
            const file = new File([blob], `capture_${Date.now()}.jpg`, { type: 'image/jpeg' });
            onCapture(file);
            onClose();
          }
        }, 'image/jpeg', 0.9);
      }
    }
  };

  if (!open) return null;

  return (
    <div 
      className="fixed inset-0 bg-black bg-opacity-90 flex items-center justify-center z-[110]"
      onClick={(e) => {
        e.stopPropagation();
        onClose();
      }}
    >
      <div 
        className="bg-white rounded-2xl p-4 w-[90%] max-w-[500px] shadow-xl relative"
        onClick={(e) => e.stopPropagation()}
      >
        <button 
          onClick={onClose}
          className="absolute top-2 right-2 p-2 rounded-full hover:bg-gray-100 z-10"
        >
          <X size={20} />
        </button>
        <h2 className="text-lg font-bold mb-4">Capture Photo</h2>
        
        {error ? (
          <div className="text-red-500 mb-4 text-center p-4 bg-red-50 rounded">
            {error}
          </div>
        ) : (
          <div className="relative rounded-lg overflow-hidden bg-black mb-4 aspect-video flex items-center justify-center">
            <video 
              ref={videoRef} 
              autoPlay 
              playsInline
              muted
              className="w-full h-full object-cover"
            />
          </div>
        )}

        <div className="flex justify-between items-center mt-4">
          <div className="flex gap-2">
            <button
              onClick={startCamera}
              className="px-3 py-2 border rounded hover:bg-gray-50 text-sm"
            >
              Retry
            </button>
            <button
              onClick={() => setFacingMode(prev => prev === 'user' ? 'environment' : 'user')}
              className="px-3 py-2 border rounded hover:bg-gray-50 text-sm flex items-center gap-1"
            >
              <RefreshCcw size={16} />
              <span>Flip</span>
            </button>
          </div>
          {!error && (
            <button
              onClick={handleCapture}
              className="flex items-center space-x-2 bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
            >
              <Camera size={20} />
              <span>Take Photo</span>
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
