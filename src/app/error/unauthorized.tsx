"use client";

import Link from "next/link";
import { ShieldAlert } from "lucide-react";

export default function UnauthorizedPage() {
  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 p-4 text-center">
      <ShieldAlert className="h-16 w-16 text-red-500" />
      <h1 className="text-3xl font-bold">Access Denied</h1>
      <p className="max-w-md text-gray-600">
        You do not have permission to access this page. Please contact an administrator if you believe this is an error.
      </p>
      <Link 
        href="/"
        className="mt-4 rounded-md bg-primary px-4 py-2 text-white transition-colors hover:bg-primary/90"
      >
        Return to Home
      </Link>
    </div>
  );
}