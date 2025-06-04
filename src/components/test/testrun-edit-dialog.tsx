'use client';

import { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { testService } from '@/services/testService';
import { toast } from 'sonner';
import { TestRun, TestRunStatus } from '@/types/test';

interface EditTestRunDialogProps {
  testRun: TestRun;
  onTestRunUpdated: () => void;
}

export function EditTestRunDialog({ testRun, onTestRunUpdated }: EditTestRunDialogProps) {
  const [open, setOpen] = useState(false);
  const [title, setTitle] = useState(testRun.title);
  const [description, setDescription] = useState(testRun.description || '');
  const [testRunStatusId, setTestRunStatusId] = useState(testRun.testRunStatus?.testRunStatusId || '');
  const [loading, setLoading] = useState(false);
  const [statuses, setStatuses] = useState<TestRunStatus[]>([]);

  useEffect(() => {
    // Fetch test run statuses
    const fetchStatuses = async () => {
      try {
        const fetchedStatuses = await testService.getAllTestRunStatuses();
        setStatuses(fetchedStatuses);
      } catch (error) {
        toast.error(error instanceof Error ? error.message : 'Failed to load test run statuses');
      }
    };
    fetchStatuses();

    setTitle(testRun.title);
    setDescription(testRun.description || '');
    setTestRunStatusId(testRun.testRunStatus?.testRunStatusId || '');
  }, [testRun]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    // Validate testRunStatusId as a GUID
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(testRunStatusId)) {
      toast.error('Invalid test run status ID');
      setLoading(false);
      return;
    }

    try {
      await testService.editTestRun(testRun.testRunId, {
        title,
        description,
        testRunStatusId,
      });

      toast.success('Test run updated successfully!');
      setOpen(false);
      onTestRunUpdated();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to update test run.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm">
          Chỉnh sửa
        </Button>
      </DialogTrigger>
      <DialogContent className="bg-gray-800 text-white max-w-md">
        <DialogHeader>
          <DialogTitle>Chỉnh sửa Test Run</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="title">Tên test run</Label>
            <Input
              id="title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Enter test run title"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="description">Mô tả</Label>
            <Textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Enter test run description"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="testRunStatusId">Trạng thái</Label>
            <Select value={testRunStatusId} onValueChange={setTestRunStatusId} required>
              <SelectTrigger id="testRunStatusId">
                <SelectValue placeholder="Select status" />
              </SelectTrigger>
              <SelectContent>
                {statuses.length > 0 ? (
                  statuses.map((status) => (
                    <SelectItem key={status.testRunStatusId} value={status.testRunStatusId}>
                      {status.testRunStatusName}
                    </SelectItem>
                  ))
                ) : (
                  <SelectItem value="" disabled>
                    Loading statuses...
                  </SelectItem>
                )}
              </SelectContent>
            </Select>
          </div>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => setOpen(false)}>
              Hủy
            </Button>
            <Button type="submit" disabled={loading || !testRunStatusId}>
              {loading ? 'Updating...' : 'Cập nhật'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}