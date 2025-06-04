"use client";

import React, { useEffect, useState } from "react";
import { ProjectsTable } from "@/components/project/projects-table";
import { projectService } from "@/services/projectService";
import { Project } from "@/types/project";
import { toast } from "sonner";
import { useSearchParams } from "next/navigation";
import { ProjectDetailsDialog } from "@/components/project/project-details-dialog";
import { EditProjectStatusDialog } from "@/components/project/projectStatus-edit-dialog";
import { EditProjectDialog } from "@/components/project/project-edit-dialog";
import { AddProjectDialog } from "@/components/project/project-add-dialog";
import { DeleteProjectDialog } from "@/components/project/project-delete-dialog";

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [editProject, setEditProject] = useState<Project | null>(null);
  const [statusProject, setStatusProject] = useState<Project | null>(null);
  const [dialogOpen, setDialogOpen] = useState<boolean>(false);
  const [editDialogOpen, setEditDialogOpen] = useState<boolean>(false);
  const [statusDialogOpen, setStatusDialogOpen] = useState<boolean>(false);
  const searchParams = useSearchParams();
  const productId = searchParams.get("productId");
  const [deletingProject, setDeletingProject] = useState<Project | null>(null);

  useEffect(() => {
    if (productId) {
      fetchProjects(productId);
    }
  }, [productId]);

  const fetchProjects = async (productId: string) => {
    setLoading(true);
    try {
      const fetchedProjects = await projectService.getAllProjects(productId);
      setProjects(fetchedProjects);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} project:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  const handleOpenDetails = (project: Project) => {
    setSelectedProject(project);
    setDialogOpen(true);
  };

  const handleOpenEdit = (project: Project) => {
    setEditProject(project);
    setEditDialogOpen(true);
  };

  const handleOpenStatusEdit = (project: Project) => {
    setStatusProject(project);
    setStatusDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setSelectedProject(null);
  };

  const handleCloseEditDialog = () => {
    setEditDialogOpen(false);
    setEditProject(null);
  };

  const handleCloseStatusDialog = () => {
    setStatusDialogOpen(false);
    setStatusProject(null);
  };

  const handleEditSubmit = async (formData: {
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
  }) => {
    if (!editProject || !productId) return;
    setLoading(true);
    try {
      await projectService.editProject(editProject.projectId, formData);
      toast.success("Cập nhật dự án thành công");
      await fetchProjects(productId);
    } catch (err: any) {
      handleError(err, "update");
    } finally {
      setLoading(false);
    }
  };

  const handleStatusSubmit = async (newStatusId: string) => {
    if (!statusProject || !productId) return;
    setLoading(true);
    try {
      await projectService.updateProjectStatus(statusProject.projectId, newStatusId);
      toast.success("Chỉnh sửa trạng thái dự án thành công");
      await fetchProjects(productId);
    } catch (err: any) {
      handleError(err, "chỉnh sửa trạng thái");
    } finally {
      setLoading(false);
    }
  };

  const handleAddProject = async (formData: {
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
  }) => {
    if (!productId) return;

    setLoading(true);
    try {
      await projectService.createProject(productId, formData);
      await fetchProjects(productId);
      toast.success("Thêm dự án thành công!");
    } catch (err: any) {
      handleError(err, "add");
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteProject = async () => {
    if (!deletingProject || !productId) return;
    
    setLoading(true);
    try {
      await projectService.deleteProject(deletingProject.projectId);
      await fetchProjects(productId);
      toast.success("Xóa dự án thành công!");
      setDeletingProject(null);
    } catch (err: any) {
      handleError(err, "delete");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách dự án</h1>

      {error && <p className="text-red-600">Lỗi: {error}</p>}
      {loading && !projects.length && <div>Đang tải dự án...</div>}

      {/* Hiển thị nút Thêm Dự Án */}
      <div className="mb-4">
        <AddProjectDialog onSubmit={handleAddProject} loading={loading} />
      </div>

      <ProjectsTable
        data={projects}
        onOpenDetails={handleOpenDetails}
        onEdit={handleOpenEdit}
        onEditStatus={handleOpenStatusEdit}
        onDelete={(project) => setDeletingProject(project)}
      />

      {selectedProject && (
        <ProjectDetailsDialog
          project={selectedProject}
          onOpenChange={handleCloseDialog}
        />
      )}

      {editProject && (
        <EditProjectDialog
          project={editProject}
          onSubmit={handleEditSubmit}
          loading={loading}
          onOpenChange={handleCloseEditDialog}
        />
      )}

      {statusProject && (
        <EditProjectStatusDialog
          project={statusProject}
          onSubmit={handleStatusSubmit}
          loading={loading}
          onOpenChange={handleCloseStatusDialog}
        />
      )}

      {deletingProject && (
        <DeleteProjectDialog
          projectName={deletingProject.projectName}
          onDelete={handleDeleteProject}
          loading={loading}
          onOpenChange={(open) => !open && setDeletingProject(null)}
        />
      )}
    </div>
  );
}
