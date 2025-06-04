'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { TestRun, TestRunStatus } from '@/types/test';
import { testService } from '@/services/testService';
import { Skeleton } from '@/components/ui/skeleton';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { format } from 'date-fns';
import { TestRunAddDialog } from '@/components/test/testrun-add-dialog';
import { EditTestRunDialog } from '@/components/test/testrun-edit-dialog';
import { TestRunDeleteDialog } from '@/components/test/testrun-delete-dialog';

export default function TestRunsPage({ params }: { params: { taskId: string } }) {
  const [testRuns, setTestRuns] = useState<TestRun[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedTestRun, setSelectedTestRun] = useState<TestRun | null>(null);
  const router = useRouter();

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await testService.getRelatedTestRuns(params.taskId);
        setTestRuns(data);
        setError(null);
      } catch (error) {
        setError(error instanceof Error ? error.message : 'Failed to load test runs');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [params.taskId]);

  const handleTestRunUpdated = () => {
    testService.getRelatedTestRuns(params.taskId).then(setTestRuns).catch(console.error);
  };

  const getStatusColor = (statusName?: string) => {
    const normalizedStatus = typeof statusName === 'string' ? statusName.toLowerCase() : '';
    switch (normalizedStatus) {
      case 'passed':
        return 'bg-green-500/20 text-green-400';
      case 'failed':
        return 'bg-red-500/20 text-red-400';
      case 'in progress':
        return 'bg-blue-500/20 text-blue-400';
      default:
        return 'bg-gray-500/20 text-gray-400';
    }
  };

  if (loading) {
    return (
      <div className="max-w-4xl mx-auto p-6 space-y-4">
        <div className="flex justify-between items-center mb-8">
          <Skeleton className="h-8 w-64 bg-gray-800 rounded-lg" />
          <Skeleton className="h-10 w-32 bg-gray-800 rounded-lg" />
        </div>
        <div className="space-y-4">
          {[...Array(3)].map((_, i) => (
            <Skeleton key={i} className="h-28 w-full bg-gray-800 rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-4xl mx-auto p-6">
        <div className="rounded-xl bg-red-900/20 border border-red-800 p-6 flex flex-col items-center text-center">
          <svg 
            className="h-12 w-12 text-red-400 mb-4"
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path 
              strokeLinecap="round" 
              strokeLinejoin="round" 
              strokeWidth={2} 
              d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" 
            />
          </svg>
          <h2 className="text-xl font-semibold text-red-100 mb-2">Error</h2>
          <p className="text-red-400 mb-6">{error}</p>
          <Button
            onClick={() => router.refresh()}
            variant="destructive"
          >
            Try Again
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto p-6">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">
            Test Runs
          </h1>
          <p className="text-gray-400 mt-1">Task ID: {params.taskId}</p>
        </div>
        <Dialog>
          <TestRunAddDialog
            taskId={params.taskId}
            onTestRunCreated={handleTestRunUpdated}
          />
        </Dialog>
      </div>

      {testRuns.length === 0 ? (
        <div className="rounded-xl border-2 border-dashed border-gray-700 p-8 text-center">
          <p className="text-gray-400 mb-4">No test runs found</p>
          <Button variant="ghost" onClick={() => router.push('/')}>
            Go Back
          </Button>
        </div>
      ) : (
        <div className="grid gap-4">
          {testRuns.map((testRun) => (
            <div
              key={testRun.testRunId}
              className="group relative rounded-xl bg-gray-800/50 hover:bg-gray-800 transition-all p-6"
            >
              <div className="flex items-center justify-between">
                <div className="flex-1 min-w-0">
                  <h3 className="text-lg font-medium truncate">{testRun.title}</h3>
                  <div className="flex items-center gap-4 mt-2 text-sm text-gray-400">
                    <div className="flex items-center gap-1">
                      <svg
                        className="w-4 h-4"
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                        />
                      </svg>
                      <span>{format(new Date(testRun.runDate), 'MMM dd, yyyy HH:mm')}</span>
                    </div>
                    <span>•</span>
                    <span>{testRun.testCasesCount} test cases</span>
                  </div>
                </div>
                
                <div className="flex items-center gap-2 ml-4">
                  <Badge className={getStatusColor(testRun.testRunStatus?.testRunStatusName) || 'bg-gray-500/20 text-gray-400'}>
                    {testRun.testRunStatus?.testRunStatusName || 'Pending'}
                  </Badge>
                  <Dialog>
                    <DialogTrigger asChild>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setSelectedTestRun(testRun)}
                      >
                        Chi tiết
                      </Button>
                    </DialogTrigger>
                    <DialogContent className="bg-gray-800 text-white max-w-2xl">
                      <DialogHeader>
                        <DialogTitle className="text-xl">{selectedTestRun?.title}</DialogTitle>
                      </DialogHeader>
                      {selectedTestRun && (
                        <div className="grid gap-6 py-4">
                          <div className="space-y-2">
                            <h4 className="font-semibold text-blue-400">Mô tả</h4>
                            <p className="text-gray-300">{selectedTestRun.description || 'No description'}</p>
                          </div>
                          <div className="grid md:grid-cols-2 gap-6">
                            <div className="space-y-2">
                              <h4 className="font-semibold text-purple-400">Chi tiết</h4>
                              <div className="space-y-1 text-sm">
                                <p>
                                  <span className="text-gray-400">Ngày thực hiện:</span>{' '}
                                  {format(new Date(selectedTestRun.runDate), 'PPpp')}
                                </p>
                                <p>
                                  <span className="text-gray-400">Thực hiển bởi:</span>{' '}
                                  {selectedTestRun.executedBy}
                                </p>
                                <p>
                                  <span className="text-gray-400">Tạo bởi:</span>{' '}
                                  {selectedTestRun.createdByTeam?.teamName}
                                </p>
                              </div>
                            </div>
                            <div className="space-y-2">
                              <h4 className="font-semibold text-green-400">Thống kê</h4>
                              <div className="space-y-1 text-sm">
                                <p>
                                  <span className="text-gray-400">Tổng số Test Cases:</span>{' '}
                                  {selectedTestRun.testCasesCount}
                                </p>
                                <p>
                                  <span className="text-gray-400">Trạng thái:</span>{' '}
                                  <Badge className={getStatusColor(selectedTestRun.testRunStatus?.testRunStatusName) || 'bg-gray-500/20 text-gray-400'}>
                                    {selectedTestRun.testRunStatus?.testRunStatusName || 'Pending'}
                                  </Badge>
                                </p>
                              </div>
                            </div>
                          </div>
                        </div>
                      )}
                    </DialogContent>
                  </Dialog>
                  <EditTestRunDialog
                    testRun={testRun}
                    onTestRunUpdated={handleTestRunUpdated}
                  />
                  <TestRunDeleteDialog
                    testRun={testRun}
                    onTestRunDeleted={handleTestRunUpdated}
                  />
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}