"use client";

import { Task } from "@/types/task";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";

// Component ActionButtons
interface ActionButtonsProps {
  onEdit?: () => void;
  onDelete?: () => void;
  onAssignTeam?: () => void;
  hasTeam: boolean;
}

export function ActionButtons({ 
  onEdit, 
  onDelete, 
  onAssignTeam,
  hasTeam 
}: ActionButtonsProps) {
  return (
    <div className="flex gap-2">
      <Button
        variant="outline"
        size="sm"
        onClick={onAssignTeam}
        disabled={hasTeam}
        className={hasTeam ? "opacity-50 cursor-not-allowed" : ""}
      >
        {hasTeam ? "Đã gán team" : "Gán team"}
      </Button>
      <Button variant="outline" size="sm" onClick={onEdit}>
        Chỉnh sửa
      </Button>
      <Button variant="destructive" size="sm" onClick={onDelete}>
        Xóa
      </Button>
    </div>
  );
}

// Component TaskDetailsCard
interface TaskDetailsCardProps {
  task: Task;
  onEdit?: () => void;
  onDelete?: () => void;
  onAssignTeam?: () => void;
}

const getStatusLabel = (statusName?: string) => {
  switch (statusName) {
    case 'Done':
      return 'Hoàn thành';
    case 'InProgress':
      return 'Đang thực hiện';
    case 'Todo':
      return 'Cần làm';
    default:
      return 'Không xác định';
  }
};

export function TaskDetailsCard({ 
  task, 
  onEdit, 
  onDelete,
  onAssignTeam 
}: TaskDetailsCardProps) {
  const hasTeam = Boolean(task.team?.teamId);

  return (
    <Card className="bg-gray-800 text-white">
      <CardHeader>
        <div className="flex justify-between items-start">
          <CardTitle className="text-2xl">{task.title}</CardTitle>
          <div className="flex flex-col items-end gap-2">
            <Badge variant={task.taskStatus?.taskStatusName === "Done" ? "default" : "outline"}>
              {getStatusLabel(task.taskStatus?.taskStatusName) || "Unknown Status"}
            </Badge>
            {task.priority && (
              <Badge variant="outline">
                Ưu tiên: {task.priority.priorityName}
              </Badge>
            )}
          </div>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <h3 className="font-semibold mb-2">Mô tả task</h3>
            <p className="text-gray-300">{task.description}</p>
          </div>

          <div className="space-y-2">
            <div>
              <h3 className="font-semibold">Ngày bắt đầu</h3>
              <p>
                {new Date(task.startDate).toLocaleDateString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                })}
              </p>
            </div>

            <div>
              <h3 className="font-semibold">Ngày kết thúc</h3>
              <p>
                {new Date(task.endDate).toLocaleDateString("vi-VN", {
                  day: "2-digit",
                  month: "2-digit",
                  year: "numeric",
                })}
              </p>
            </div>
          </div>
        </div>

        <div className="border-t border-gray-700 pt-4">
          <h3 className="font-semibold mb-2">Thông tin bổ sung</h3>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <p className="text-gray-400">Mã Task:</p>
              <p>{task.taskId}</p>
            </div>
            <div>
              <p className="text-gray-400">Team được gán:</p>
              <p>
                {task.team?.teamName || "Chưa có team"} - {task.team?.teamId || ""}
              </p>
            </div>
          </div>
        </div>
        
        <div className="flex justify-end gap-2 border-t border-gray-700 pt-4">
          <ActionButtons 
            onEdit={onEdit} 
            onDelete={onDelete}
            onAssignTeam={hasTeam ? undefined : onAssignTeam}
            hasTeam={hasTeam}
          />
        </div>
      </CardContent>
    </Card>
  );
}