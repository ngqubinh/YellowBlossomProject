import { AssignTeamToTaskRequest, CreateTaskRequest, EditTaskRequest, Task, TaskService, TaskStatus, UpdateTaskStatusRequest } from "@/types/task";
import authService from "./authService";
import axios from "axios";

export const taskService : TaskService = {
    async getAllTasks(projectId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/task/${projectId}/tasks`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }, responseType: 'json'
            });

            if (!response.data) {
                throw new Error('Empty response from server.')
            }

            const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

            if (Array.isArray(data) && data.length > 0 && data[0].Message) {
                throw new Error(data[0].Message);
            }

            if (!Array.isArray(data)) {
                throw new Error("Expected an array of projects but received a different data structure.");
            }

            return data;
            }catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                config: error.config,
                });
                const errorMessage =
                error.response?.data?.Message ||
                (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch tasks");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch tasks");
        }
    },

    async getAllPriorities() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/task/tasks/priorities`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }, responseType: 'json'
            });

            if (!response.data) {
                throw new Error('Empty response from server.')
            }

            const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

            if (Array.isArray(data) && data.length > 0 && data[0].Message) {
                throw new Error(data[0].Message);
            }

            if (!Array.isArray(data)) {
                throw new Error("Expected an array of projects but received a different data structure.");
            }

            return data;
            }catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                config: error.config,
                });
                const errorMessage =
                error.response?.data?.Message ||
                (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch priorites");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch priorites");
        }
    },

    async getAllTaskStatuses() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/task/tasks/statuses`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }, responseType: 'json'
            });

            if (!response.data) {
                throw new Error('Empty response from server.')
            }

            const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

            if (Array.isArray(data) && data.length > 0 && data[0].Message) {
                throw new Error(data[0].Message);
            }

            if (!Array.isArray(data)) {
                throw new Error("Expected an array of projects but received a different data structure.");
            }

            return data as TaskStatus[];
            }catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                config: error.config,
                });
                const errorMessage =
                error.response?.data?.Message ||
                (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch task statuses");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch task statuses");
        }
    },

    async getTaskDetails(taskId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.');
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/task/tasks/${taskId}/details`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                responseType: 'json'
            });

            if (!response.data) {
                throw new Error('Empty response from server.');
            }

            // Parse data nếu cần
            const data = typeof response.data === "string" 
                ? JSON.parse(response.data) 
                : response.data;

            // Kiểm tra lỗi từ API (nếu API trả về { Message: ... })
            if (data.Message) {
                throw new Error(data.Message);
            }

            // Kiểm tra xem data có đúng cấu trúc Task không
            const requiredFields = ['taskId', 'title', 'description', 'startDate', 'endDate', 'projectId'];
            const isValidTask = requiredFields.every(field => field in data);
            
            if (!isValidTask) {
                throw new Error("Invalid task data structure received from server.");
            }

            return data; // Trả về task đơn
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    message: error.message,
                });
                const errorMessage = error.response?.data?.Message 
                    || (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch task details");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch task details");
        }
    },

    async getCurrentTasks() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/task/tasks/related`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }, responseType: 'json'
            });

            if (!response.data) {
                throw new Error('Empty response from server.')
            }

            const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

            if (Array.isArray(data) && data.length > 0 && data[0].Message) {
                throw new Error(data[0].Message);
            }

            if (!Array.isArray(data)) {
                throw new Error("Expected an array of projects but received a different data structure.");
            }

            return data;
            }catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                config: error.config,
                });
                const errorMessage =
                error.response?.data?.Message ||
                (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch tasks");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch tasks");
        }
    },

    async createTask(projectId: string, model: CreateTaskRequest) {
        const token = authService.getToken();
        if (!token) {
        throw new Error("User not authenticated.");
        }

        try {
        // Format dates to UTC
        const formattedModel = {
            ...model,
            startDate: new Date(model.startDate).toISOString(),
            endDate: model.endDate ? new Date(model.endDate).toISOString() : "",
        };

        const response = await axios.post(
            `http://localhost:5250/api/task/${projectId}/create-task`,
            formattedModel,
            {
            headers: {
                Authorization: `Bearer ${token}`,
                "Content-Type": "application/json",
                Accept: "application/json",
            },
            }
        );

        if (!response.data) {
            throw new Error("Empty response from server.");
        }

        // Check for error message in response
        if (response.data.Message) {
            throw new Error(response.data.Message);
        }

        // Validate the returned task data
        const requiredFields = ["taskId", "title", "description", "startDate", "projectId"];
        const isValidTask = requiredFields.every((field) => field in response.data);
        if (!isValidTask) {
            throw new Error("Invalid task data structure received from server.");
        }

        return response.data as Task;
        } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
            config: error.config,
            });
            const errorMessage =
            error.response?.data?.Message ||
            (typeof error.response?.data === "string" ? error.response.data : error.message);
            throw new Error(errorMessage || "Failed to create task");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to create task");
        }
    },

    async editTask(taskId: string, model: EditTaskRequest) {
        const token = authService.getToken();
        if (!token) {
        throw new Error('User not authenticated.');
        }

        try {
        const response = await axios.post(
            `http://localhost:5250/api/task/tasks/${taskId}/edit`,
            model,
            {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            }
        );

        if (!response.data) {
            throw new Error('Empty response from server.');
        }

        // Kiểm tra cấu trúc response
        const requiredFields = ['taskId', 'title', 'description', 'startDate'];
        const isValidTask = requiredFields.every(field => field in response.data);
        
        if (!isValidTask) {
            throw new Error("Invalid task data structure received from server.");
        }

        return response.data;
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                });
                const errorMessage = error.response?.data?.Message 
                || (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to update task");
            }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to update task");
    }
  },

  async deleteTask(taskId: string) {
    const token = authService.getToken();
    if (!token) {
        throw new Error("User not authenticated.");
    }

    try {
        const response = await axios.delete(`http://localhost:5250/api/task/tasks/${taskId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
        },
        });

        if (!response.data) {
        throw new Error("Empty response from server.");
        }

        return response.data;
    } catch (error) {
        if (axios.isAxiosError(error)) {
        console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
        });
        const errorMessage =
            error.response?.data?.Message ||
            (typeof error.response?.data === "string" ? error.response.data : error.message);
        throw new Error(errorMessage || "Failed to delete task");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to delete task");
    }
    },

    async assignTeamToTask(taskId: string, model: AssignTeamToTaskRequest) {
        const token = authService.getToken();
        if (!token) {
        throw new Error("User not authenticated.");
        }

        try {
            const formattedModel = {
                ...model
            };

            const response = await axios.post(
                `http://localhost:5250/api/task/tasks/${taskId}/assign`,
                formattedModel,
                {
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                    Accept: "application/json",
                },
                }
            );

            if (!response.data) {
                throw new Error("Empty response from server.");
            }

            // Check for error message in response
            if (response.data.Message) {
                throw new Error(response.data.Message);
            }

            return response.data as Task;
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                config: error.config,
                });
                const errorMessage =
                error.response?.data?.Message ||
                (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to create task");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to create task");
        }
    },

    async updateTaskStatus(taskId: string, model: UpdateTaskStatusRequest) {
        const token = authService.getToken();
        if (!token) {
        throw new Error('User not authenticated.');
        }

        try {
        const response = await axios.post(
            `http://localhost:5250/api/task/tasks/${taskId}/update-status`,
            model,
            {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
            }
        );

        if (!response.data) {
            throw new Error('Empty response from server.');
        }

        return response.data as Task;
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
                });
                const errorMessage = error.response?.data?.Message 
                || (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to update task status");
            }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to update task status");
    }
  },
}