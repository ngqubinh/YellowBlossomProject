'use client';

import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { testService } from '@/services/testService';
import { toast } from 'sonner';
import { TestCase } from '@/types/test';

interface TestCaseDeleteDialogProps {
  testCase: TestCase;
  onTestCaseDeleted: () => void;
}

export function TestCaseDeleteDialog({ testCase, onTestCaseDeleted }: TestCaseDeleteDialogProps) {
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleDelete = async () => {
    setLoading(true);

    // Validate testCaseId as a GUID
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(testCase.testCaseId)) {
      toast.error('Invalid test case ID');
      setLoading(false);
      return;
    }

    try {
      await testService.deleteTestCase(testCase.testCaseId);
      toast.success('Test case deleted successfully!');
      setOpen(false);
      onTestCaseDeleted();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to delete test case.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm" className="border-red-500 text-red-500 hover:bg-red-500/10">
          Xóa
        </Button>
      </DialogTrigger>
      <DialogContent className="bg-gray-800 text-white max-w-md">
        <DialogHeader>
          <DialogTitle>Xóa Test Case</DialogTitle>
        </DialogHeader>
        <div className="py-4">
          <p className="text-gray-300">
            Bạn có chắc chắn muốn xóa test case <strong>{testCase.title}</strong>? Hành động này không thể hoàn tác.
          </p>
        </div>
        <DialogFooter>
          <Button type="button" variant="ghost" onClick={() => setOpen(false)} disabled={loading}>
            Hủy
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={handleDelete}
            disabled={loading}
          >
            {loading ? 'Đang xóa...' : 'Xóa'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}