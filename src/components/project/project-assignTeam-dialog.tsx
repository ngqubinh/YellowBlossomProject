"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { teamService } from "@/services/teamService";
import { projectService } from "@/services/projectService";

interface Team {
  teamId: string;
  teamName: string;
}

interface AssignTeamDialogProps {
  projectId: string;
  loading?: boolean;
  onSubmit: (data: { teamId: string; roleOfTeam: string }) => Promise<void>; 
  onOpenChange: (open: boolean) => void;
}

export function AssignTeamDialog({
  projectId,
  onSubmit,
  onOpenChange,
}: AssignTeamDialogProps) {
  const [teams, setTeams] = useState<Team[]>([]);
  const [teamId, setTeamId] = useState<string>("");
  const [roleOfTeam, setRoleOfTeam] = useState<string>("");
  const [loading, setLoading] = useState(false);
  const [fetchError, setFetchError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchTeams() {
      try {
        const teamData = await teamService.getAllTeams();
        setTeams(teamData);
        if (teamData.length > 0) {
          setTeamId(teamData[0].teamId);
        }
      } catch (error) {
        console.error("Failed to fetch teams:", error);
        setFetchError("Không thể tải danh sách nhóm");
      }
    }
    fetchTeams();
  }, []);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!teamId || !roleOfTeam) return;

    setLoading(true);
    try {
      await onSubmit({ teamId, roleOfTeam }); // Truyền dữ liệu đúng cách
      onOpenChange(false); // Đóng dialog sau khi thành công
    } catch (error) {
      console.error("Error assigning team:", error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={true} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px] bg-gray-800">
        <DialogHeader>
          <DialogTitle className="text-white">Giao Nhóm cho Dự án</DialogTitle>
          <DialogDescription className="text-gray-300">
            Chọn nhóm và nhập vai trò cho dự án. Nhấn "Lưu" để xác nhận.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Chọn Nhóm
            </label>
            {fetchError ? (
              <p className="text-sm text-red-400">{fetchError}</p>
            ) : (
              <Select
                value={teamId}
                onValueChange={setTeamId}
                disabled={loading || teams.length === 0}
              >
                <SelectTrigger className="text-white border-gray-600 bg-gray-700">
                  <SelectValue
                    placeholder={
                      teams.length > 0 ? "Chọn nhóm" : "Đang tải danh sách nhóm..."
                    }
                  />
                </SelectTrigger>
                <SelectContent className="bg-gray-700 text-white border-gray-600">
                  {teams.map((team) => (
                    <SelectItem
                      key={team.teamId}
                      value={team.teamId}
                      className="text-white"
                    >
                      {team.teamName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Vai trò của Nhóm
            </label>
            <input
              type="text"
              value={roleOfTeam}
              onChange={(e) => setRoleOfTeam(e.target.value)}
              disabled={loading}
              className="w-full px-3 py-2 text-white bg-gray-700 border border-gray-600 rounded-md focus:ring-blue-500 focus:border-blue-500"
              placeholder="Nhập vai trò (ví dụ: Developer, Tester...)"
            />
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              className="text-white border-white hover:bg-gray-700"
              onClick={() => onOpenChange(false)}
              disabled={loading}
            >
              Hủy
            </Button>
            <Button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700 text-white"
              disabled={loading || !teamId || !roleOfTeam || fetchError !== null}
            >
              {loading ? "Đang xử lý..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}