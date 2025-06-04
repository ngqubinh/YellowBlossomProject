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

interface TeamAddRequest {
  teamName: string;
  teamDescription: string;
}

interface TeamAddDialogProps {
  onSubmit: (formData: TeamAddRequest) => Promise<void>;
  loading?: boolean;
}

export function TeamAddDialog({ onSubmit, loading }: TeamAddDialogProps) {
  const [formData, setFormData] = useState<TeamAddRequest>({
    teamName: "",
    teamDescription: "",
  });
  const [open, setOpen] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      await onSubmit(formData);
      setOpen(false);
      setFormData({
        teamName: "",
        teamDescription: "",
      });
    } catch (error) {
      // Error handling should be done in the parent component
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>Thêm đội nhóm mới</Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Thêm đội nhóm mới</DialogTitle>
          <DialogDescription className="text-gray-300">
            Nhập thông tin đội nhóm mới. Nhấn "Lưu" khi hoàn tất.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Tên đội nhóm</label>
            <Input
              placeholder="Nhập tên đội nhóm"
              value={formData.teamName}
              onChange={(e) =>
                setFormData({ ...formData, teamName: e.target.value })
              }
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Mô tả</label>
            <Input
              placeholder="Nhập mô tả đội nhóm"
              value={formData.teamDescription}
              onChange={(e) =>
                setFormData({ ...formData, teamDescription: e.target.value })
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
              onClick={() => setOpen(false)}
              disabled={loading}
            >
              Hủy
            </Button>
            <Button
              type="submit"
              className="bg-blue-600 hover:bg-blue-700"
              disabled={loading}
            >
              {loading ? "Đang xử lý..." : "Lưu"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}