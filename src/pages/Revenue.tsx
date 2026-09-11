import React, { useEffect, useState } from 'react';
import { revenueService } from '../services/revenueService';
import type { MonthlyRevenue, RevenueByPeriod, RevenueStats ,RevenueRow} from '../types/revenue';
import { Table, type Column } from '@/components/ui/Table';
import { Card } from '@/components/ui/Card';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { TrendingUp, AlertCircle, Calendar, BarChart3, Filter } from 'lucide-react';

const months = [
  '1-JAN', '2-Feb', '3-Mar', '4-Apr', '5-May', '6-Jun',
  '7-Jul', '8-Aug', '9-Sep', '10-Oct', '11-Nov', '12-Dec'
];

const Revenue: React.FC = () => {
  const [stats, setStats] = useState<RevenueStats>({
    today: 0,
    thisMonth: 0,
    thisYear: 0,
    debt: 0
  });
  
  // Filters
  const [selectedYear, setSelectedYear] = useState('2026');
  const [monthlyRevenue, setMonthlyRevenue] = useState<MonthlyRevenue|null>(null);
  const [revenueByPeriod, setRevenueByPeriod] = useState<RevenueByPeriod|null>(null);
  const [error,setError] = useState<Error|null>(null);
  
  const [dateRange, setDateRange] = useState({ start: '', end: '' });

  const fetchStats = async () => {
    try {
      const data = await revenueService.getStats();
      setStats({
        today: data.today,
        thisMonth: data.thisMonth,
        thisYear: data.thisYear,
        debt: data.debt 
      });
     
    } catch (err) {
      setError(err as Error);
    }
  };

  const fetchMonthlyRevenue = async () => {
    try {
      const data = await revenueService.monthlyRevenue(Number(selectedYear));
      setMonthlyRevenue(data);
    } catch (err) {
      setError(err as Error);
    }
  };

  const fetchRevenueByPeriod = async () => {
    try {
      const data = await revenueService.revenueByPeriod(dateRange.start,dateRange.end);
      setRevenueByPeriod(data);
    } catch (err) {
      setError(err as Error);
    }
  };

  useEffect(() => {
    fetchMonthlyRevenue();
  }, [selectedYear]);

  useEffect(() => {
    fetchStats();
  }, []);

  const columns: Column<RevenueRow>[] = [
    {key:'id',header: 'ID'},
    {
      key:'memberName',
      header: 'Member Name',
      render: (row) => <span className="font-semibold text-slate-800 dark:text-slate-200">{row.memberName}</span> 
    },
    { 
      key: 'amount', 
      header: 'Amount',
      render: (row) => <span className="text-emerald-600 dark:text-emerald-400 font-bold">${row.amount}</span>
    },
    {
      key:'debt',
      header: 'Debt',
      render: (row) => {
        if((row.debt ?? 0) > 0)
          return (
            <span className="inline-flex px-2.5 py-1 rounded-md text-[11px] font-bold bg-rose-50 dark:bg-rose-500/10 text-rose-600 dark:text-rose-400 border border-rose-200/50 dark:border-rose-500/20 uppercase tracking-wider">
              ${row.debt}
            </span>
          );
        return <span className="text-slate-400 dark:text-slate-500 font-medium">-</span>;
      }
    },
    {
      key:'pkg',
      header: 'Package',
      render: (row) => <span className="text-slate-600 dark:text-slate-300 text-sm font-medium">{row.pkg}</span>
    },
    {key:'addedByName',header: 'Added By'},
    {key:'createdAt',header: 'Date'},
    {key:'description',header: 'Description'},
  ];

  return (
    <div className='max-w-7xl mx-auto space-y-8 animate-fade-in-up'>
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold text-slate-900 dark:text-white tracking-tight">Revenue Analytics</h2>
          <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">Track payments, financial growth, and open debts.</p>
        </div>
      </div>

      {error && (
        <div className="bg-rose-50 dark:bg-rose-500/10 border border-rose-200 dark:border-rose-500/20 text-rose-600 dark:text-rose-400 p-4 rounded-xl text-sm font-medium flex items-center gap-3 animate-pulse">
          <AlertCircle className="w-5 h-5 flex-shrink-0" />
          Something went wrong. Please try again.
        </div>
      )}

      {/* Top Stats Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
        <Card padding="none" className="relative overflow-hidden border-none shadow-md hover:shadow-lg transition-all transform hover:-translate-y-1 group bg-gradient-to-br from-indigo-500 to-violet-600">
           <div className="absolute top-0 right-0 w-32 h-32 bg-white/10 rounded-full blur-2xl group-hover:bg-white/20 transition-colors -mr-8 -mt-8 pointer-events-none" />
           <div className="p-6 relative z-10">
             <h3 className="text-white/80 font-semibold text-sm tracking-wide uppercase">Today</h3>
             <p className="text-white text-4xl font-extrabold mt-2 tracking-tight drop-shadow-sm">${stats.today}</p>
           </div>
        </Card>
        
        <Card padding="none" className="relative overflow-hidden border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 shadow-sm hover:shadow-md transition-all transform hover:-translate-y-1 group">
           <div className="p-6">
             <h3 className="text-slate-500 dark:text-slate-400 font-semibold text-sm tracking-wide uppercase">This Month</h3>
             <p className="text-slate-900 dark:text-white text-4xl font-extrabold mt-2 tracking-tight">${stats.thisMonth}</p>
           </div>
        </Card>

        <Card padding="none" className="relative overflow-hidden border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 shadow-sm hover:shadow-md transition-all transform hover:-translate-y-1 group">
           <div className="p-6">
             <h3 className="text-slate-500 dark:text-slate-400 font-semibold text-sm tracking-wide uppercase">This Year</h3>
             <p className="text-slate-900 dark:text-white text-4xl font-extrabold mt-2 tracking-tight">${stats.thisYear}</p>
           </div>
        </Card>

        <Card padding="none" className="relative overflow-hidden border border-rose-200 dark:border-rose-900/50 bg-rose-50/50 dark:bg-rose-900/10 shadow-sm hover:shadow-md transition-all transform hover:-translate-y-1 group">
           <div className="p-6">
             <h3 className="text-rose-600 dark:text-rose-400 font-semibold text-sm tracking-wide uppercase flex items-center gap-2">
                Outstanding Debt
                <AlertCircle className="w-4 h-4 opacity-80" />
             </h3>
             <p className="text-rose-700 dark:text-rose-300 text-4xl font-extrabold mt-2 tracking-tight drop-shadow-sm">${stats.debt}</p>
           </div>
        </Card>
      </div>

      {/* Monthly Breakdown */}
      <Card padding="lg" className="space-y-6 border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 shadow-sm">
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-slate-100 dark:border-slate-700 pb-5">
          <div className="flex items-center gap-4">
             <div className="p-3 bg-indigo-50 dark:bg-indigo-500/10 text-indigo-600 dark:text-indigo-400 rounded-xl shadow-sm">
                <BarChart3 className="w-6 h-6" />
             </div>
             <div>
               <h3 className="text-lg font-bold text-slate-900 dark:text-white tracking-tight">Monthly Breakdown</h3>
               <p className="text-sm text-slate-500 dark:text-slate-400 flex items-center gap-2 mt-1">
                  <span className="font-semibold text-emerald-600 dark:text-emerald-400 bg-emerald-50 dark:bg-emerald-500/10 px-2 py-0.5 rounded-md">
                    Total: ${monthlyRevenue?.yearTotal || 0}
                  </span> 
                  in {selectedYear}
               </p>
             </div>
          </div>
          <div className="flex items-center gap-3 bg-slate-50 dark:bg-slate-900/50 p-1.5 rounded-xl border border-slate-200 dark:border-slate-700">
             <span className="text-sm font-medium text-slate-500 dark:text-slate-400 pl-3">Year:</span>
             <select 
              className="px-4 py-2 border-none rounded-lg text-sm bg-white dark:bg-slate-800 text-slate-900 dark:text-white shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-500/20 transition-all font-bold cursor-pointer"
              value={selectedYear}
              onChange={e => setSelectedYear(e.target.value)}
            >
              <option value="2026">2026</option>
              <option value="2025">2025</option>
            </select>
          </div>
        </div>

        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
          {months.map(m => {
             const monthLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
             const mIndex = Number(m.split('-')[0]);
             const mLabel = monthLabels[mIndex - 1];
             const val = monthlyRevenue?.months[mIndex] || 0;
             const isPositive = val > 0;
             
             return (
              <div key={m} className={`relative p-5 border rounded-2xl transition-all duration-300 group
                ${isPositive 
                  ? 'bg-gradient-to-b from-white to-emerald-50/30 dark:from-slate-800 dark:to-emerald-900/10 border-emerald-100 dark:border-emerald-800/30 hover:border-emerald-300 dark:hover:border-emerald-700/50 hover:shadow-md hover:shadow-emerald-500/5' 
                  : 'bg-slate-50 dark:bg-slate-800/50 border-slate-100 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600 hover:bg-white dark:hover:bg-slate-700/50 hover:shadow-sm'}
              `}>
                <span className={`block text-[11px] font-bold uppercase tracking-wider mb-2 transition-colors
                  ${isPositive ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-400 dark:text-slate-500 group-hover:text-slate-600 dark:group-hover:text-slate-300'}
                `}>
                   {mLabel}
                </span>
                <span className={`text-2xl font-black tracking-tight inline-block transition-transform duration-300 group-hover:-translate-y-0.5
                  ${isPositive ? 'text-slate-900 dark:text-white' : 'text-slate-700 dark:text-slate-300'}
                `}>
                  ${val}
                </span>
              </div>
             )
          })}
        </div>
      </Card>

      {/* Period Analysis */}
      <Card padding="lg" className="space-y-6 border border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-800 shadow-sm relative overflow-hidden">
        {/* Subtle decorative background gradient */}
        <div className="absolute inset-0 bg-gradient-to-br from-transparent via-transparent to-violet-50/30 dark:to-violet-900/10 pointer-events-none" />

        <div className="flex items-center gap-4 border-b border-slate-100 dark:border-slate-700 pb-5 relative z-10">
           <div className="p-3 bg-violet-50 dark:bg-violet-500/10 text-violet-600 dark:text-violet-400 rounded-xl shadow-sm">
              <Calendar className="w-6 h-6" />
           </div>
           <div>
             <h3 className="text-lg font-bold text-slate-900 dark:text-white tracking-tight">Custom Period Analysis</h3>
             <p className="text-sm text-slate-500 dark:text-slate-400 mt-1">Filter revenue by specific dates and demographics</p>
           </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-5 items-end bg-slate-50/50 dark:bg-slate-800/30 p-5 rounded-2xl border border-slate-100 dark:border-slate-700/50 relative z-10">
          <Input
             type="date"
             label="Start Date"
             value={dateRange.start}
             onChange={e => setDateRange({ ...dateRange, start: e.target.value })}
             className="mb-0 bg-white dark:bg-slate-800 h-11 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white shadow-sm"
          />
          <Input
             type="date"
             label="End Date"
             value={dateRange.end}
             onChange={e => setDateRange({ ...dateRange, end: e.target.value })}
             className="mb-0 bg-white dark:bg-slate-800 h-11 border-slate-200 dark:border-slate-700 text-slate-900 dark:text-white shadow-sm"
          />
          <Button
             onClick={fetchRevenueByPeriod}
             disabled={dateRange.start === "" || dateRange.end === ""}
             className="w-full h-11 bg-gradient-to-r from-indigo-500 to-violet-600 hover:from-indigo-600 hover:to-violet-700 text-white shadow-md hover:shadow-lg transition-all duration-300 border-none font-bold rounded-xl disabled:opacity-60 disabled:cursor-not-allowed group"
          >
            <Filter className="w-4 h-4 mr-2 group-hover:scale-110 transition-transform" />
            Generate Report
          </Button>
        </div>
        
        <div className="pt-4 relative z-10">
          {revenueByPeriod === null ? (
            <div className="py-20 border-2 border-dashed border-slate-200 dark:border-slate-700 bg-slate-50/50 dark:bg-slate-800/20 rounded-2xl flex flex-col items-center justify-center text-slate-400 dark:text-slate-500 transition-colors shadow-inner">
               <div className="w-20 h-20 bg-white dark:bg-slate-800 shadow-sm border border-slate-100 dark:border-slate-700 rounded-full flex items-center justify-center mb-5 hover:scale-105 transition-transform duration-300">
                 <TrendingUp className="w-10 h-10 text-slate-300 dark:text-slate-600" />
               </div>
               <h3 className="text-slate-900 dark:text-white text-lg font-bold mb-1.5">No Data Selected</h3>
               <p className="text-sm font-medium">Select a period and click Generate Report above</p>
            </div>
          ) : (
            <div className="space-y-6 animate-fade-in-up">
              <div className="flex flex-col sm:flex-row gap-8 p-8 bg-white dark:bg-slate-800 rounded-2xl border border-slate-200 dark:border-slate-700 shadow-md relative overflow-hidden">
                 {/* Premium subtle glow effect in the background */}
                 <div className="absolute top-0 right-0 -mr-12 -mt-12 w-48 h-48 bg-emerald-500/10 rounded-full blur-3xl pointer-events-none" />
                 
                 <div className="flex flex-col flex-1 relative z-10">
                   <div className="flex items-center gap-2.5 mb-3">
                     <span className="w-2.5 h-2.5 rounded-full bg-emerald-500 shadow-[0_0_12px_rgba(16,185,129,0.8)]" />
                     <span className="text-xs font-bold text-slate-500 dark:text-slate-400 uppercase tracking-widest">Total Revenue</span>
                   </div>
                   <span className="text-4xl font-black text-slate-900 dark:text-white tracking-tight drop-shadow-sm">${revenueByPeriod?.periodTotal || 0}</span>
                 </div>
                 
                 <div className="w-px bg-slate-200 dark:bg-slate-700 hidden sm:block relative z-10 self-stretch my-2"></div>
                 
                 <div className="flex flex-col flex-1 relative z-10">
                   <div className="flex items-center gap-2.5 mb-3">
                     <span className="w-2.5 h-2.5 rounded-full bg-rose-500 shadow-[0_0_12px_rgba(244,63,94,0.8)]" />
                     <span className="text-xs font-bold text-slate-500 dark:text-slate-400 uppercase tracking-widest">Period Debt</span>
                   </div>
                   <span className="text-4xl font-black text-slate-900 dark:text-white tracking-tight drop-shadow-sm">${revenueByPeriod?.periodDebt || 0}</span>
                 </div>
              </div>
              
              <div className="border border-slate-200 dark:border-slate-700 rounded-2xl overflow-hidden shadow-sm bg-white dark:bg-slate-800">
                <Table data={revenueByPeriod?.revenues || []} columns={columns} keyExtractor={(row) => row.id} />
              </div>
            </div>
          )}
        </div>
      </Card>
    </div>
  );
};

export default Revenue;
