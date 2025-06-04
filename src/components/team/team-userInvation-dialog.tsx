// components/team/team-userInvation-dialog.tsx
"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { InviteUserRequest } from "@/types/user";

interface TeamInviteUserDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (formData: InviteUserRequest) => Promise<void>;
  teamId: string;
}

export function TeamInviteUserDialog({
  open,
  onOpenChange,
  onSubmit,
  teamId,
}: TeamInviteUserDialogProps) {
  const [formData, setFormData] = useState<Omit<InviteUserRequest, "teamId">>({
    email: "",
    expiryDays: 2,
  });
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      // Merge với teamId từ props
      await onSubmit({
        ...formData,
        teamId
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[425px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Mời thành viên mới</DialogTitle>
          <DialogDescription className="text-gray-300">
            Gửi lời mời tham gia đội nhóm qua email
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Email</label>
            <Input
              type="email"
              placeholder="Nhập email thành viên"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Thời hạn (ngày)</label>
            <Input
              type="number"
              min="1"
              placeholder="Số ngày hiệu lực"
              value={formData.expiryDays}
              onChange={(e) => 
                setFormData({ 
                  ...formData, 
                  expiryDays: Math.max(1, parseInt(e.target.value) || 1) 
                })
              }
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
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
              disabled={loading}
            >
              {loading ? "Đang gửi..." : "Gửi lời mời"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}