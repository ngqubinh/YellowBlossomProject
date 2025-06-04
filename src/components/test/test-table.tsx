"use client";

import { useRouter } from "next/navigation";
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Task } from "@/types/task";

interface TasksTableProps {
  data: Task[];
}

export function TasksTable({ data }: TasksTableProps) {
  const router = useRouter();

  const columns: ColumnDef<Task>[] = [
    {
      accessorKey: "taskId",
      header: "Mã",
      cell: ({ row }) => (
        <div className="font-mono w-32 truncate">{row.original.taskId.slice(0, 8)}</div>
      ),
    },
    {
      accessorKey: "title",
      header: "Tên công việc",
      cell: ({ row }) => (
        <div className="w-64 truncate">{row.getValue("title")}</div>
      ),
    },
    {
      id: "taskStatus",
      accessorFn: (row) => row.taskStatus?.taskStatusName,
      header: "Tiến độ",
      cell: ({ row }) => (
        <div className="w-32">
          {row.original.taskStatus?.taskStatusName || "N/A"}
        </div>
      ),
    },
    {
      accessorKey: "startDate",
      header: "Bắt đầu",
      cell: ({ row }) => (
        <div className="w-32">
          {new Date(row.original.startDate).toLocaleDateString()}
        </div>
      ),
    },
    {
      accessorKey: "endDate",
      header: "Deadline",
      cell: ({ row }) => (
        <div className="w-32">
          {new Date(row.original.endDate).toLocaleDateString()}
        </div>
      ),
    },
    {
      id: "priority",
      accessorFn: (row) => row.priority?.priorityName,
      header: "Ưu tiên",
      cell: ({ row }) => (
        <div className="w-24">
          {row.original.priority?.priorityName || "N/A"}
        </div>
      ),
    },
    {
      id: "team",
      accessorFn: (row) => row.team?.teamName,
      header: "Nhóm",
      cell: ({ row }) => (
        <div className="w-48 truncate">{row.original.team?.teamName}</div>
      ),
    },
    {
      id: "actions",
      header: "Chi tiết",
      cell: ({ row }) => (
        <button
          onClick={() => router.push(`/tasks/${row.original.taskId}`)}
          className="px-3 py-1.5 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-sm"
        >
          Chi tiết
        </button>
      ),
    },
  ];

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <div className="rounded-lg border border-gray-200 dark:border-gray-700 overflow-x-auto">
      <Table className="min-w-[1200px]">
        <TableHeader className="bg-gray-50 dark:bg-gray-800">
          {table.getHeaderGroups().map((headerGroup) => (
            <TableRow key={headerGroup.id}>
              {headerGroup.headers.map((header) => (
                <TableHead 
                  key={header.id}
                  className="text-gray-700 dark:text-gray-300 px-4 py-3 font-medium text-left"
                  style={{ width: header.getSize() }}
                >
                  {flexRender(
                    header.column.columnDef.header,
                    header.getContext()
                  )}
                </TableHead>
              ))}
            </TableRow>
          ))}
        </TableHeader>
        
        <TableBody>
          {table.getRowModel().rows.map((row) => (
            <TableRow 
              key={row.id}
              className="hover:bg-gray-50 dark:hover:bg-gray-800"
            >
              {row.getVisibleCells().map((cell) => (
                <TableCell
                  key={cell.id}
                  className="text-gray-600 dark:text-gray-400 px-4 py-2.5 text-sm"
                >
                  {flexRender(
                    cell.column.columnDef.cell,
                    cell.getContext()
                  )}
                </TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}