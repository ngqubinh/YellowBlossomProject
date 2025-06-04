'use client';

import { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { testService } from '@/services/testService';
import { toast } from 'sonner';

interface TestRunAddDialogProps {
  taskId: string;
  onTestRunCreated: () => void;
}

export function TestRunAddDialog({ taskId, onTestRunCreated }: TestRunAddDialogProps) {
  const [open, setOpen] = useState(false);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [runDate, setRunDate] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      await testService.createTestRun(taskId, {
        title,
        description,
        runDate: runDate || new Date().toISOString(),
      });
      toast.success('Test run created successfully!');
      setOpen(false);
      setTitle('');
      setDescription('');
      setRunDate('');
      onTestRunCreated();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to create test run.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>Thêm mới</Button>
      </DialogTrigger>
      <DialogContent className="bg-gray-800 text-white max-w-md">
        <DialogHeader>
          <DialogTitle>Create New Test Run</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="title">Title</Label>
            <Input
              id="title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Enter test run title"
              required
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Enter test run description"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="runDate">Run Date</Label>
            <Input
              id="runDate"
              type="datetime-local"
              value={runDate}
              onChange={(e) => setRunDate(e.target.value)}
            />
          </div>
          <DialogFooter>
            <Button type="button" variant="ghost" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? 'Creating...' : 'Create'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}