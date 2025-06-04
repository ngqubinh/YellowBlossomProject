"use client";

import { UsersTable } from "@/components/team/user-table";
import { teamService } from "@/services/teamService";
import { User } from "@/types/user";
import React, { useEffect, useState } from "react";
import { toast } from "sonner";


export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const fetchedUsers = await teamService.getAllUsers();
      setUsers(fetchedUsers);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} user:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách người dùng</h1>

      {error && <p className="text-red-600">Lỗi: {error}</p>}
      {loading && !users.length && <div>Đang tải người dùng...</div>}

      <UsersTable data={users} />
    </div>
  );
}