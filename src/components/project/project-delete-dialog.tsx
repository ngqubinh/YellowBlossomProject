"use client";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface DeleteProjectDialogProps {
  projectName: string;
  onDelete: () => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function DeleteProjectDialog({ 
  projectName,
  onDelete,
  loading,
  onOpenChange,
}: DeleteProjectDialogProps) {
  
  const handleDelete = async () => {
    try {
      await onDelete();
      onOpenChange(false); // Đóng dialog khi thành công
    } catch (error) {
      console.error("Lỗi khi xóa dự án:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px]">
        <DialogHeader>
          <DialogTitle>Xóa Dự Án</DialogTitle>
          <DialogDescription>
            Bạn có chắc chắn muốn xóa dự án "{projectName}"? 
            <span className="text-red-500"> Thao tác này không thể hoàn tác.</span>
          </DialogDescription>
        </DialogHeader>
        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            Hủy
          </Button>
          <Button
            type="button"
            variant="destructive"
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