import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Search, Eye, Plus, UserPlus, AlertCircle, CheckCircle2 } from 'lucide-react';
import { memberService} from '../services/memberService';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';
import { Table, type Column } from '../components/ui/Table';
import { Card } from '../components/ui/Card';
import type { Member } from '../types/member';
import AddMemberModal from '../components/AddMemberModal';
import { getRemainingDays } from '@/utils/date';
import { useDebounce } from '@/hooks/useDebounce';

const Members: React.FC = () => {
  const [members, setMembers] = useState<Member[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [open,setOpen]=useState(false);
  const [error,setError]=useState<string|null>(null);
  const navigate = useNavigate();

  const [page,setPage]=useState(0);
  const [totalPages,setTotalPages]=useState(0);

  const debouncedSearchTerm = useDebounce(searchTerm);

  useEffect(() => {
    const fetchMembers = async () => {
      try {
        const data = await memberService.getAllMembers(page,20,debouncedSearchTerm);
        setMembers(data.content);
        setTotalPages(data.totalPages);
      } catch (err) {
        setError("Failed to fetch members");
      } finally {
        setLoading(false);
      }
    };
    fetchMembers();
  }, [page,debouncedSearchTerm]);

  const filteredMembers = members.filter(m => 
    m.fullName.toLowerCase().includes(searchTerm.toLowerCase()) || 
    m.phone.includes(searchTerm)
  );

  const columns: Column<Member>[] = [
    { key: 'memberId', header: 'ID' },
    { 
      key: 'fullName', 
      header: 'Name', 
      render: (row) => <span className="font-semibold text-slate-800">{row.fullName}</span> 
    },
    { key: 'phone', header: 'Phone' },
    { key: "gender", header: "Gender" }, 
    { 
      key: 'debt', 
      header: 'Debt', 
      render: (row) => {
        if((row.debt ?? 0) > 0)
          return (
            <div className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-amber-50 text-amber-600 font-medium text-xs border border-amber-200/50">
              <AlertCircle className="w-3.5 h-3.5" />
              <span>${row.debt}</span>
            </div>
          );
        else
          return (
            <div className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-md bg-emerald-50 text-emerald-600 font-medium text-xs border border-emerald-200/50">
              <CheckCircle2 className="w-3.5 h-3.5" />
              <span>Clear</span>
            </div>
          );
      }
    },
    { 
      key: 'endDate', 
      header: 'Status',
      render: (row) => {
        if(!row.endDate) {
          return (
            <span className="inline-flex px-2.5 py-1 rounded-md bg-rose-50 text-rose-600 font-medium text-xs border border-rose-200/50">
              Inactive
            </span>
          );
        }
        
        const days = getRemainingDays(row.endDate);
        
        return (
          <div className="flex flex-col gap-0.5">
            <span className={`inline-flex px-2 py-0.5 rounded text-[11px] font-semibold w-max ${
              days <= 3 ? "bg-rose-50 text-rose-600 border border-rose-200/50" : "bg-emerald-50 text-emerald-600 border border-emerald-200/50"
            }`}>
              {days > 0 ? `${days} days left` : "Expired"}
            </span>
            <span className="text-[11px] text-slate-400 font-medium">
              Until {row.endDate}
            </span>
          </div>
        )
      }
    },
    { key: 'addedByName', header: 'Added By' },
    {
      key: 'actions', 
      header: 'Actions', 
      render: (row) => (
        <button 
          onClick={() => navigate(`/members/${row.memberId}`)} 
          className="inline-flex items-center justify-center gap-1.5 px-3 py-1.5 text-xs font-medium bg-white text-indigo-600 border border-indigo-200 shadow-sm rounded-lg hover:bg-indigo-50 hover:border-indigo-300 transition-all focus:outline-none focus:ring-2 focus:ring-indigo-500/20"
        >
          <Eye className="w-3.5 h-3.5" /> 
          View
        </button>
      )
    }
  ];

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 tracking-tight">Members</h2>
          <p className="text-sm text-slate-500 mt-1">Manage and view all gym members</p>
        </div>
        <button 
          onClick={() => setOpen(true)}
          className="inline-flex items-center gap-2 px-4 py-2.5 text-[13px] font-medium text-white bg-gradient-to-r from-indigo-500 to-violet-600 rounded-xl shadow-sm shadow-indigo-500/20 hover:shadow-md hover:shadow-indigo-500/30 transition-all duration-200"
        >
          <UserPlus className="w-4 h-4" /> 
          Add Member
        </button>
        <AddMemberModal open={open} onClose={() => setOpen(false)} onSuccess={(newMember) => setMembers(prev => [...prev, newMember])} />
      </div>

      <Card padding="md" className="space-y-4">
        <div className="flex items-center">
          <div className="relative flex-1 max-w-md group">
            <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
              <Search className="h-[18px] w-[18px] text-slate-400 group-focus-within:text-indigo-500 transition-colors" />
            </div>
            <input
              type="text"
              placeholder="Search members by name or phone..."
              value={searchTerm}
              onChange={(e) => {
                setSearchTerm(e.target.value);
                setPage(0);
              }}
              className="w-full pl-10 pr-4 py-2.5 text-sm bg-slate-50 border border-slate-200 rounded-xl focus:bg-white focus:outline-none focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-400 transition-all placeholder:text-slate-400"
            />
          </div>
        </div>

        {loading ? (
          <div className="flex flex-col items-center justify-center py-12 gap-3">
             <div className="w-8 h-8 border-2 border-indigo-200 border-t-indigo-600 rounded-full animate-spin" />
             <p className="text-sm text-slate-400 font-medium">Loading members...</p>
          </div>
        ) : error ? (
          <div className="p-4 bg-rose-50 border border-rose-100 rounded-xl text-rose-600 text-sm font-medium flex items-center gap-2">
            <AlertCircle className="w-4 h-4" />
            {error}
          </div>
        ) : (
          <Table data={filteredMembers} columns={columns} keyExtractor={(row) => row.memberId}  />
        )}
        
        <div className="flex justify-between items-center pt-4 border-t border-slate-100 mt-2">
          <span className="text-xs font-medium text-slate-500 bg-slate-50 px-2.5 py-1 rounded-md border border-slate-100">
            Page {page + 1} of {totalPages || 1}
          </span>

          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              disabled={page === 0}
              onClick={() => setPage(p => p - 1)}
              className="text-xs h-8 text-slate-600 hover:text-slate-900 border-slate-200"
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              disabled={page >= Math.max(totalPages - 1, 0)}
              onClick={() => setPage(p => p + 1)}
              className="text-xs h-8 text-slate-600 hover:text-slate-900 border-slate-200"
            >
              Next
            </Button>
          </div>
        </div>
      </Card>
      
    </div>
  );
};

export default Members;
