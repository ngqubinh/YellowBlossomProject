"use client";

import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  SortingState,
  useReactTable,
} from "@tanstack/react-table";
import { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Task } from "@/types/task";
import { Button } from "../ui/button";

interface TasksTableProps {
  data: Task[];
  onOpenDetails: (taskId: string) => void;
}

export function TasksTable({ data, onOpenDetails }: TasksTableProps) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = useState("");

  const columns: ColumnDef<Task>[] = [
    {
      accessorKey: "taskId",
      header: "Mã",
      cell: ({ row }) => <div>{String(row.getValue("taskId"))}</div>,
    },
    {
      accessorKey: "title",
      header: "Tên công việc",
      cell: ({ row }) => <div>{String(row.getValue("title"))}</div>,
    },
    {
      accessorKey: "taskStatus",
      header: "Tiến độ",
      cell: ({ row }) => <div>{row.original.taskStatus?.taskStatusName || "Không có dữ liệu"}</div>,
    },
    {
      accessorKey: "endDate",
      header: "Deadline",
      cell: ({ row }) => (
        <div>
          {row.getValue("endDate")
            ? new Date(row.getValue("endDate")).toLocaleDateString()
            : "Không xác định"}
        </div>
      ),
    },
    {
      id: "actions",
      header: "Công cụ",
      cell: ({ row }) => (
        <div className="flex space-x-2">
          <Button
            variant="secondary"
            size="sm"
            onClick={() => onOpenDetails(row.original.taskId)} // Sửa thành taskId
          >
            Chi tiết
          </Button>
        </div>
      ),
    },
  ];

  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    state: {
      sorting,
      globalFilter,
    },
  });

  return (
    <div className="space-y-4">
      <Input
        placeholder="Tìm kiếm công việc..."
        value={globalFilter ?? ""}
        onChange={(event) => setGlobalFilter(event.target.value)}
        className="max-w-sm text-white bg-gray-700 border-gray-600"
      />
      <div className="rounded-md border border-gray-600 bg-gray-800">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id} className="bg-gray-700">
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    onClick={header.column.getToggleSortingHandler()}
                    className="cursor-pointer text-white"
                  >
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                    {{
                      asc: " 🔼",
                      desc: " 🔽",
                    }[header.column.getIsSorted() as string] ?? null}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow key={row.id} className="hover:bg-gray-700">
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id} className="text-white">
                      {flexRender(cell.column.columnDef.cell, cell.getContext())}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center text-gray-300"
                >
                  Không có công việc.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}