"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface TaskDeleteDialogProps {
  title: string;
  onDelete: () => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function TaskDeleteDialog({
  title,
  onDelete,
  loading,
  onOpenChange,
}: TaskDeleteDialogProps) {
  const handleDelete = async () => {
    try {
      await onDelete();
      onOpenChange(false);
    } catch (error) {
      console.error("Lỗi khi xóa task:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Xóa Task</DialogTitle>
          <DialogDescription className="text-gray-300">
            Bạn có chắc chắn muốn xóa task "{title}"?{" "}
            <span className="text-red-500">Thao tác này không thể hoàn tác.</span>
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            className="text-white border-gray-500 hover:bg-gray-700"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="destructive"
            className="bg-red-600 hover:bg-red-700"
            onClick={handleDelete}
            disabled={loading}
          >
            {loading ? "Đang xóa..." : "Xác nhận xóa"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}