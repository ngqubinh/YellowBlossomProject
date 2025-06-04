export interface Task {
    taskId: string;
    title: string;
    description: string;
    startDate: string;
    endDate: string;
    projectId: string;
    taskStatus?: { 
        taskStatusId: string;
        taskStatusName: string;
    };
    priority?: { 
        priorityId: string;
        priorityName: string;
    };
    team?: {
        teamId: string;
        teamName: string;
        teamDescription?: string;
    };
}

export interface Priority {
    priorityId: string;
    priorityName: string;
}

export interface EditTaskRequest {
    title: string;
    description: string;
    startDate: string;
    endDate: string;
    priorityId: string;
}

export interface CreateTaskRequest {
    title: string;
    description: string;
    startDate: string;
    endDate: string;
}

export interface AssignTeamToTaskRequest {
    teamId: string;
}

export interface TaskStatus {
    taskStatusId: string;
    taskStatusName: string;
}

export interface UpdateTaskStatusRequest {
    taskStatusId: string;
}

export interface TaskService {
    getAllTasks: (projectId: string) => Promise<Task[]>;
    getAllPriorities: () => Promise<Priority[]>;
    getTaskDetails: (taskId: string) => Promise<Task>;
    getCurrentTasks: () => Promise<Task[]>;
    getAllTaskStatuses: () => Promise<TaskStatus[]>;
    createTask: (projectId: string, model: CreateTaskRequest) => Promise<Task>;
    editTask: (taskId: string, model : EditTaskRequest) => Promise<Task>;
    deleteTask: (taskId: string) => Promise<boolean>;
    assignTeamToTask: (taskId: string, model: AssignTeamToTaskRequest) => Promise<Task>;
    updateTaskStatus: (taskId: string, model: UpdateTaskStatusRequest) => Promise<Task>;
}