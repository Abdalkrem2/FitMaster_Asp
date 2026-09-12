import React from "react";
import { Users } from "lucide-react";

export interface Column<T> {
  key: keyof T | string;
  header: string;
  render?: (row: T) => React.ReactNode;
}

interface TableProps<T> {
  data: T[];
  columns: Column<T>[];
  keyExtractor: (row: T) => string | number;
}

export function Table<T>({ data, columns, keyExtractor }: TableProps<T>) {
  return (
    <div className="overflow-x-auto rounded-2xl border border-slate-200/60 shadow-soft bg-white w-full">
      <table className="min-w-full divide-y divide-slate-100">
        <thead>
          <tr className="bg-slate-50/80">
            {columns.map((col, index) => (
              <th
                key={String(col.key) + index}
                scope="col"
                className="px-5 py-3.5 text-left text-[11px] font-semibold text-slate-400 uppercase tracking-wider"
              >
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 bg-white">
          {data.length === 0 ? (
            <tr>
              <td colSpan={columns.length} className="px-6 py-16 text-center">
                <div className="flex flex-col items-center gap-2">
                  <div className="w-12 h-12 rounded-xl bg-slate-100 flex items-center justify-center">
                    <Users className="w-5 h-5 text-slate-300" />
                  </div>
                  <p className="text-sm font-medium text-slate-500">
                    No data available
                  </p>
                  <p className="text-xs text-slate-400">
                    Items will appear here once added
                  </p>
                </div>
              </td>
            </tr>
          ) : (
            data.map((row) => (
              <tr
                key={keyExtractor(row)}
                className="group hover:bg-slate-50/50 transition-colors duration-150"
              >
                {columns.map((col, index) => (
                  <td
                    key={String(col.key) + index}
                    className="px-5 py-4 whitespace-nowrap text-[13px] text-slate-600"
                  >
                    {col.render
                      ? col.render(row)
                      : String(row[col.key as keyof T])}
                  </td>
                ))}
              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}
