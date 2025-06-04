"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
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
import { Task, Priority, EditTaskRequest } from "@/types/task";
import { taskService } from "@/services/taskService";

interface TaskEditDialogProps {
  task: Task;
  onSubmit: (formData: EditTaskRequest) => Promise<void>;
  loading?: boolean;
  onOpenChange: (open: boolean) => void;
}

export function TaskEditDialog({ task, onSubmit, loading, onOpenChange }: TaskEditDialogProps) {
  const [priorities, setPriorities] = useState<Priority[]>([]);
  const [fetchError, setFetchError] = useState<string | null>(null);
  const [formData, setFormData] = useState<EditTaskRequest>({
    title: task.title,
    description: task.description,
    startDate: task.startDate.split("T")[0],
    endDate: task.endDate?.split("T")[0] || "",
    priorityId: task.priority?.priorityId || "",
  });

  // Fetch priorities
  useEffect(() => {
    async function fetchPriorities() {
      try {
        const prioritiesData = await taskService.getAllPriorities();
        setPriorities(prioritiesData);
        // Đặt priorityId mặc định nếu task có priority, nếu không chọn priority đầu tiên
        if (task.priority?.priorityId) {
          setFormData((prev) => ({ ...prev, priorityId: task.priority!.priorityId }));
        } else if (prioritiesData.length > 0) {
          setFormData((prev) => ({ ...prev, priorityId: prioritiesData[0].priorityId }));
        }
      } catch (error) {
        console.error("Failed to fetch priorities:", error);
        setFetchError("Không thể tải danh sách độ ưu tiên");
      }
    }
    fetchPriorities();
  }, [task.priority?.priorityId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!formData.priorityId) return;
    try {
      await onSubmit({
        ...formData,
        startDate: new Date(formData.startDate).toISOString(),
        endDate: formData.endDate ? new Date(formData.endDate).toISOString() : "",
      });
      onOpenChange(false);
    } catch (error) {
      console.error("Error updating task:", error);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Chỉnh sửa Task</DialogTitle>
          <DialogDescription className="text-gray-300">
            Cập nhật thông tin task của bạn
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-2">Tiêu đề</label>
            <Input
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Mô tả</label>
            <Input
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-2">Độ ưu tiên</label>
            {fetchError ? (
              <p className="text-sm text-red-400">{fetchError}</p>
            ) : (
              <Select
                value={formData.priorityId}
                onValueChange={(value) => setFormData({ ...formData, priorityId: value })}
                disabled={loading || priorities.length === 0}
              >
                <SelectTrigger className="bg-gray-700 border-gray-600 text-white">
                  <SelectValue
                    placeholder={
                      priorities.length > 0 ? "Chọn độ ưu tiên" : "Đang tải độ ưu tiên..."
                    }
                  />
                </SelectTrigger>
                <SelectContent className="bg-gray-700 text-white border-gray-600">
                  {priorities.map((priority) => (
                    <SelectItem
                      key={priority.priorityId}
                      value={priority.priorityId}
                      className="text-white"
                    >
                      {priority.priorityName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-2">Ngày bắt đầu</label>
              <Input
                type="date"
                value={formData.startDate}
                onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                className="bg-gray-700 border-gray-600 text-white"
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-2">Ngày kết thúc</label>
              <Input
                type="date"
                value={formData.endDate}
                onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                className="bg-gray-700 border-gray-600 text-white"
              />
            </div>
          </div>

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
              type="submit"
              className="bg-blue-600 hover:bg-blue-700"
              disabled={loading || !formData.priorityId || fetchError !== null}
            >
              {loading ? "Đang lưu..." : "Lưu thay đổi"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}