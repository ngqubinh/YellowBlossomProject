"use client";

import React, { useEffect, useState } from "react";
import { toast } from "sonner";
import { TeamAddDialog } from "@/components/team/team-add-dialog";
import { CreateTeamRequest, Team } from "@/types/user";
import { teamService } from "@/services/teamService";
import { TeamsTable } from "@/components/team/team-table";
import { useRouter } from "next/navigation";

export default function TeamsPage() {
  const [teams, setTeams] = useState<Team[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  useEffect(() => {
    fetchTeams();
  }, []);

  const handleViewTeam = (teamId: string) => {
    router.push(`/team-details/${teamId}`);
  };

  const fetchTeams = async () => {
    setLoading(true);
    try {
      const fetchedTeams = await teamService.getAllTeams();
      setTeams(fetchedTeams);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} team:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  const handleAddTeam = async (formData: CreateTeamRequest) => {
    setLoading(true);
    try {
      await teamService.createTeam(formData);
      await fetchTeams();
      toast.success("Thêm đội nhóm thành công!");
    } catch (err: any) {
      handleError(err, "add");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách đội nhóm</h1>

      {error && <p className="text-red-600">Lỗi: {error}</p>}
      {loading && !teams.length && <div>Đang tải đội nhóm...</div>}

      {/* Nút và Dialog để thêm đội nhóm */}
      <div className="mb-4">
        <TeamAddDialog onSubmit={handleAddTeam} loading={loading} />
      </div>

       <TeamsTable 
        data={teams} 
        onView={handleViewTeam} 
      />
    </div>
  );
}