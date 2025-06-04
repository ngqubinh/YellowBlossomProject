import { CreateTestCaseRequest, CreateTestRunRequest, EditTestCaseRequest, EditTestRunRequest, TaskTest, TestCase, TestCaseStatus, TestRun, TestRunStatus, TestRunTestCaseDTO, TestService, TestType, UpdateResultRequest } from "@/types/test";
import authService from "./authService";
import axios from "axios";
import { toast } from "sonner";

export const testService : TestService = {
    async getDoneTasks() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/doneTasks`, {
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
                throw new Error(errorMessage || "Failed to fetch done tasks");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch done tasks");
        }
    },

    async getRelatedTestCases(taskId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/${taskId}/test-cases`, {
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

            return data as TestCase[];
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
                throw new Error(errorMessage || "Failed to fetch related test cases.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch related test cases.");
        }
    },

    async getTestCaseDetails(testCaseId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/testcases/${testCaseId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }, responseType: 'json'
            });


            return response.data as TestCase;
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
                throw new Error(errorMessage || "Failed to fetch test case details..");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch test case details.");
        }
    },

    async getRelatedTestRuns(taskId: string) {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/${taskId}/test-runs`, {
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

            return data as TestRun[];
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
                throw new Error(errorMessage || "Failed to fetch related test runs.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch related test runs.");
        }
    },

    async createTestCase(taskId: string, model: CreateTestCaseRequest) {
            const token = authService.getToken();
            if (!token) {
            throw new Error("User not authenticated.");
            }
    
            try {
                const response = await axios.post(
                    `http://localhost:5250/api/test/tests/${taskId}/create-test-case`,
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
    
                // Check for error message in response
                if (response.data.Message) {
                    throw new Error(response.data.Message);
                }

    
                return response.data as TestCase;
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
                throw new Error(errorMessage || "Failed to create test case.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to create test case.");
        }
    },

    async createTestRun(taskId: string, model: CreateTestRunRequest) {
            const token = authService.getToken();
            if (!token) {
            throw new Error("User not authenticated.");
            }
    
            try {
                const response = await axios.post(
                    `http://localhost:5250/api/test/tests/tasks/${taskId}/create-test-run`,
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
    
                // Check for error message in response
                if (response.data.Message) {
                    throw new Error(response.data.Message);
                }

    
                return response.data as TestRun;
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
                throw new Error(errorMessage || "Failed to create test case.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to create test case.");
        }
    },

    async editTestRun(testRunId: string, model: EditTestRunRequest) {
            const token = authService.getToken();
            if (!token) {
            throw new Error('User not authenticated.');
            }
    
            try {
            const response = await axios.put(
                `http://localhost:5250/api/test/testruns/${testRunId}/edit`,
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
    
            return response.data as TestRun;
            } catch (error) {
                if (axios.isAxiosError(error)) {
                    console.error("Axios error:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    message: error.message,
                    });
                    const errorMessage = error.response?.data?.Message 
                    || (typeof error.response?.data === "string" ? error.response.data : error.message);
                    throw new Error(errorMessage || "Failed to update test run.");
                }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to update test run.");
        }
    },

    async editTestCase(testCaseId: string, model: EditTestCaseRequest) {
            const token = authService.getToken();
            if (!token) {
            throw new Error('User not authenticated.');
            }
    
            try {
            const response = await axios.put(
                `http://localhost:5250/api/test/testcases/${testCaseId}/edit`,
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
    
            return response.data as TestCase;
            } catch (error) {
                if (axios.isAxiosError(error)) {
                    console.error("Axios error:", {
                    status: error.response?.status,
                    data: error.response?.data,
                    message: error.message,
                    });
                    const errorMessage = error.response?.data?.Message 
                    || (typeof error.response?.data === "string" ? error.response.data : error.message);
                    throw new Error(errorMessage || "Failed to update test case.");
                }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to update test case.");
        }
    },

    async getAllTestRunStatuses() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/test-run-statuses`, {
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

            return data as TestRunStatus[];
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
                throw new Error(errorMessage || "Failed to fetch related test cases.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch related test cases.");
        }
    },

    async getAllTestCaseStatuses() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/test-case-statuses`, {
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

            return data as TestCaseStatus[];
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
                throw new Error(errorMessage || "Failed to fetch related test case statuses.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch related test case statuses.");
        }
    },

    async getAllTestTypes() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/test-types`, {
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

            return data as TestType[];
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
                throw new Error(errorMessage || "Failed to fetch related test types.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch related test types.");
        }
    },

    async deleteTestRun(testRunId: string): Promise<boolean> {  
        const token = authService.getToken();
        if (!token) {
        throw new Error("User not authenticated");
        }

        try {
        const response = await axios.delete(`http://localhost:5250/api/test/testruns/${testRunId}/delete`, {
            headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
            },
        });

        console.log("Raw response:", response);

        if (response.status === 200) {
            toast.success("Xóa test run thành công!");
            return true;
        } else {
            throw new Error("Failed to delete test run");
        }
        } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
            config: error.config,
            });
            throw new Error(error.response?.data?.Message || error.message || "Failed to delete test run");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to delete test run");
        }
    }, 

    async deleteTestCase(testCaseId: string): Promise<boolean> {  
        const token = authService.getToken();
        if (!token) {
        throw new Error("User not authenticated");
        }

        try {
        const response = await axios.delete(`http://localhost:5250/api/test/testcases/${testCaseId}/delete`, {
            headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
            },
        });

        console.log("Raw response:", response);

        if (response.status === 200) {
            toast.success("Xóa test case thành công!");
            return true;
        } else {
            throw new Error("Failed to delete test case");
        }
        } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
            config: error.config,
            });
            throw new Error(error.response?.data?.Message || error.message || "Failed to delete test case");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to delete test case");
        }
    },

    async getAllTestForTester() {
        const token = authService.getToken();
        if (!token) {
            throw new Error('User not authenticated.')
        }

        try {
            const response = await axios.get(`http://localhost:5250/api/test/tests/testruns-testcases`, {
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

            return data as TaskTest[];
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
                throw new Error(errorMessage || "Failed to fetch tasks for tester.");
            }
            console.error("Unexpected error:", error);
            throw new Error(error instanceof Error ? error.message : "Failed to fetch tasks for tester.");
        }
    },

    async updateResult(testCaseId: string, testRunId: string, model: UpdateResultRequest) {
    const token = authService.getToken();
    if (!token) {
        throw new Error('User not authenticated.');
    }

    try {
        // Debug: Log the values being sent
        console.log("Sending data:", {
            actualResult: model.actualResult,
            testCaseStatusId: model.testCaseStatusId
        });

        // Wrap the model in a request object as expected by the API
        // const requestBody = {
        //     request: {
        //         actualResult: model.actualResult,
        //         testCaseStatusId: model.testCaseStatusId // Make sure this is a valid GUID string
        //     }
        // };

        // console.log("Request body:", JSON.stringify(requestBody, null, 2));

        const response = await axios.post(
            `http://localhost:5250/api/test/test-runs/${testRunId}/test-cases/${testCaseId}/results`,
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

        return response.data as TestRunTestCaseDTO;
    } catch (error) {
        if (axios.isAxiosError(error)) {
            console.error("Axios error:", {
                status: error.response?.status,
                data: error.response?.data,
                message: error.message,
            });
            const errorMessage = error.response?.data?.Message
                || (typeof error.response?.data === "string" ? error.response.data : error.message);
            throw new Error(errorMessage || "Failed to update result.");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to update result.");
    }
}
}