"use client";

import { User, AuthContextType, LoginCredentials, Role, RegisterCredentials } from "@/types/auth";
import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import authService from "@/services/authService";
import { usePathname, useRouter } from "next/navigation";
import { Loader2 } from "lucide-react";

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();
  const pathname = usePathname();

  // Sync localStorage with cookies for SSR compatibility
  const syncTokenWithCookie = (token: string | null) => {
    if (typeof document === "undefined") return;
    if (token) {
      document.cookie = `accessToken=${token}; path=/; max-age=${60 * 60 * 24 * 7}; SameSite=Lax`;
    } else {
      document.cookie = "accessToken=; path=/; max-age=0; SameSite=Lax";
    }
  };

  // Initialize auth state
  useEffect(() => {
    const initAuth = async () => {
      try {
        authService.setupInterceptors();
        if (authService.isAuthenticated()) {
          const currentUser = authService.getCurrentUser();
          console.log("Current User:", currentUser); // Debug
          setUser(currentUser);
          syncTokenWithCookie(authService.getToken());
        } else {
          const newToken = await authService.refreshToken();
          if (newToken) {
            const currentUser = authService.getCurrentUser();
            setUser(currentUser);
            syncTokenWithCookie(newToken);
          } else {
            setUser(null);
            syncTokenWithCookie(null);
          }
        }
      } catch (error) {
        console.error("Auth initialization error:", error);
        setUser(null);
        syncTokenWithCookie(null);
      } finally {
        setIsLoading(false);
      }
    };
    initAuth();
  }, []);

  const login = async (credentials: LoginCredentials) => {
    setIsLoading(true);
    try {
      const user = await authService.login(credentials);
      setUser(user);

      // Giả định user có thuộc tính role (ví dụ: 'admin', 'user', 'manager')
      const role = user.role;

      // Chuyển hướng dựa trên vai trò
      switch (role) {
        case 'ADMIN':
          router.push('/product-dashboard');
          break;
        case 'ProjectManager':
          router.push('/project/related-page');
          break;
        default:
          router.push('/');
          break;
      }
    } catch (error) {
      console.error('Login failed:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (credentials: RegisterCredentials) => {
    // Implement register logic if needed
    throw new Error("Register not implemented");
  };

  const logout = () => {
    authService.logout();
    setUser(null);
    syncTokenWithCookie(null);
    router.push("/auth/login");
  };

  const hasRole = (roleToCheck: Role | Role[]): boolean => {
    console.log("User:", user);
    console.log("User Role:", user?.role);
    console.log("Role to Check:", roleToCheck);
    if (!user) return false;
    const userRoles = Array.isArray(user.role) ? user.role : [user.role];
    if (Array.isArray(roleToCheck)) {
      return roleToCheck.some((role) => userRoles.includes(role));
    }
    return userRoles.includes(roleToCheck);
  };

  const isProtectedRoute = (requiredRoles: Role | Role[]): boolean => {
    return hasRole(requiredRoles);
  };

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    isLoading,
    login,
    register,
    logout,
    hasRole,
    isProtectedRoute
  };

  if (isLoading && !pathname.startsWith("/auth/")) {
    return (
      <div className="flex h-screen w-screen items-center justify-center">
        <Loader2 className="h-10 w-10 animate-spin text-primary" />
      </div>
    );
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};