export interface User {
    id: string;
    fullName: string;
    email: string;
    role: string;
}

export interface LoginCredentials {
    email: string; 
    password: string;
}

export interface RegisterCredentials extends LoginCredentials {
    confirmPassword: string;
}

export interface UserRespone {
    accessToken: string;
    refreshToken: string;
}

export interface AuthContextType {
    user: User | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    hasRole: (role: Role | Role[]) => boolean;
    isProtectedRoute: (role: Role | Role[]) => boolean;
    login: (credential: LoginCredentials) => Promise<void>;
    register: (credentials: RegisterCredentials) => Promise<void>;
    logout: () => void;
}

export type Role = "ADMIN" | "user" | "qa" | "developer" | "tester" | 'ProjectManager'; 