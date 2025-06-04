"use client";

import { useEffect, useState } from "react";
import { testService } from "@/services/testService";
import { Skeleton } from "@/components/ui/skeleton";
import { TaskTest, TestCaseStatus } from "@/types/test";
import { formatDistanceToNow } from "date-fns";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";

export default function DoneTasksPage() {
  const [tasks, setTasks] = useState<TaskTest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [selectedTestCase, setSelectedTestCase] = useState<{
    testRunId: string;
    testCaseId: string;
  } | null>(null);
  const [actualResult, setActualResult] = useState("");
  const [selectedStatusId, setSelectedStatusId] = useState("");
  const [testCaseStatuses, setTestCaseStatuses] = useState<TestCaseStatus[]>([]);
  const [selectedTask, setSelectedTask] = useState<TaskTest | null>(null);

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        const data = await testService.getAllTestForTester();
        setTasks(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Không tải được task");
      } finally {
        setLoading(false);
      }
    };

    fetchTasks();
  }, []);

  useEffect(() => {
    const fetchStatuses = async () => {
      try {
        const statuses = await testService.getAllTestCaseStatuses();
        setTestCaseStatuses(statuses);
      } catch (error) {
        console.error("Failed to fetch test case statuses:", error);
      }
    };
    fetchStatuses();
  }, []);
  

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto p-6 mt-8">
        <h1 className="text-3xl font-bold mb-8 text-white">Task của Tester</h1>
        <div className="grid gap-4">
          {[...Array(5)].map((_, i) => (
            <Skeleton
              key={i}
              className="h-20 w-full bg-gray-700 rounded-xl"
            />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-6xl mx-auto p-6 mt-8">
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
          <h2 className="text-xl font-semibold text-red-100 mb-2">
            Không tải được task
          </h2>
          <p className="text-red-400 mb-6 max-w-md">{error}</p>
          <Button
            onClick={() => window.location.reload()}
            variant="destructive"
          >
            Thử lại
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-6 mt-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold text-white">Task của Tester</h1>
        <Badge className="border-blue-400/30 text-blue-300">
          {tasks.length} task
        </Badge>
      </div>

      {tasks.length === 0 ? (
        <div className="rounded-xl border-2 border-dashed border-gray-700 p-12 text-center">
          <div className="text-gray-400">Bạn chưa có task nào</div>
        </div>
      ) : (
        <div className="grid gap-4">
          {tasks.map((task) => (
            <div
              key={task.taskId}
              className="rounded-xl bg-gray-800/50 p-6 grid grid-cols-1 md:grid-cols-2 gap-6"
            >
              {/* Left column: Task, TestCase, TestRunDTO */}
              <div>
                <div className="flex items-center gap-2 mb-2">
                  <h3 className="text-lg font-semibold text-white">
                    {task.title}
                  </h3>
                  {task.taskStatus && (
                    <Badge className="bg-blue-400/10 text-blue-300">
                      {task.taskStatus.taskStatusName}
                    </Badge>
                  )}
                </div>
                <p className="text-gray-300 mb-4">{task.description}</p>
                <div className="text-sm text-gray-400 mb-2">
                  <span>
                    Hoàn thành{" "}
                    {formatDistanceToNow(new Date(task.endDate), {
                      addSuffix: true,
                    })}
                  </span>
                </div>
                {task.testCase && task.testCase.length > 0 && (
                  <div className="mb-4">
                    <h4 className="text-sm font-semibold text-white mb-2">
                      Test Case
                    </h4>
                    <ul className="list-disc list-inside text-sm text-gray-400">
                      {task.testCase.map((testCase) => (
                        <li key={testCase.testCaseId}>{testCase.title}</li>
                      ))}
                    </ul>
                  </div>
                )}
                {task.testRunDTO && task.testRunDTO.length > 0 && (
                  <div>
                    <h4 className="text-sm font-semibold text-white mb-2">
                      Test Run
                    </h4>
                    <ul className="list-disc list-inside text-sm text-gray-400">
                      {task.testRunDTO.map((testRun) => (
                        <li key={testRun.testRunId}>{testRun.title}</li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>

              {/* Right column: TestRunTestCase */}
              <div className="border-l border-gray-600 pl-6 relative">
                <h4 className="text-sm font-semibold text-white mb-2">
                  Test Run Test Cases
                </h4>
                
                {task.testRunTestCases && task.testRunTestCases.length > 0 ? (
                  <div className="space-y-4">
                    {task.testRunTestCases.map((testRunTestCase) => (
                      <div 
                        key={`${testRunTestCase.testRunId}-${testRunTestCase.testCaseId}`}
                        className="rounded-lg bg-gray-700/50 p-4"
                      >
                        <div className="grid grid-cols-2 gap-2 mb-2">
                          <span className="text-gray-400">Trạng thái:</span>
                          <span className="text-white">
                            {testRunTestCase.testCaseStatus.testCaseStatusName}
                          </span>
                          <span className="text-gray-400">Kết quả:</span>
                          <span className="text-white">
                            {testRunTestCase.actualResult}
                          </span>
                        </div>
                      </div>
                    ))}
                    
                    {/* Add button container at the bottom right */}
                    <div className="flex justify-end mt-6">
                      <Button
                      onClick={() => {
  if (task.testRunTestCases?.length) {
    const firstResult = task.testRunTestCases[0];
    
    setSelectedTestCase({
      testRunId: firstResult.testRunId,
      testCaseId: firstResult.testCaseId,
    });
    setActualResult(firstResult.actualResult || "");
    
    // Tìm status ID dựa trên tên hiện tại
    const currentStatusName = firstResult.testCaseStatus.testCaseStatusName;
    const matchingStatus = testCaseStatuses.find(
      status => status.testCaseStatusName === currentStatusName
    );
    
    if (matchingStatus) {
      setSelectedStatusId(matchingStatus.testCaseStatudId);
    } else {
      // Fallback to first status if not found
      if (testCaseStatuses.length > 0) {
        setSelectedStatusId(testCaseStatuses[0].testCaseStatudId);
      }
    }
    
    setIsDialogOpen(true);
  }
}}
                    >
                      Cập nhật kết quả
                    </Button>
                    </div>
                  </div>
                ) : (
                  <div className="flex flex-col items-end">
                    <p className="text-gray-400 mb-4">Không có Test Run Test Case</p>
                    <Button>
                      Cập nhật kết quả
                    </Button>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
       <DialogContent>
  <DialogHeader>
    <DialogTitle>Cập nhật kết quả kiểm thử</DialogTitle>
  </DialogHeader>
  
  <div className="grid gap-4 py-4">
    <div className="grid grid-cols-4 items-center gap-4">
      <Label htmlFor="actualResult" className="text-right">
        Kết quả thực tế
      </Label>
      <Input
        id="actualResult"
        value={actualResult}
        onChange={(e) => setActualResult(e.target.value)}
        className="col-span-3"
      />
    </div>
    
    <div className="grid grid-cols-4 items-center gap-4">
      <Label htmlFor="status" className="text-right">
        Trạng thái
      </Label>
      <select
  id="status"
  value={selectedStatusId} // Lưu ID chứ không phải tên
  onChange={(e) => setSelectedStatusId(e.target.value)}
  className="col-span-3 rounded-md bg-gray-800 p-2 text-white"
>
  {testCaseStatuses.map((status) => (
    <option 
      key={status.testCaseStatudId}
      value={status.testCaseStatudId} // Truyền ID vào value
    >
      {status.testCaseStatusName} {/* Hiển thị tên */}
    </option>
  ))}
</select>
    </div>
  </div>

  <DialogFooter>
    <Button 
      variant="outline" 
      onClick={() => setIsDialogOpen(false)}
    >
      Hủy
    </Button>
    <Button
     onClick={async () => {
  if (selectedTestCase) {
    // Validate before sending
    if (!selectedStatusId) {
      console.error("No status ID selected");
      return;
    }
    
    // Validate GUID format
    const guidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    if (!guidRegex.test(selectedStatusId)) {
      console.error("Invalid GUID format:", selectedStatusId);
      console.log("Available statuses:", testCaseStatuses);
      return;
    }
    
    console.log("Sending update with:", {
      testCaseId: selectedTestCase.testCaseId,
      testRunId: selectedTestCase.testRunId,
      actualResult: actualResult,
      testCaseStatusId: selectedStatusId
    });
    
    try {
      await testService.updateResult(
        selectedTestCase.testCaseId,
        selectedTestCase.testRunId,
        {
          actualResult: actualResult || ' ',
          testCaseStatusId: selectedStatusId,
        }
      );
      
      // Refresh data
      const updatedTasks = await testService.getAllTestForTester();
      setTasks(updatedTasks);
      setIsDialogOpen(false);
    } catch (error) {
      console.error("Update failed:", error);
    }
  }
}}
    >
      Xác nhận
    </Button>
  </DialogFooter>
</DialogContent>
      </Dialog>
    </div>
  );
}