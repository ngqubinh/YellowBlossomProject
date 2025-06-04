"use client";

import React, { useEffect, useState } from "react";
import { projectService } from "@/services/projectService";
import { Project } from "@/types/project";
import { toast } from "sonner";
import { ProjectDetailsDialog } from "@/components/project/project-details-dialog";
import { EditProjectStatusDialog } from "@/components/project/projectStatus-edit-dialog";
import { EditProjectDialog } from "@/components/project/project-edit-dialog";
import { RoleGuard } from "@/context/RoleGuardProps ";
import { RelatedProjectsTable } from "@/components/project/projects-table-PM";
import { useSearchParams } from "next/navigation";
import { AssignTeamDialog } from "@/components/project/project-assignTeam-dialog";

export default function RelatedProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedProject, setSelectedProject] = useState<Project | null>(null);
  const [statusProject, setStatusProject] = useState<Project | null>(null);
  const [teamProject, setTeamProject] = useState<Project | null>(null); // Thêm state cho gán nhóm
  const [dialogOpen, setDialogOpen] = useState<boolean>(false);
  const [statusDialogOpen, setStatusDialogOpen] = useState<boolean>(false);
  const [teamDialogOpen, setTeamDialogOpen] = useState<boolean>(false); // Thêm state dialog gán nhóm

  useEffect(() => {
    fetchProjects();
  }, []);

  const fetchProjects = async () => {
    setLoading(true);
    try {
      console.log("Fetching projects from API...");
      const fetchedProjects = await projectService.getAllRelatedProjectsForPM();
      console.log("Projects fetched:", fetchedProjects);
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

  const handleOpenStatusEdit = (project: Project) => {
    setStatusProject(project);
    setStatusDialogOpen(true);
  };

  // Xử lý mở dialog gán nhóm
  const handleOpenTeamAssignment = (project: Project) => {
    setTeamProject(project);
    setTeamDialogOpen(true);
  };

  // Xử lý đóng dialog gán nhóm
  const handleCloseTeamDialog = () => {
    setTeamDialogOpen(false);
    setTeamProject(null);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setSelectedProject(null);
  };

  const handleCloseStatusDialog = () => {
    setStatusDialogOpen(false);
    setStatusProject(null);
  };

  const handleStatusSubmit = async (newStatusId: string) => {
    if (!statusProject) return;
    setLoading(true);
    try {
      await projectService.updateProjectStatus(
        statusProject.projectId, 
        newStatusId
      );
      toast.success("Chỉnh sửa trạng thái dự án thành công");
      await fetchProjects();
    } catch (err: any) {
      handleError(err, "chỉnh sửa trạng thái");
    } finally {
      setLoading(false);
      handleCloseStatusDialog();
    }
  };

  // Xử lý gán nhóm
  const handleTeamSubmit = async (data: { teamId: string; roleOfTeam: string }) => {
    if (!teamProject) return;
    setLoading(true);
    try {
      await projectService.assignTeamToProject(
        teamProject.projectId,
        data
      );
      toast.success("Gán nhóm cho dự án thành công");
      await fetchProjects();
    } catch (err: any) {
      handleError(err, "gán nhóm");
    } finally {
      setLoading(false);
      handleCloseTeamDialog();
    }
  };

  return (
    <RoleGuard requiredRoles={"ProjectManager"}>
      <div className="container mx-auto py-10">
        <h1 className="text-2xl font-bold mb-6">Danh sách dự án</h1>

        {error && <p className="text-red-600">Lỗi: {error}</p>}
        {loading && !projects.length && <div>Đang tải dự án...</div>}

        <RelatedProjectsTable 
          data={projects} 
          onOpenDetails={handleOpenDetails} 
          onEditStatus={handleOpenStatusEdit}
         onAssignTeam={handleOpenTeamAssignment} 
        />
      </div>

      {selectedProject && (
        <ProjectDetailsDialog
          project={selectedProject}
          onOpenChange={handleCloseDialog}
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

      {teamProject && (
        <AssignTeamDialog
          projectId={teamProject.projectId}
          onSubmit={handleTeamSubmit}
          loading={loading}
          onOpenChange={(open) => {
            if (!open) handleCloseTeamDialog();
          }}
        />
      )}
    </RoleGuard>
  );
}