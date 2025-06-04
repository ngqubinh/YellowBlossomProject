"use client";

import React, { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { teamService } from "@/services/teamService";
import { Team } from "@/types/user";
import { TeamDetailsCard } from "@/components/team/team-detail";

export default function TeamDetailPage() {
  const { teamId } = useParams();
  const router = useRouter();
  const [team, setTeam] = useState<Team | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

  useEffect(() => {
    if (teamId) {
      fetchTeamDetails(teamId as string);
    }
  }, [teamId]);

  const fetchTeamDetails = async (id: string) => {
    try {
      setLoading(true);
      const teamData = await teamService.getTeamDetails(id);
      setTeam(teamData);
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

  const handleEdit = () => setIsEditing(true);
  const handleCloseEdit = () => setIsEditing(false);

  const handleUpdateTeam = async (formData: any) => {
    if (!teamId) return;

    try {
      //await teamService.updateTeam(teamId as string, formData);
      await fetchTeamDetails(teamId as string);
      toast.success("Cập nhật team thành công!");
      setIsEditing(false);
    } catch (err: any) {
      handleError(err, "update");
    }
  };

  const handleOpenDeleteDialog = () => setIsDeleteDialogOpen(true);
  const handleCloseDeleteDialog = () => setIsDeleteDialogOpen(false);

  const handleDeleteTeam = async () => {
    if (!teamId) return;

    try {
      setIsDeleting(true);
      //ait teamService.deleteTeam(teamId as string);
      toast.success("Xóa team thành công");
      router.push("/teams");
    } catch (err: any) {
      handleError(err, "delete");
    } finally {
      setIsDeleting(false);
    }
  };

  if (loading) return <div className="text-center py-8">Đang tải thông tin team...</div>;

  return (
    <div className="container mx-auto py-8">
      <div className="mb-6">
        <Button variant="outline" onClick={() => router.push("/team")}>
          ← Quay lại
        </Button>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {team && (
        <>
          <TeamDetailsCard
            team={team}
            onEdit={handleEdit}
            onDelete={handleOpenDeleteDialog}
          />

          {/* {isEditing && (
            <TeamEditDialog
              team={team}
              onSubmit={handleUpdateTeam}
              onClose={handleCloseEdit}
            />
          )} */}

          {/* <TeamDeleteDialog
            open={isDeleteDialogOpen}
            onOpenChange={handleCloseDeleteDialog}
            teamName={team.teamName}
            onConfirm={handleDeleteTeam}
            loading={isDeleting}
          /> */}
        </>
      )}
    </div>
  );
}