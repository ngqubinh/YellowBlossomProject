"use client";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Project } from "@/types/project";

interface ProjectDetailsDialogProps {
  project: Project;
  onOpenChange: (open: boolean) => void;
}

export function ProjectDetailsDialog({ project, onOpenChange }: ProjectDetailsDialogProps) {
  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Chi tiết Dự án</DialogTitle>
          <DialogDescription>Xem thông tin chi tiết của dự án.</DialogDescription>
        </DialogHeader>
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1">Tên dự án</label>
            <p className="text-sm text-white">{project.projectName || "Chưa có tên dự án"}</p>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Mô tả</label>
            <p className="text-sm text-white">{project.description || "Chưa có mô tả"}</p>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Ngày bắt đầu</label>
            <p className="text-sm text-white">{project.startDate || "Chưa xác định"}</p>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Ngày kết thúc</label>
            <p className="text-sm text-white">{project.endDate || "Chưa xác định"}</p>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Trạng thái dự án</label>
            <p className="text-sm text-white">{project.projectStatusDTO?.projectStatusName || "NotStarted"}</p>
          </div>
        </div>
        <DialogFooter>
          <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
            Đóng
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}