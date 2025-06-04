"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";
import { Loader2 } from "lucide-react";
import { Role } from "@/types/auth";
import UnauthorizedPage from "@/app/error/unauthorized";
import { useAuth } from "@/components/providers/AuthProvider";

interface RoleGuardProps {
  children: ReactNode;
  requiredRoles: Role | Role[];
  fallbackPath?: string;
}

export function RoleGuard({ 
  children, 
  requiredRoles, 
  fallbackPath = "/unauthorized" 
}: RoleGuardProps) {
  const { user, isLoading, hasRole } = useAuth();
  const router = useRouter();

  useEffect(() => {
    // Skip check while loading
    if (isLoading) return;
    
    // If no user or doesn't have required role, redirect
    if (!user || !hasRole(requiredRoles)) {
      router.push(fallbackPath);
    }
  }, [user, isLoading, hasRole, requiredRoles, router, fallbackPath]);

  // Show loading while checking
  if (isLoading) {
    return (
      <div className="flex h-screen w-screen items-center justify-center">
        <Loader2 className="h-10 w-10 animate-spin text-primary" />
      </div>
    );
  }

  // If not loading and either no user or doesn't have role, don't render children
  if (!user || !hasRole(requiredRoles)) {
    return <UnauthorizedPage/>;
  }

  // User has required role, render children
  return <>{children}</>;
}