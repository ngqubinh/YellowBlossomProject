// components/products-table.tsx
"use client";

import { useState } from "react";
import {
  ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
  getSortedRowModel,
  SortingState,
  getFilteredRowModel,
} from "@tanstack/react-table";
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
import { Product } from "@/types/product";
import { productService } from "@/services/productService";
import { useRouter } from "next/navigation";

interface ProductsTableProps {
  data: Product[];
  onEdit?: (product: Product) => void;
  onDelete?: (product: Product) => void;
}

export function ProductsTable({ data, onEdit,onDelete }: ProductsTableProps) {
  const [sorting, setSorting] = useState<SortingState>([]);
  const [globalFilter, setGlobalFilter] = useState("");
  const router = useRouter();

  const columns: ColumnDef<Product>[] = [
    {
      accessorKey: "productName",
      header: "Tên sản phẩm",
      cell: ({ row }) => <div>{String(row.getValue("productName"))}</div>,
    },
    // {
    //   accessorKey: "description",
    //   header: "Mô tả",
    //   cell: ({ row }) => <div>{row.getValue("description")}</div>,
    // },
    // {
    //   // Cột hiển thị Fullname của user (nếu có)
    //   id: "userFullname",
    //   header: "Người tạo",
    //   cell: ({ row }) => (
    //     <div>{row.original.user?.fullName || "N/A"}</div>
    //   ),
    // },
    {
      accessorKey: "version",
      header: "Phiên bản",
      cell: ({ row }) => <div>{row.getValue("version")}</div>,
    },
    {
      accessorKey: "createdAt",
      header: "Ngày tạo",
      cell: ({ row }) => (
        <div>{new Date(row.getValue("createdAt")).toLocaleString()}</div>
      ),
    },
    {
      accessorKey: "lastUpdated",
      header: "Ngày cập nhật",
      cell: ({ row }) => (
        <div>{new Date(row.getValue("lastUpdated")).toLocaleString()}</div>
      ),
    },
    {
      id: "actions",
      header: "Công cụ",
      cell: ({ row }) => (
        <div className="flex space-x-2">
          <Button  variant="secondary"
            size="sm"
            onClick={() => router.push(`/project?productId=${row.original.productId}`)}>
            Dự án
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() => onEdit?.(row.original)}
          >
            Chỉnh sửa
          </Button>
          <Button
            variant="destructive"
            size="sm"
            onClick={() => onDelete?.(row.original)}
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
        placeholder="Tìm kiếm sản phẩm..."
        value={globalFilter ?? ""}
        onChange={(event) => setGlobalFilter(event.target.value)}
        className="max-w-sm"
      />
      <div className="rounded-md border overflow-x-auto">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    onClick={header.column.getToggleSortingHandler()}
                    className="cursor-pointer"
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
                <TableRow key={row.id}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  Không có sản phẩm.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}