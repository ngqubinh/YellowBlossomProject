import { LoginCredentials, User, UserRespone } from "@/types/auth";
import axios from "axios";
import { jwtDecode } from "jwt-decode"

class AuthService {
    private apiUrl = "http://localhost:5250/api";

    async login(credentials: LoginCredentials): Promise<User> {
        try {
            const response = await axios.post<UserRespone>(
                `${this.apiUrl}/auth/sign-in`,
                credentials,
                {
                    headers: {
                        "Content-Type": "application/json", // Ensuring correct media type
                    },
                }
            );
            const { accessToken } = response.data;
            this.setTokens(accessToken);
            return this.getUserFromToken(accessToken);
        } catch (error) {
            console.error("Login failed: ", error);
            throw error;
        }
    }


    logout(): void {
        if (typeof window !== 'undefined') {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
        }
    }

    isAuthenticated(): boolean {
        const token = this.getToken();
        if (!token) return false;
        try {
            const decoded: any = jwtDecode(token);
            return decoded.exp * 1000 > Date.now();
        }
        catch {
            return false;
        }
    }

    getCurrentUser(): User | null {
        const token = this.getToken();
        if (!token) return null;
        try {
            return this.getUserFromToken(token);
        } catch {
            return null;
        }
    }

    getToken(): string | null {
        if (typeof window !== "undefined") {
            return localStorage.getItem("accessToken");
        }
        return null;
    }


    private setTokens(accessToken: string, refreshToken?: string): void {
        if (typeof window !== "undefined") {
            localStorage.setItem("accessToken", accessToken);
            if (refreshToken) {
            localStorage.setItem("refreshToken", refreshToken);
            }
        }
    }


    private getUserFromToken(token: string): User {
    try {
      const decoded: any = jwtDecode(token);
      console.log("Decoded token:", decoded);
      return {
        id: decoded.nameid || decoded.sub || decoded.id, // Prioritize nameid
        email: decoded.email,
        fullName: decoded.fullName || "", // Default to empty string if undefined
        role: decoded.role || decoded.Role || "", // Handle role or Role
      };
    } catch (error) {
      console.error("Failed to decode token: ", error);
      throw new Error("Invalid token.");
    }
  }

    async refreshToken(): Promise<string | null> {
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) return null;
        
        try {
          const response = await axios.post<UserRespone>(
            `${this.apiUrl}/auth/refresh`,
            { refreshToken }
          );
          
          const { accessToken, refreshToken: newRefreshToken } = response.data;
          this.setTokens(accessToken, newRefreshToken);
          
          return accessToken;
        } catch (error) {
          console.error('Token refresh failed:', error);
          this.logout();
          return null;
        }
      }

    setupInterceptors(): void {
        axios.interceptors.response.use(
            (response) => response,
            async (error) => {
                const originalRequest = error.config;
                if (error.response?.status === 401 && !originalRequest._retry) {
                    originalRequest._retry = true;

                    try {
                        const newToken = await this.refreshToken();
                        if (newToken) {
                            originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
                            return axios(originalRequest);
                        }
                    } catch (refreshError) {
                        this.logout();
                        window.location.href = '/auth/sign-in';
                    }
                }
                return Promise.reject(error);
            }
        )
    }
}


const authService = new AuthService();
export default authService;