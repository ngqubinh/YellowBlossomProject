"use client";

import React, { useEffect, useState } from "react";
import { taskService } from "@/services/taskService";
import { Task } from "@/types/task";
import { toast } from "sonner";
import { TasksTable } from "@/components/task/task-table-Team";
import { useRouter } from "next/navigation";

export default function TasksPage() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleOpenDetails = (taskId: string) => {
    router.push(`/task-details-team?taskId=${taskId}`);
  };


  const fetchTasks = async () => {
    setLoading(true);
    try {
      const fetchedTasks = await taskService.getCurrentTasks();
      setTasks(fetchedTasks);
      setError(null);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} tasks:`, err);
    const message = err.message || `Lỗi khi tải danh sách công việc`;
    toast.error(message);
    setError(message);
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách công việc của tôi</h1>

      {error && <p className="text-red-600 mb-4">Lỗi: {error}</p>}
      
      <div className="relative">
        {loading && (
          <div className="absolute inset-0 bg-gray-100/50 flex items-center justify-center">
            <p>Đang tải danh sách công việc...</p>
          </div>
        )}

        <TasksTable data={tasks}   onOpenDetails={handleOpenDetails}/>
      </div>
    </div>
  );
}