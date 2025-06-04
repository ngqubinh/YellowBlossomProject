"use client";

import { useEffect, useState } from "react";
import { bugService } from "@/services/bugService";
import { Skeleton } from "@/components/ui/skeleton";
import { formatDistanceToNow } from "date-fns";
import { Badge } from "@/components/ui/badge";
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip";
import { Bug, ResolveBugRequest } from "@/types/bug";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

export default function BugsPage() {
  const [bugs, setBugs] = useState<Bug[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [resolveDialogOpen, setResolveDialogOpen] = useState(false);
  const [selectedBug, setSelectedBug] = useState<Bug | null>(null);
  const [formData, setFormData] = useState<ResolveBugRequest>({
    StepsToReduce: "",
    Serverity: ""
  });
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    const fetchBugs = async () => {
      try {
        const data = await bugService.getAllBugs();
        setBugs(data);
        setError(null);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Failed to load bugs");
      } finally {
        setLoading(false);
      }
    };

    fetchBugs();
  }, []);

  const handleOpenDialog = (bug: Bug) => {
    setSelectedBug(bug);
    setFormData({
      StepsToReduce: bug.stepsToReduces,
      Serverity: bug.serverity
    });
    setResolveDialogOpen(true);
  };

  const handleSubmit = async () => {
    if (!selectedBug) return;
    
    setSubmitting(true);
    try {
      const resolvedBug = await bugService.resolveBug(
        selectedBug.bugId,
        formData
      );
      
      setBugs(prev => prev.map(bug => 
        bug.bugId === selectedBug.bugId ? resolvedBug : bug
      ));
      setResolveDialogOpen(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to resolve bug");
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="max-w-6xl mx-auto p-6 mt-8">
        <h1 className="text-3xl font-bold mb-8 bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">
          Reported Bugs
        </h1>
        <div className="grid gap-4">
          {[...Array(5)].map((_, i) => (
            <Skeleton 
              key={i}
              className="h-32 w-full bg-gradient-to-r from-gray-800 to-gray-700 rounded-xl animate-pulse"
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
            Failed to load bugs
          </h2>
          <p className="text-red-400 mb-6 max-w-md">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto p-6 mt-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold bg-gradient-to-r from-blue-400 to-purple-500 bg-clip-text text-transparent">
          Reported Bugs
        </h1>
        <Badge variant="outline" className="border-blue-400/30 text-blue-300">
          {bugs.length} bugs
        </Badge>
      </div>

      {bugs.length === 0 ? (
        <div className="rounded-xl border-2 border-dashed border-gray-700 p-12 text-center">
          <div className="text-gray-400 mb-4">No bugs reported yet</div>
        </div>
      ) : (
        <div className="grid gap-4">
          {bugs.map((bug) => (
            <div
              key={bug.bugId}
              className="group relative rounded-xl bg-gray-800/50 p-6"
            >
              <div className="flex flex-col gap-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <svg
                      className="h-5 w-5 text-red-400"
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
                    <Tooltip>
                      <TooltipTrigger>
                        <h3 className="text-lg font-semibold truncate max-w-[200px]">
                          {bug.title}
                        </h3>
                      </TooltipTrigger>
                      <TooltipContent>{bug.title}</TooltipContent>
                    </Tooltip>
                    <Badge className="ml-2 bg-purple-400/10 text-purple-300">
                      #{bug.bugId.slice(0, 6)}
                    </Badge>
                  </div>
                  <Button
                    size="sm"
                    onClick={() => handleOpenDialog(bug)}
                    className="bg-orange-600 hover:bg-orange-700 text-white"
                  >
                    Cập nhật
                  </Button>
                </div>

                <div className="flex flex-wrap items-center gap-4 text-sm text-gray-400">
                  <Badge variant={bug.serverity?.includes("High") ? "destructive" : "secondary"}>
                    {bug.serverity}
                  </Badge>
                  
                  <Badge variant={bug.resolvedDate ? "default" : "outline"}>
                    {bug.resolvedDate ? "Resolved" : "Unresolved"}
                  </Badge>

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
                      Reported{" "}
                      {formatDistanceToNow(new Date(bug.reportedDate), {
                        addSuffix: true
                      })}
                    </span>
                  </div>

                  {bug.priority && (
                    <div className="flex items-center gap-1">
                      <span className="text-gray-500">Priority:</span>
                      <Badge variant="outline" className="border-green-400/30 text-green-300">
                        {bug.priority.priorityName}
                      </Badge>
                    </div>
                  )}

                  {bug.testRun && (
                    <div className="flex items-center gap-1">
                      <span className="text-gray-500">Test Run:</span>
                      <span className="text-blue-300">{bug.testRun.title}</span>
                    </div>
                  )}

                  {bug.reportedByTeam && (
                    <div className="flex items-center gap-1">
                      <span className="text-gray-500">Team:</span>
                      <span className="text-purple-300">{bug.reportedByTeam.teamName}</span>
                    </div>
                  )}
                </div>

                <p className="text-gray-300 text-sm mt-2 line-clamp-2">
                  {bug.description}
                </p>
              </div>
            </div>
          ))}
        </div>
      )}

      <Dialog open={resolveDialogOpen} onOpenChange={setResolveDialogOpen}>
        <DialogContent className="sm:max-w-[500px] bg-gray-800 border-gray-700">
          <DialogHeader>
            <DialogTitle className="text-gray-100">
              Resolve Bug: {selectedBug?.title}
            </DialogTitle>
          </DialogHeader>

          <div className="grid gap-4 py-4">
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="steps" className="text-right text-gray-300">
                Steps to Reduce
              </Label>
              <Input
                id="steps"
                value={formData.StepsToReduce}
                onChange={(e) => setFormData(prev => ({
                  ...prev,
                  StepsToReduce: e.target.value
                }))}
                className="col-span-3 bg-gray-700 border-gray-600 text-gray-100"
              />
            </div>
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="severity" className="text-right text-gray-300">
                Severity
              </Label>
              <Input
                id="severity"
                value={formData.Serverity}
                onChange={(e) => setFormData(prev => ({
                  ...prev,
                  Serverity: e.target.value
                }))}
                className="col-span-3 bg-gray-700 border-gray-600 text-gray-100"
              />
            </div>
          </div>

          <DialogFooter>
            <Button 
              onClick={() => setResolveDialogOpen(false)}
              variant="outline"
              className="text-gray-100 border-gray-600 hover:bg-gray-700"
            >
              Hủy
            </Button>
            <Button 
              onClick={handleSubmit}
              disabled={submitting}
              className="bg-blue-600 hover:bg-blue-700 text-white"
            >
              {submitting ? "Processing..." : "Xác nhận"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}