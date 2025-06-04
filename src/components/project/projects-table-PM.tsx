"use client";

import { Project } from "@/types/project";
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
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";

interface ProjectTableProps {
  data: Project[];
  onOpenDetails: (project: Project) => void;
  onEdit?: (project: Project) => void;
  onEditStatus?: (project: Project) => void;
  onAssignTeam?: (project: Project) => void;
}

export function RelatedProjectsTable({
  data,
  onOpenDetails,
  onEditStatus,
  onAssignTeam
}: ProjectTableProps) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const router = useRouter();

  console.log("Received data in ProjectsTable:", data);

  const columns: ColumnDef<Project>[] = [
    // {
    //   accessorKey: "projectId",
    //   header: "Mã",
    //   cell: ({ row }) => <div>{String(row.getValue("projectId"))}</div>,
    // },
    {
      accessorKey: "projectName",
      header: "Tên dự án",
      cell: ({ row }) => <div>{String(row.getValue("projectName"))}</div>,
    },
    {
      id: "projectStatusDTO",
      header: "Tiến độ",
      cell: ({ row }) => (
        <div>
          {row.original.projectStatusDTO?.projectStatusName || "Không xác định"}
        </div>
      ),
    },
    {
      id: "projectTeam",
      header: "Nhóm/Team",
      cell: ({ row }) => (
        <div>
          {row.original.projectTeam && row.original.projectTeam.length > 0
            ? row.original.projectTeam.map((team) => (
                <p key={team.teamId}>{team.roleOfTeam}</p>
              ))
            : "Chưa có"}
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
            onClick={() => router.push(`/task?productId=${row.original.projectId}`)}
          >
            Task
          </Button>
          <Button
            variant="secondary"
            size="sm"
            onClick={() => onOpenDetails(row.original)}
          >
            Chi tiết
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onEditStatus?.(row.original)}
          >
            Cập nhật tiến độ
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onAssignTeam?.(row.original)}
          >
            Gán nhóm/team
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
        placeholder="Tìm kiếm dự án..."
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
                          header.getContext(),
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
                  Không có dự án.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}