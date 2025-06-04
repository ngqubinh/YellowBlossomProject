"use client";

import React, { useEffect, useState } from "react";
import { taskService } from "@/services/taskService";
import { Task, CreateTaskRequest } from "@/types/task";
import { toast } from "sonner";
import { useRouter, useSearchParams } from "next/navigation";
import { TasksTable } from "@/components/task/task-table";
import { TaskAddDialog } from "@/components/task/task-add-dialog";

export default function TasksPage() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [isAddDialogOpen, setIsAddDialogOpen] = useState<boolean>(false);
  const searchParams = useSearchParams();
  const projectId = searchParams.get("productId");
  const router = useRouter();

  useEffect(() => {
    if (projectId) {
      fetchTasks(projectId);
    }
  }, [projectId]);

  const fetchTasks = async (projectId: string) => {
    setLoading(true);
    try {
      const fetchedTasks = await taskService.getAllTasks(projectId);
      setTasks(fetchedTasks);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} task:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  const handleOpenDetails = (taskId: string) => {
    router.push(`/task-details?taskId=${taskId}`);
  };

  const handleAddTask = async (formData: CreateTaskRequest) => {
    if (!projectId) return;

    setLoading(true);
    try {
      await taskService.createTask(projectId, formData);
      await fetchTasks(projectId);
      toast.success("Thêm task thành công!");
      setIsAddDialogOpen(false); // Đóng dialog sau khi thêm thành công
    } catch (err: any) {
      handleError(err, "add");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách công việc</h1>

      {error && <p className="text-red-600">Lỗi: {error}</p>}
      {loading && !tasks.length && <div>Đang tải công việc...</div>}

      {/* Nút và Dialog để thêm task */}
      <div className="mb-4">
        <TaskAddDialog
          onSubmit={handleAddTask}
          loading={loading}
          projectId={projectId || ""}
        />
      </div>

      <TasksTable data={tasks} onOpenDetails={handleOpenDetails} />
    </div>
  );
}