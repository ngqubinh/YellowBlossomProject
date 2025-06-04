"use client";

import React, { useEffect, useState } from "react";
import { taskService } from "@/services/taskService";
import { EditTaskRequest, Task, AssignTeamToTaskRequest } from "@/types/task";
import { useSearchParams, useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import { TaskDetailsCard } from "@/components/task/task-detail-team";
import { TaskStatusUpdateDialog } from "@/components/task/task-updateStatus-dialog";

export default function TaskDetailsPage() {
  const [task, setTask] = useState<Task | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isStatusDialogOpen, setIsStatusDialogOpen] = useState<boolean>(false);
  const searchParams = useSearchParams();
  const taskId = searchParams.get("taskId");
  const router = useRouter();

  useEffect(() => {
    if (!taskId) {
      setError("Không tìm thấy task");
      setLoading(false);
      return;
    }
    fetchTaskDetails(taskId);
  }, [taskId]);

  const fetchTaskDetails = async (taskId: string) => {
    try {
      setLoading(true);
      const taskData = await taskService.getTaskDetails(taskId);
      setTask(taskData);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} task details:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  const handleUpdateStatus = async (statusId: string) => {
    if (!taskId) return;

    try {
      await taskService.updateTaskStatus(taskId, { taskStatusId: statusId });
      await fetchTaskDetails(taskId);
      toast.success("Cập nhật trạng thái thành công!");
    } catch (err: any) {
      handleError(err, "update status");
    }
  };

  if (loading) return <div className="text-center py-8">Đang tải chi tiết task...</div>;

  return (
    <div className="container mx-auto py-8">
      <div className="mb-6">
        <Button variant="outline" onClick={() => router.back()}>
          ← Quay lại
        </Button>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {task && (
        <>
          <TaskDetailsCard
            task={task}
            onUpdateStatus={() => setIsStatusDialogOpen(true)}
          />

          <TaskStatusUpdateDialog
            taskId={task.taskId}
            currentStatus={{
              taskStatusId: task.taskStatus?.taskStatusId || "",
              taskStatusName: task.taskStatus?.taskStatusName || ""
            }}
            onSubmit={handleUpdateStatus}
            onOpenChange={setIsStatusDialogOpen}
            open={isStatusDialogOpen}
          />
        </>
      )}
    </div>
  );
}