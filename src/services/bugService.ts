import { Bug, BugService, ResolveBugRequest } from "@/types/bug";
import axios from "axios";
import authService from "./authService";

export const bugService : BugService = {
    async getAllBugs() {
        const token = authService.getToken();
        if (!token) {
            throw new Error("User not authenticated.");
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/bug/bugs`, {
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
            return data as Bug[];
        }  catch (error) {
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
                throw new Error(errorMessage || "Failed to fetch bugs.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch bugs.");
        }
    },

    async resolveBug(bugId: string, model: ResolveBugRequest) {
        const token = authService.getToken();
            if (!token) {
                throw new Error('User not authenticated.');
            }

            try {
                const response = await axios.post(
                    `http://localhost:5250/api/bug/bugs/${bugId}/resolve`,
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

                return response.data as Bug;
            } catch (error) {
                if (axios.isAxiosError(error)) {
                    console.error("Axios error:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    message: error.message,
                    });
                    const errorMessage = error.response?.data?.Message 
                    || (typeof error.response?.data === "string" ? error.response.data : error.message);
                    throw new Error(errorMessage || "Failed to resolve bug.");
                }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to resolve bug.");
        }
    },
}