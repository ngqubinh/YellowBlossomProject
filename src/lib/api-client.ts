// src/lib/api-client.ts
import axios from 'axios';
import authService from '@/services/authService';

// Create an axios instance with base configuration
const apiClient = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5250/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to attach the auth token to every request
apiClient.interceptors.request.use(
  (config) => {
    // Get the token
    const token = authService.getToken();
    
    // If token exists, add it to the header
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle token refresh
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    // If error is 401 (Unauthorized) and we haven't retried yet
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        // Try to refresh the token
        const newToken = await authService.refreshToken();
        
        // If we got a new token, retry the original request
        if (newToken) {
          originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // If refresh fails, let the auth service handle the logout
        authService.logout();
      }
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;