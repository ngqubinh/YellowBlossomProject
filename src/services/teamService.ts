import { CreateTeamRequest, GeneralResponse, InviteUserRequest, Team, UserService } from "@/types/user";
import authService from "./authService";
import axios from "axios";

export const teamService: UserService = {
    async getAllUsers() {
        const token = authService.getToken();
        if (!token) {
            throw new Error("User not authenticated.");
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/team/teams/users`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                    Accept: "application/json",
                },
                responseType: 'json'
            });

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

    async getAllTeams() {
        const token = authService.getToken();
        if (!token) {
            throw new Error("User not authenticated.");
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/team/teams`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    "Content-Type": "application/json",
                    Accept: "application/json",
                },
                responseType: 'json'
            });

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
                throw new Error(errorMessage || "Failed to fetch teams.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch teams.");
        }
    },

    async getTeamDetails(teamId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.');
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/team/teams/${teamId}/details`, {
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

            return data;
        } catch (error) {
            if (axios.isAxiosError(error)) {
                console.error("Axios error:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    message: error.message,
                });
                const errorMessage = error.response?.data?.Message 
                    || (typeof error.response?.data === "string" ? error.response.data : error.message);
                throw new Error(errorMessage || "Failed to fetch team details");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch team details");
        }
    },

    async createTeam(model: CreateTeamRequest) {
        const token = authService.getToken();
        if (!token) {
        throw new Error("User not authenticated.");
        }

        try {
        const response = await axios.post(
            `http://localhost:5250/api/team/create-team`,
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
        
        if (response.data.Message) {
            throw new Error(response.data.Message);
        }

        return response.data as Team;
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
            throw new Error(errorMessage || "Failed to create team");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to create team");
        }
    },

    async inviteUserToTeam(model: InviteUserRequest) {
        const token = authService.getToken();
        if (!token) {
            throw new Error("User not authenticated.");
        }

        try {
            const response = await axios.post(
                `http://localhost:5250/api/team/invite`,
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
            
            if (response.data.Message) {
                throw new Error(response.data.Message);
            }

            return response.data as GeneralResponse;
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
                throw new Error(errorMessage || "Failed to invite user into team");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to invite user into team.");
        }
    },
}