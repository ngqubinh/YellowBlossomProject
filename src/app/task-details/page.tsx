"use client";

import React, { useEffect, useState } from "react";
import { taskService } from "@/services/taskService";
import { EditTaskRequest, Task, AssignTeamToTaskRequest } from "@/types/task";
import { useSearchParams, useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { TaskDetailsCard } from "@/components/task/task-detail";
import { TaskEditDialog } from "@/components/task/task-edit-dialog";
import { TaskDeleteDialog } from "@/components/task/task-delete-dialog";
import { teamService } from "@/services/teamService";
import { Team } from "@/types/user";
import { toast } from "sonner";
import { AssignTeamToTaskDialog } from "@/components/task/task-addTeam-dialog";

export default function TaskDetailsPage() {
  const [task, setTask] = useState<Task | null>(null);
  const [teams, setTeams] = useState<Team[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState<boolean>(false);
  const [isUpdating, setIsUpdating] = useState<boolean>(false);
  const [isDeleting, setIsDeleting] = useState<boolean>(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState<boolean>(false);
  const [isAssignTeamDialogOpen, setIsAssignTeamDialogOpen] = useState<boolean>(false);
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
    fetchTeams();
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

  const fetchTeams = async () => {
    try {
      const teamsData = await teamService.getAllTeams();
      setTeams(teamsData);
    } catch (err: any) {
      handleError(err, "fetch teams");
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} task details:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  const handleEdit = () => {
    setIsEditing(true);
  };

  const handleCloseEdit = () => {
    setIsEditing(false);
  };

  const handleUpdateTask = async (formData: EditTaskRequest) => {
    if (!taskId) return;

    try {
      setIsUpdating(true);
      await taskService.editTask(taskId, formData);
      await fetchTaskDetails(taskId);
      toast.success("Cập nhật task thành công!");
      setIsEditing(false);
    } catch (err: any) {
      handleError(err, "update");
    } finally {
      setIsUpdating(false);
    }
  };

  const handleOpenDeleteDialog = () => {
    setIsDeleteDialogOpen(true);
  };

  const handleCloseDeleteDialog = () => {
    setIsDeleteDialogOpen(false);
  };

  const handleDelete = async () => {
    if (!taskId) return;

    try {
      setIsDeleting(true);
      await taskService.deleteTask(taskId);
      toast.success("Xóa task thành công");
      router.push("/tasks");
    } catch (err: any) {
      handleError(err, "delete");
    } finally {
      setIsDeleting(false);
    }
  };

  const handleAssignTeam = async (formData: AssignTeamToTaskRequest) => {
    if (!taskId) return;

    try {
      setIsUpdating(true);
      await taskService.assignTeamToTask(taskId, formData);
      await fetchTaskDetails(taskId);
      toast.success("Gán team thành công!");
      setIsAssignTeamDialogOpen(false);
    } catch (err: any) {
      handleError(err, "assign team");
    } finally {
      setIsUpdating(false);
    }
  };

  const handleOpenAssignTeamDialog = async () => {
    try {
      await fetchTeams(); 
      setIsAssignTeamDialogOpen(true);
    } catch (err: any) {
      handleError(err, "fetch teams");
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
            onEdit={handleEdit}
            onDelete={handleOpenDeleteDialog}
            onAssignTeam={handleOpenAssignTeamDialog}
          />
        </>
      )}

      {task && isEditing && (
        <TaskEditDialog
          task={task}
          onSubmit={handleUpdateTask}
          loading={isUpdating}
          onOpenChange={setIsEditing}
        />
      )}

      {task && isDeleteDialogOpen && (
        <TaskDeleteDialog
          title={task.title}
          onDelete={handleDelete}
          loading={isDeleting}
          onOpenChange={setIsDeleteDialogOpen}
        />
      )}

      {task && (
        <AssignTeamToTaskDialog
          taskId={task.taskId}
          onSubmit={handleAssignTeam}
          loading={isUpdating}
          teams={teams}
          open={isAssignTeamDialogOpen}
          onOpenChange={setIsAssignTeamDialogOpen}
        />
      )}
    </div>
  );
}