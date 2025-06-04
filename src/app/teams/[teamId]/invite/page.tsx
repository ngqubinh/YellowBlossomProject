"use client";
import { TeamInviteUserDialog } from "@/components/team/team-userInvation-dialog";
import { teamService } from "@/services/teamService";
import { InviteUserRequest } from "@/types/user";
import { useState } from "react";


export default function TeamInvitePage({ params }: { params: { teamId: string } }) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const handleInviteSubmit = async (formData: Omit<InviteUserRequest, "teamId">) => {
    try {
      setLoading(true);
      setError(null);
      setSuccessMessage(null);

      const response = await teamService.inviteUserToTeam({
        ...formData,
        teamId: params.teamId
      });

      if (response.success) {
        setSuccessMessage(`Invitation sent successfully to ${formData.email}!`);
      }
    } catch (err) {
      let errorMessage = "Failed to send invitation";
      if (err instanceof Error) {
        errorMessage = err.message;
      }
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h1 className="text-2xl font-bold mb-6 text-white">Team Management</h1>
      
      {/* Status Feedback */}
      {/* {error && (
        <Alert variant="destructive" className="mb-4">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}
      
      {successMessage && (
        <Alert variant="success" className="mb-4">
          <AlertDescription>{successMessage}</AlertDescription>
        </Alert>
      )} */}

      {/* Invite Dialog */}
      <div className="border rounded-lg p-4 bg-gray-800 border-gray-700">
        <h2 className="text-lg font-semibold mb-4 text-white">Member Invitation</h2>
        <TeamInviteUserDialog
          teamId={params.teamId}
          onSubmit={handleInviteSubmit}
          loading={loading}
        />
        
        <p className="mt-4 text-sm text-gray-300">
          Invitation link will expire after specified days. Max 10 invitations per day.
        </p>
      </div>
    </div>
  );
}