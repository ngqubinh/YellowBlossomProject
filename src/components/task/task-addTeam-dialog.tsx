"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Team } from "@/types/user";

interface AssignTeamToTaskRequest {
  teamId: string;
}

interface AssignTeamToTaskDialogProps {
  onSubmit: (formData: AssignTeamToTaskRequest) => Promise<void>;
  loading?: boolean;
  taskId: string;
  teams: Team[];
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function AssignTeamToTaskDialog({
  onSubmit,
  loading,
  taskId,
  teams,
  open,
  onOpenChange,
}: AssignTeamToTaskDialogProps) {
  const [formData, setFormData] = useState<AssignTeamToTaskRequest>({
    teamId: "",
  });

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await onSubmit(formData);
      onOpenChange(false);
      setFormData({ teamId: "" });
    } catch (error) {
      // Error handling should be done in the parent component
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Gán team vào Task</DialogTitle>
          <DialogDescription className="text-gray-300">
            Chọn team từ danh sách. Ấn "Lưu" sau khi chọn!
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">
              Team
            </label>
            <Select
              value={formData.teamId}
              onValueChange={(value) =>
                setFormData({ ...formData, teamId: value })
              }
              required
            >
              <SelectTrigger className="bg-gray-700 border-gray-600 text-white">
                <SelectValue placeholder="Select a team" />
              </SelectTrigger>
              <SelectContent>
                {teams.map((team) => (
                  <SelectItem key={team.teamId} value={team.teamId}>
                    {team.teamName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              className="text-white border-gray-500 hover:bg-gray-700"
              onClick={() => onOpenChange(false)}
              disabled={loading}
            >
              Hủy
            </Button>
            <Button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700"
              disabled={loading || !formData.teamId}
            >
              {loading ? "Đang xử lý..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}