// src/services/userService.ts
import apiClient from '@/lib/api-client';
import { User } from '@/types/auth';
// Define types for user-related API responses
interface UserResponse {
  user: User;
}

interface UsersResponse {
  users: User[];
}

class UserService {
  // Get current user profile
  async getCurrentUser(): Promise<User> {
    const response = await apiClient.get<UserResponse>('/users/me');
    return response.data.user;
  }
  
  // Update user profile
  async updateProfile(userId: string, userData: Partial<User>): Promise<User> {
    const response = await apiClient.put<UserResponse>(`/users/${userId}`, userData);
    return response.data.user;
  }
  
  // Change password
  async changePassword(userId: string, oldPassword: string, newPassword: string): Promise<void> {
    await apiClient.post(`/users/${userId}/change-password`, {
      oldPassword,
      newPassword
    });
  }
  
  // Get all users (admin only)
  async getAllUsers(): Promise<User[]> {
    const response = await apiClient.get<UsersResponse>('/users');
    return response.data.users;
  }
  
  // Other user-related API calls can be added here
}

// Export as singleton
const userService = new UserService();
export default userService;