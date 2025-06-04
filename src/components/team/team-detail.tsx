"use client";

import { Team } from "@/types/user";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { useState } from "react";
import { TeamInviteUserDialog } from "./team-userInvation-dialog";
import { teamService } from "@/services/teamService";

interface TeamDetailsCardProps {
  team: Team;
  onEdit?: () => void;
  onDelete?: () => void;
}

export function TeamDetailsCard({ team, onEdit, onDelete }: TeamDetailsCardProps) {

   const router = useRouter();
  const [isInviteDialogOpen, setInviteDialogOpen] = useState(false);

  const handleInviteSubmit = async (formData: { email: string; expiryDays: number }) => {
    try {
      // Gọi API service ở đây
      await teamService.inviteUserToTeam({ ...formData, teamId: team.teamId });
      
      toast.success("Đã gửi lời mời thành công!");
      setInviteDialogOpen(false);
      router.refresh();
    } catch (error) {
      toast.error("Gửi lời mời thất bại");
      console.error("Invitation error:", error);
    }
  };



  return (
    <Card className="bg-gray-800 text-white">
      <CardHeader>
        <div className="flex justify-between items-start">
          <CardTitle className="text-2xl">{team.teamName}</CardTitle>
          <div className="flex flex-col items-end gap-2">
            <Badge variant="outline">
              Mã Team: {team.teamId}
            </Badge>
            {team.createdDate && (
              <Badge variant="outline">
                Ngày tạo: {new Date(team.createdDate).toLocaleDateString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                })}
              </Badge>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <h3 className="font-semibold mb-2">Mô tả team</h3>
            <p className="text-gray-300">
              {team.teamDescription || "Không có mô tả"}
            </p>
          </div>

          <div className="space-y-4">
            <div>
              <h3 className="font-semibold mb-2">Người tạo</h3>
              <p className="text-gray-300">
                {team.createdBy || "Không xác định"}
              </p>
            </div>

            <div>
              <Table className="border border-gray-600 rounded-md">
                <TableHeader className="bg-gray-700">
                  <TableRow>
                    <TableHead className="text-white">STT</TableHead>
                    <TableHead className="text-white">Email</TableHead>
                    <TableHead className="text-white">Họ tên</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {team.users?.length ? (
                    team.users.map((user, index) => (
                      <TableRow key={index} className="hover:bg-gray-700">
                        <TableCell className="text-gray-300">{index + 1}</TableCell>
                        <TableCell className="text-gray-300">{user.email}</TableCell>
                        <TableCell className="text-gray-300">
                          {user.fullName || "-"}
                        </TableCell>
                      </TableRow>
                    ))
                  ) : (
                    <TableRow>
                      <TableCell colSpan={3} className="text-center text-gray-400 h-24">
                        Chưa có thành viên
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
              <div className="flex justify-end mt-4">
                <Button 
                  variant="secondary" 
                  size="sm" 
                  className="bg-blue-600 hover:bg-blue-700"
                  onClick={() => setInviteDialogOpen(true)}
                >
                  + Mời thành viên
                </Button>
              </div>
            </div>
          </div>
        </div>

        <div className="border-t border-gray-700 pt-4">
          <h3 className="font-semibold mb-2">Thông tin bổ sung</h3>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-gray-400">Tổng thành viên:</p>
              <p>{team.users?.length || 0}</p>
            </div>
            <div>
              <p className="text-gray-400">Trạng thái:</p>
              <Badge variant="outline" className="border-green-500 text-green-500">
                Hoạt động
              </Badge>
            </div>
          </div>
        </div>
      </CardContent>

       <TeamInviteUserDialog
        open={isInviteDialogOpen}
        onOpenChange={setInviteDialogOpen}
        teamId={team.teamId}
        onSubmit={handleInviteSubmit}
      />
    </Card>
  );
}