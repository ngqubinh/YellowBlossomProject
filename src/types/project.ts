export interface Project {
    projectId: string;
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
    productManager: string;
    projectStatusDTO?: ProjectStatusDTO | null;
    projectTeam?: ProjectTeam[] | null;
}

export interface ProjectTeam {
    teamId: string;
    roleOfTeam: string;
}

export interface ProjectStatusDTO {
    projectStatusId: string;
    projectStatusName: string;
}

export interface ProductUser {
  fullName: string;
}

export interface EditProjectRequest {
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
}

export interface CreateProjectRequest {
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
}

export interface ProjectTeamDTO {
    projectId: string;
    teamId: string;
    roleOfTeam: string;
    assignedDate: string;
}

export interface AssignTeamToProject {
    teamId: string;
    roleOfTeam: string;
}

export interface ProjectService {
    getAllProjectStatuses: () => Promise<ProjectStatusDTO[]>;
    getAllProjects: (productId: string) => Promise<Project[]>;
    getAllRelatedProjectsForPM: () => Promise<Project[]>;
    createProject: (productId: string, model: CreateProjectRequest) => Promise<Project>;
    editProject: (projectId: string, model: EditProjectRequest) => Promise<Project>;
    updateProjectStatus: (projectId: string, projectStatusId: string) => Promise<Project>;
    deleteProject: (projectId: string) => Promise<boolean>;
    assignTeamToProject: (projectId: string, model: AssignTeamToProject) => Promise<ProjectTeamDTO>;
}