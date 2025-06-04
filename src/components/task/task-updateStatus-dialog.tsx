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
import { taskService } from "@/services/taskService";
import { TaskStatus } from "@/types/task";

interface TaskStatusUpdateDialogProps {
  taskId: string;
  currentStatus: {
    taskStatusId: string;
    taskStatusName: string;
  };
  onSubmit: (statusId: string) => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
  open: boolean;
}

export function TaskStatusUpdateDialog({
  taskId,
  currentStatus,
  onSubmit,
  loading,
  onOpenChange,
  open, // Use the open prop from props
}: TaskStatusUpdateDialogProps) {
  const [statusOptions, setStatusOptions] = useState<TaskStatus[]>([]);
  const [selectedStatusId, setSelectedStatusId] = useState<string>(
    currentStatus.taskStatusId
  );
  const [fetchError, setFetchError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchStatuses() {
      try {
        const statuses = await taskService.getAllTaskStatuses();
        setStatusOptions(statuses);
        if (!currentStatus.taskStatusId && statuses.length > 0) {
          setSelectedStatusId(statuses[0].taskStatusId);
        }
      } catch (error) {
        console.error("Failed to fetch task statuses:", error);
        setFetchError("Không thể tải danh sách trạng thái");
      }
    }
    fetchStatuses();
  }, [currentStatus.taskStatusId]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!selectedStatusId) return;
    try {
      await onSubmit(selectedStatusId);
      onOpenChange(false);
    } catch (error) {
      console.error("Error updating task status:", error);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}> {/* Use the open prop here */}
      <DialogContent className="sm:max-w-[500px] bg-gray-800">
        <DialogHeader>
          <DialogTitle className="text-white">Cập nhật Trạng thái Công việc</DialogTitle>
          <DialogDescription className="text-gray-300">
            Chọn trạng thái mới cho công việc. Nhấn "Lưu" để cập nhật.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Trạng thái hiện tại
            </label>
            <p className="text-sm text-white">
              {currentStatus.taskStatusName || "Không xác định"}
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
                      key={status.taskStatusId}
                      value={status.taskStatusId}
                      className="text-white"
                    >
                      {status.taskStatusName}
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