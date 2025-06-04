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
import { Team } from "@/types/user";
import { Button } from "@/components/ui/button";

interface TeamsTableProps {
  data: Team[];
  // onEdit: (team: Team) => void; 
  // onDelete: (teamId: string) => void; 
  onView: (teamId: string) => void; 
}

export function TeamsTable({ data, onView }: TeamsTableProps) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = useState("");

  const columns: ColumnDef<Team>[] = [
    {
      accessorKey: "teamId",
      header: "Mã team",
      cell: ({ row }) => <div>{String(row.getValue("teamId"))}</div>,
    },
    {
      accessorKey: "teamName",
      header: "Tên team",
      cell: ({ row }) => <div>{String(row.getValue("teamName"))}</div>,
    },
     {
      accessorKey: "createdDate",
      header: "Ngày tạo",
      cell: ({ row }) => <div>{String(row.getValue("createdDate"))}</div>,
    },
    {
      id: "actions",
      header: "Công cụ",
      cell: ({ row }) => (
        <div className="flex space-x-2">
           <Button 
            variant="secondary" 
            size="sm"
            onClick={() => onView(row.original.teamId)} 
          >
            Chi tiết
          </Button>
          <Button
            variant="secondary"
            size="sm"
            // onClick={() => onEdit(row.original)}
          >
            Chỉnh sửa
          </Button>
          <Button
            variant="destructive"
            size="sm"
            // onClick={() => onDelete(row.original.teamId)}
          >
            Xóa
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
        placeholder="Tìm kiếm team..."
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
                  Không có nhóm(team).
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}