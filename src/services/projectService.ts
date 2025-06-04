import { ProjectService, EditProjectRequest, CreateProjectRequest, AssignTeamToProject, ProjectTeamDTO } from "@/types/project";
import authService from "./authService";
import axios from "axios";

export const projectService: ProjectService = {
  async getAllProjectStatuses() {
    try {
      const response = await axios.get(
        `http://localhost:5250/api/project/project-statuses`,
        {
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          responseType: "json",
        }
      );

      if (!response.data) {
        throw new Error("Empty response from server.");
      }

      const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

      if (Array.isArray(data) && data.length > 0 && data[0].Message) {
        throw new Error(data[0].Message);
      }

      if (!Array.isArray(data)) {
        throw new Error("Expected an array of projects but received a different data structure.");
      }

      return data;
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
        throw new Error(errorMessage || "Failed to fetch project statuses.");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to fetch project statuses.");
    }
  },

  async getAllRelatedProjectsForPM() {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }
    try {
      const response = await axios.get(
        `http://localhost:5250/api/project/projects/related`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          responseType: "json",
        }
      );

      if (!response.data) {
        throw new Error("Empty response from server.");
      }

      const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

      if (Array.isArray(data) && data.length > 0 && data[0].Message) {
        throw new Error(data[0].Message);
      }

      if (!Array.isArray(data)) {
        throw new Error("Expected an array of projects but received a different data structure.");
      }

      return data;
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
        throw new Error(errorMessage || "Failed to fetch projects.");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to fetch projects.");
    }
  },

  async getAllProjects(productId: string) {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }

    try {
      const response = await axios.get(
        `http://localhost:5250/api/project/products/${productId}/projects`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          responseType: "json",
        }
      );

      if (!response.data) {
        throw new Error("Empty response from server.");
      }

      const data = typeof response.data === "string" ? JSON.parse(response.data) : response.data;

      if (Array.isArray(data) && data.length > 0 && data[0].Message) {
        throw new Error(data[0].Message);
      }

      if (!Array.isArray(data)) {
        throw new Error("Expected an array of projects but received a different data structure.");
      }

      return data;
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
        throw new Error(errorMessage || "Failed to fetch projects");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to fetch projects");
    }
  },

  async createProject(productId: string, model: CreateProjectRequest) {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }

    try {
      // Format dates to UTC
      const formattedModel = {
        ...model,
        startDate: new Date(model.startDate).toISOString(),
        endDate: new Date(model.endDate).toISOString(),
      };

      const response = await axios.post(
        `http://localhost:5250/api/project/products/${productId}/create-project`,
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

      return response.data;
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
        throw new Error(errorMessage || "Failed to create project");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to create project");
    }
  },

  async editProject(projectId: string, model: EditProjectRequest) {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }

    try {
      const updatedModel = {
        ...model,
        startDate: new Date(model.startDate).toISOString(), // ✅ Convert to UTC
        endDate: new Date(model.endDate).toISOString(), // ✅ Convert to UTC
      };

      const response = await axios.post(
        `http://localhost:5250/api/project/projects/${projectId}/edit`,
        updatedModel,
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

      return response.data;
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
        throw new Error(errorMessage || "Failed to edit project");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to edit project");
    }
  },

  async updateProjectStatus(projectId: string, projectStatusId: string) {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }

    try {
      const response = await axios.put(
        `http://localhost:5250/api/project/projects/${projectId}/update-status`,
        { ProjectStatusId: projectStatusId },
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

      return response.data;
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
        throw new Error(errorMessage || "Failed to update project status");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to update project status");
    }
  },

  async deleteProject(projectId: string): Promise<boolean> {  
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated");
    }

    try {
      const response = await axios.delete(`http://localhost:5250/api/project/projects/${projectId}/delete`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
          Accept: "application/json",
        },
      });

      console.log("Raw response:", response);

      if (response.status === 200) {
        console.log("Xóa dự án thành công!");
        return true;
      } else {
        throw new Error("Failed to delete project");
      }
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Axios error:", {
          status: error.response?.status,
          data: error.response?.data,
          message: error.message,
          config: error.config,
        });
        throw new Error(error.response?.data?.Message || error.message || "Failed to delete dự án");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to dự án");
    }
  },

  async assignTeamToProject(projectId: string, model: AssignTeamToProject) {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated.");
    }

    try {
      const response = await axios.post(
        `http://localhost:5250/api/project/${projectId}/assign-team-to-project`,
        model,
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

      return response.data as ProjectTeamDTO;
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
        throw new Error(errorMessage || "Failed to edit project");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to edit project");
    }
  },
};
