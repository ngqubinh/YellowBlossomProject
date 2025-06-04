"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Project } from "@/types/project";

interface EditProjectDialogProps {
  project: Project;
  onSubmit: (formData: { projectName: string; description: string; startDate: string; endDate: string }) => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function EditProjectDialog({ project, onSubmit, loading, onOpenChange }: EditProjectDialogProps) {
  const [formData, setFormData] = useState({
    projectName: project.projectName || "",
    description: project.description || "",
    startDate: project.startDate ? new Date(project.startDate).toISOString().split("T")[0] : "",
    endDate: project.endDate ? new Date(project.endDate).toISOString().split("T")[0] : "",
  });

  useEffect(() => {
    setFormData({
      projectName: project.projectName || "",
      description: project.description || "",
      startDate: project.startDate ? new Date(project.startDate).toISOString().split("T")[0] : "",
      endDate: project.endDate ? new Date(project.endDate).toISOString().split("T")[0] : "",
    });
  }, [project]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await onSubmit(formData);
      onOpenChange(false);
    } catch (error) {
      console.error("Error updating project:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px] bg-gray-800">
        <DialogHeader>
          <DialogTitle className="text-white">Chỉnh sửa Dự án</DialogTitle>
          <DialogDescription className="text-gray-300">
            Cập nhật thông tin dự án. Nhấn "Lưu" khi hoàn tất.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Tên dự án</label>
            <Input
              placeholder="Nhập tên dự án"
              value={formData.projectName}
              onChange={(e) => setFormData({ ...formData, projectName: e.target.value })}
              className="text-white border-gray-600 bg-gray-700"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Mô tả</label>
            <Input
              placeholder="Nhập mô tả dự án"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              className="text-white border-gray-600 bg-gray-700"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Ngày bắt đầu</label>
            <Input
              type="date"
              value={formData.startDate}
              onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
              className="text-white border-gray-600 bg-gray-700"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Ngày kết thúc</label>
            <Input
              type="date"
              value={formData.endDate}
              onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
              className="text-white border-gray-600 bg-gray-700"
            />
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              className="text-white border-white hover:bg-gray-700"
              onClick={() => onOpenChange(false)}
              disabled={loading}
            >
              Hủy
            </Button>
            <Button type="submit" className="bg-blue-600 hover:bg-blue-700 text-white" disabled={loading}>
              {loading ? "Đang xử lý..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
