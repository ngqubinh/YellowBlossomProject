'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { TestCase, EditTestCaseRequest, TestCaseStatus, TestType } from '@/types/test';
import { testService } from '@/services/testService';
import { Skeleton } from '@/components/ui/skeleton';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { TestCaseAddDialog } from '@/components/test/testcase-add-dialog';
import { Badge } from '@/components/ui/badge';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { TestCaseDeleteDialog } from '@/components/test/testcase-delete-dialog';

interface TestCaseEditDialogProps {
  testCase: TestCase;
  onTestCaseUpdated: () => void;
}

function TestCaseEditDialog({ testCase, onTestCaseUpdated }: TestCaseEditDialogProps) {
  const [formData, setFormData] = useState<EditTestCaseRequest>({
    title: testCase.title,
    description: testCase.description,
    steps: testCase.steps,
    expectedResult: testCase.expectedResult,
    actualResult: testCase.actualResult,
    testTypeId: testCase.testTypeId,
    testCaseStatusId: testCase.testCaseStatusId,
  });
  const [testTypes, setTestTypes] = useState<TestType[]>([]);
  const [testCaseStatuses, setTestCaseStatuses] = useState<TestCaseStatus[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isValidGuid = (value: string) => {
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    return guidRegex.test(value);
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [types, statuses] = await Promise.all([
          testService.getAllTestTypes(),
          testService.getAllTestCaseStatuses(),
        ]);
        setTestTypes(types);
        setTestCaseStatuses(statuses);
        console.log('Test Types:', types);
        console.log('Test Case Statuses:', statuses);

        if (statuses.length > 0) {
          const validStatus = statuses.find((status) => isValidGuid(status.testCaseStatusId));
          if (!testCase.testCaseStatusId || !isValidGuid(testCase.testCaseStatusId)) {
            setFormData((prev) => ({
              ...prev,
              testCaseStatusId: validStatus ? validStatus.testCaseStatusId : '',
            }));
            console.log('Set default testCaseStatusId to:', validStatus ? validStatus.testCaseStatusId : 'None');
          }
        }
      } catch (err) {
        setError('Không thể tải dữ liệu loại test hoặc trạng thái');
        console.error('Error fetching test types/statuses:', err);
      }
    };
    fetchData();
  }, [testCase.testCaseStatusId]);

  const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();
  setLoading(true);
  setError(null);

  if (!formData.testCaseStatusId || !isValidGuid(formData.testCaseStatusId)) {
    setError('Vui lòng chọn một trạng thái hợp lệ');
    setLoading(false);
    return;
  }

  try {
    // Pass formData directly without nesting
    await testService.editTestCase(testCase.testCaseId, formData);
    onTestCaseUpdated();
  } catch (err) {
    setError(err instanceof Error ? err.message : 'Không thể cập nhật test case');
    console.error('API Error:', err);
  } finally {
    setLoading(false);
  }
};

  const isButtonDisabled = loading || !formData.testCaseStatusId || !isValidGuid(formData.testCaseStatusId);

  return (
    <DialogContent className="bg-gray-800 text-white max-w-2xl max-h-[80vh] overflow-y-auto">
      <DialogHeader>
        <DialogTitle className="text-xl">Chỉnh sửa Test Case</DialogTitle>
      </DialogHeader>
      <form onSubmit={handleSubmit} className="grid gap-6 py-4">
        {error && <p className="text-red-400">{error}</p>}
        <div className="space-y-2">
          <Label htmlFor="title" className="text-blue-400">Tiêu đề</Label>
          <Input
            id="title"
            value={formData.title}
            onChange={(e) => setFormData({ ...formData, title: e.target.value })}
            className="bg-gray-700 text-white border-gray-600"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="description" className="text-blue-400">Mô tả</Label>
          <Textarea
            id="description"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            className="bg-gray-700 text-white border-gray-600"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="steps" className="text-blue-400">Các bước</Label>
          <Textarea
            id="steps"
            value={formData.steps}
            onChange={(e) => setFormData({ ...formData, steps: e.target.value })}
            className="bg-gray-700 text-white border-gray-600"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="expectedResult" className="text-green-400">Kết quả mong đợi</Label>
          <Textarea
            id="expectedResult"
            value={formData.expectedResult}
            onChange={(e) => setFormData({ ...formData, expectedResult: e.target.value })}
            className="bg-gray-700 text-white border-gray-600"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="actualResult" className="text-purple-400">Kết quả thực tế</Label>
          <Textarea
            id="actualResult"
            value={formData.actualResult}
            onChange={(e) => setFormData({ ...formData, actualResult: e.target.value })}
            className="bg-gray-700 text-white border-gray-600"
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="testTypeId" className="text-blue-400">Loại Test</Label>
          <select
            id="testTypeId"
            value={formData.testTypeId}
            onChange={(e) => setFormData({ ...formData, testTypeId: e.target.value })}
            className="bg-gray-700 text-white border-gray-600 w-64 p-2 rounded"
          >
            {testTypes.length === 0 ? (
              <option value="" disabled>Không có loại test nào</option>
            ) : (
              <>
                <option value="" disabled>Chọn loại test</option>
                {testTypes.map((type) => (
                  <option key={type.testTypeId} value={type.testTypeId}>
                    {type.testTypeName}
                  </option>
                ))}
              </>
            )}
          </select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="testCaseStatusId" className="text-blue-400">Trạng thái</Label>
          <select
            id="testCaseStatusId"
            value={formData.testCaseStatusId}
            onChange={(e) => {
              console.log('Selected testCaseStatusId:', e.target.value);
              setFormData({ ...formData, testCaseStatusId: e.target.value });
            }}
            className="bg-gray-700 text-white border-gray-600 w-64 p-2 rounded"
          >
            {testCaseStatuses.length === 0 ? (
              <option value="" disabled>Không có trạng thái nào</option>
            ) : (
              <>
                <option value="" disabled>Chọn trạng thái</option>
                {testCaseStatuses.map((status) => (
                  <option key={status.testCaseStatusId} value={status.testCaseStatusId}>
                    {status.testCaseStatusName}
                  </option>
                ))}
              </>
            )}
          </select>
        </div>
        <Button
          type="submit"
          disabled={isButtonDisabled}
          className="bg-blue-600 hover:bg-blue-700"
        >
          {loading ? 'Đang lưu...' : 'Lưu thay đổi'}
        </Button>
      </form>
    </DialogContent>
  );
}

export default function TestDetailsPage({ params }: { params: { taskId: string } }) {
  const [testCases, setTestCases] = useState<TestCase[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedTestCase, setSelectedTestCase] = useState<TestCase | null>(null);
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);
  const router = useRouter();

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await testService.getRelatedTestCases(params.taskId);
        setTestCases(data);
        setError(null);
      } catch (error) {
        setError(error instanceof Error ? error.message : 'Không thể tải test cases');
      } finally {
        setLoading(false);
      }
    };

    loadData();
  }, [params.taskId]);

  const handleTestCaseCreated = () => {
    testService.getRelatedTestCases(params.taskId).then(setTestCases).catch(console.error);
  };

  const handleTestCaseUpdated = () => {
    testService.getRelatedTestCases(params.taskId).then(setTestCases).catch(console.error);
    setIsEditDialogOpen(false); // Close the dialog after update
  };

  const handleTestCaseDeleted = () => {
    testService.getRelatedTestCases(params.taskId).then(setTestCases).catch(console.error);
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
            <Skeleton key={i} className="h-20 w-full bg-gray-800 rounded-xl" />
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
          <h2 className="text-xl font-semibold text-red-100 mb-2">Lỗi</h2>
          <p className="text-red-400 mb-6">{error}</p>
          <Button
            onClick={() => router.refresh()}
            variant="destructive"
          >
            Thử lại
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
            Test Cases
          </h1>
          <p className="text-gray-400 mt-1">Task ID: {params.taskId}</p>
        </div>
        <TestCaseAddDialog
          taskId={params.taskId}
          onTestCaseCreated={handleTestCaseCreated}
        />
      </div>

      {testCases.length === 0 ? (
        <div className="rounded-xl border-2 border-dashed border-gray-700 p-8 text-center">
          <p className="text-gray-400 mb-4">Không tìm thấy test case nào</p>
          <Button variant="ghost" onClick={() => router.push('/')}>
            Quay lại
          </Button>
        </div>
      ) : (
        <div className="grid gap-4">
          {testCases.map((testCase) => (
            <div
              key={testCase.testCaseId}
              className="group relative rounded-xl bg-gray-800/50 hover:bg-gray-800 transition-all p-6"
            >
              <div className="flex items-center justify-between">
                <div className="flex-1 min-w-0">
                  <h3 className="text-lg font-medium truncate">{testCase.title}</h3>
                  <p className="text-sm text-gray-400 mt-1 truncate">
                    {testCase.description}
                  </p>
                </div>
                
                <div className="flex items-center gap-4 ml-4">
                  <Badge
                    className={
                      testCase.testCaseStatusId === 'cd4504c3-cd8a-4892-947e-f1ac62146566'
                        ? 'bg-green-500/20 text-green-400'
                        : 'bg-red-500/20 text-red-400'
                    }
                  >
                    {testCase.testCaseStatusId === 'cd4504c3-cd8a-4892-947e-f1ac62146566'
                      ? 'Passed'
                      : 'Failed'}
                  </Badge>
                  <Dialog>
                    <DialogTrigger asChild>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => setSelectedTestCase(testCase)}
                      >
                        Chi tiết
                      </Button>
                    </DialogTrigger>
                    <DialogContent className="bg-gray-800 text-white max-w-2xl">
                      <DialogHeader>
                        <DialogTitle className="text-xl">{selectedTestCase?.title}</DialogTitle>
                      </DialogHeader>
                      {selectedTestCase && (
                        <div className="grid gap-6 py-4">
                          <div className="space-y-2">
                            <h4 className="font-semibold text-blue-400">Mô tả</h4>
                            <p className="text-gray-300">{selectedTestCase.description}</p>
                          </div>
                          
                          <div className="space-y-2">
                            <h4 className="font-semibold text-blue-400">Các bước</h4>
                            <div className="prose prose-invert">
                              {selectedTestCase.steps.split('\n').map((step, i) => (
                                <div key={i} className="flex gap-2">
                                  <span className="text-gray-400">{i + 1}.</span>
                                  <span className="text-gray-300">{step}</span>
                                </div>
                              ))}
                            </div>
                          </div>

                          <div className="grid md:grid-cols-2 gap-6">
                            <div className="space-y-2">
                              <h4 className="font-semibold text-green-400">Mong đợi</h4>
                              <p className="text-gray-300">{selectedTestCase.expectedResult}</p>
                            </div>
                            <div className="space-y-2">
                              <h4 className="font-semibold text-purple-400">Thực tế</h4>
                              <p className="text-gray-300">{selectedTestCase.actualResult}</p>
                            </div>
                          </div>
                        </div>
                      )}
                    </DialogContent>
                  </Dialog>
                  <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
                    <DialogTrigger asChild>
                      <Button
                        variant="outline"
                        size="sm"
                        onClick={() => {
                          setSelectedTestCase(testCase);
                          setIsEditDialogOpen(true);
                        }}
                      >
                        Sửa
                      </Button>
                    </DialogTrigger>
                    {selectedTestCase && (
                      <TestCaseEditDialog
                        testCase={selectedTestCase}
                        onTestCaseUpdated={handleTestCaseUpdated}
                      />
                    )}
                  </Dialog>
                  {/* Thêm nút xóa vào đây */}
                  <TestCaseDeleteDialog
                    testCase={testCase}
                    onTestCaseDeleted={handleTestCaseDeleted}
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