'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { testService } from '@/services/testService';
import { CreateTestCaseRequest } from '@/types/test';

interface TestCaseAddDialogProps {
  taskId: string;
  onTestCaseCreated: () => void;
}

export function TestCaseAddDialog({ taskId, onTestCaseCreated }: TestCaseAddDialogProps) {
  const [formData, setFormData] = useState<CreateTestCaseRequest>({
    title: '',
    description: '',
    steps: '',
    expectedResult: '',
    actualResult: '',
  });
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      await testService.createTestCase(taskId, formData);
      onTestCaseCreated();
      setOpen(false);
      setFormData({
        title: '',
        description: '',
        steps: '',
        expectedResult: '',
        actualResult: '',
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Không thể tạo test case');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="bg-blue-600 hover:bg-blue-700 text-white">Thêm mới</Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px] bg-gray-800 text-white">
        <DialogHeader>
          <DialogTitle>Thêm test case mới</DialogTitle>
          <DialogDescription className="text-gray-300">
            Nhập thông tin test case mới. Nhấn "Lưu" khi hoàn tất.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Tiêu đề</label>
            <Input
              placeholder="Nhập tiêu đề test case"
              value={formData.title}
              onChange={(e) => setFormData({ ...formData, title: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Mô tả</label>
            <Textarea
              placeholder="Nhập mô tả test case"
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Các bước thực hiện</label>
            <Textarea
              placeholder="Nhập các bước thực hiện"
              value={formData.steps}
              onChange={(e) => setFormData({ ...formData, steps: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Kết quả mong đợi</label>
            <Input
              placeholder="Nhập kết quả mong đợi"
              value={formData.expectedResult}
              onChange={(e) => setFormData({ ...formData, expectedResult: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1 text-white">Kết quả thực tế</label>
            <Input
              placeholder="Nhập kết quả thực tế"
              value={formData.actualResult}
              onChange={(e) => setFormData({ ...formData, actualResult: e.target.value })}
              className="bg-gray-700 border-gray-600 text-white"
            />
          </div>
          {error && <p className="text-red-400 text-sm">{error}</p>}
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
              {loading ? 'Đang xử lý...' : 'Lưu'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}