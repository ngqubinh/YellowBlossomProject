"use client";

import { useEffect, useState } from "react";
import { testService } from "@/services/testService";
import { Skeleton } from "@/components/ui/skeleton";
import { Task } from "@/types/test";
import { useRouter } from "next/navigation";
import { formatDistanceToNow } from "date-fns";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";

export default function DoneTasksPage() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const [hoveredTask, setHoveredTask] = useState<string | null>(null);

  useEffect(() => {
    const fetchDoneTasks = async () => {
      try {
        const data = await testService.getDoneTasks();
        setTasks(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load tasks");
      } finally {
        setLoading(false);
      }
    };

    fetchDoneTasks();
  }, []);

  const handleTaskHover = (taskId: string) => {
    setHoveredTask(taskId);
  };

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto p-6 mt-8">
        <h1 className="text-3xl font-bold mb-8 bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">
          Công việc đã hoàn thành
        </h1>
        <div className="grid gap-4">
          {[...Array(5)].map((_, i) => (
            <Skeleton 
              key={i}
              className="h-20 w-full bg-gradient-to-r from-gray-800 to-gray-700 rounded-xl animate-pulse"
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
            Thất bại khi load công việc
          </h2>
          <p className="text-red-400 mb-6 max-w-md">{error}</p>
          <Button
            onClick={() => window.location.reload()}
            variant="destructive"
          >
            <svg
              className="h-4 w-4 mr-2"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
              />
            </svg>
            Thử lại
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-6 mt-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">
          Công việc đã hoàn thành
        </h1>
        <Badge variant="outline" className="border-blue-400/30 text-blue-300">
          {tasks.length} công việc
        </Badge>
      </div>

      {tasks.length === 0 ? (
        <div className="rounded-xl border-2 border-dashed border-gray-700 p-12 text-center">
          <div className="text-gray-400 mb-4">Không có công việc đã hoàn thành</div>
          <Button onClick={() => router.push("/")} variant="ghost">
            Tạo công việc
          </Button>
        </div>
      ) : (
        <div className="grid gap-4">
          {tasks.map((task) => (
            <div
              key={task.taskId}
              className="group relative rounded-xl bg-gray-800/50 hover:bg-gray-800 transition-all p-6 pr-32"
              onMouseEnter={() => handleTaskHover(task.taskId)}
              onMouseLeave={() => setHoveredTask(null)}
            >
              <div className="flex flex-col gap-2">
                <div className="flex items-center gap-3">
                  <svg
                    className="h-5 w-5 text-green-400"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                  <Tooltip>
                    <TooltipTrigger>
                      <h3 className="text-lg font-semibold truncate max-w-[200px]">
                        {task.title}
                      </h3>
                    </TooltipTrigger>
                    <TooltipContent>{task.title}</TooltipContent>
                  </Tooltip>
                  <Badge className="ml-2 bg-purple-400/10 text-purple-300">
                    #{task.taskId.slice(0, 6)}
                  </Badge>
                </div>

                <div className="flex items-center gap-4 text-sm text-gray-400">
                  <div className="flex items-center gap-1">
                    <svg
                      className="h-4 w-4"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                    <span>
                      Hoàn thành{" "}
                      {formatDistanceToNow(new Date(task.endDate), {
                        addSuffix: true
                      })}
                    </span>
                  </div>
                </div>
              </div>

              <div className="absolute right-6 top-1/2 -translate-y-1/2 flex gap-3 opacity-0 group-hover:opacity-100 transition-opacity">
                <Button
                  size="sm"
                  onClick={() => router.push(`/test-cases/${task.taskId}`)}
                  variant="outline"
                  className="border-blue-400/30 text-blue-300 hover:bg-blue-400/10"
                >
                  Xem Test Cases
                </Button>
                <Button
                  size="sm"
                  onClick={() => router.push(`/test-runs/${task.taskId}`)}
                  className="bg-blue-400/10 hover:bg-blue-400/20 text-blue-300"
                >
                  Xem Test Runs
                </Button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}