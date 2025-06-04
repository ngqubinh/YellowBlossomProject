"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Project, ProjectStatusDTO } from "@/types/project";
import { projectService } from "@/services/projectService";

interface EditProjectStatusDialogProps {
  project: Project;
  onSubmit: (newStatusId: string) => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function EditProjectStatusDialog({
  project,
  onSubmit,
  loading,
  onOpenChange,
}: EditProjectStatusDialogProps) {
  const [statusOptions, setStatusOptions] = useState<ProjectStatusDTO[]>([]);
  const [selectedStatusId, setSelectedStatusId] = useState<string>(
    project.projectStatusDTO?.projectStatusId || "",
  );
  const [fetchError, setFetchError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchStatuses() {
      try {
        const statuses = await projectService.getAllProjectStatuses();
        setStatusOptions(statuses);
        if (project.projectStatusDTO?.projectStatusId) {
          setSelectedStatusId(project.projectStatusDTO.projectStatusId);
        } else if (statuses.length > 0) {
          setSelectedStatusId(statuses[0].projectStatusId);
        }
      } catch (error) {
        console.error("Failed to fetch project statuses:", error);
        setFetchError("Không thể tải danh sách trạng thái");
      }
    }
    fetchStatuses();
  }, [project.projectStatusDTO?.projectStatusId]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!selectedStatusId) return;
    try {
      await onSubmit(selectedStatusId);
      onOpenChange(false);
    } catch (error) {
      console.error("Error editing project status:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px] bg-gray-800">
        <DialogHeader>
          <DialogTitle className="text-white">Chỉnh sửa Trạng thái Dự án</DialogTitle>
          <DialogDescription className="text-gray-300">
            Chọn trạng thái mới cho dự án. Nhấn "Lưu" để cập nhật.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Trạng thái hiện tại
            </label>
            <p className="text-sm text-white">
              {project.projectStatusDTO?.projectStatusName || "Không xác định"}
            </p>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Trạng thái mới
            </label>
            {fetchError ? (
              <p className="text-sm text-red-400">{fetchError}</p>
            ) : (
              <Select
                value={selectedStatusId}
                onValueChange={setSelectedStatusId}
                disabled={loading || statusOptions.length === 0}
              >
                <SelectTrigger className="text-white border-gray-600 bg-gray-700">
                  <SelectValue
                    placeholder={
                      statusOptions.length > 0
                        ? "Chọn trạng thái"
                        : "Đang tải trạng thái..."
                    }
                  />
                </SelectTrigger>
                <SelectContent className="bg-gray-700 text-white border-gray-600">
                  {statusOptions.map((status) => (
                    <SelectItem
                      key={status.projectStatusId}
                      value={status.projectStatusId}
                      className="text-white"
                    >
                      {status.projectStatusName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
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
            <Button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700 text-white"
              disabled={loading || !selectedStatusId || fetchError !== null}
            >
              {loading ? "Đang xử lý..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}