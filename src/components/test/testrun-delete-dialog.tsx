'use client';

import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { testService } from '@/services/testService';
import { toast } from 'sonner';
import { TestRun } from '@/types/test';

interface TestRunDeleteDialogProps {
  testRun: TestRun;
  onTestRunDeleted: () => void;
}

export function TestRunDeleteDialog({ testRun, onTestRunDeleted }: TestRunDeleteDialogProps) {
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleDelete = async () => {
    setLoading(true);

    // Validate testRunId as a GUID
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(testRun.testRunId)) {
      toast.error('Invalid test run ID');
      setLoading(false);
      return;
    }

    try {
      await testService.deleteTestRun(testRun.testRunId);
      toast.success('Test run deleted successfully!');
      setOpen(false);
      onTestRunDeleted();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to delete test run.');
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
          <DialogTitle>Xóa Test Run</DialogTitle>
        </DialogHeader>
        <div className="py-4">
          <p className="text-gray-300">
            Bạn có muốn xóa test run <strong>{testRun.title}</strong>? Hành động này không thể thu hồi.
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
            {loading ? 'Deleting...' : 'Xóa'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
