'use client';

import { useState, useEffect } from 'react';
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { useAuth } from "@/components/providers/AuthProvider";
import { Button } from "@/components/ui/button";
import { Loader2, MoreHorizontal, Bug, AlertTriangle, CheckCircle } from "lucide-react";
import { productService } from '@/services/productService';
import { DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem } from "@/components/ui/dropdown-menu";
import Link from 'next/link';
import { RoleGuard } from '@/context/RoleGuardProps ';
import { Badge } from "@/components/ui/badge";

// Import your existing types
import { ProductStatistic } from '@/types/product';

// Define Bug type to match actual API response
export type Bug = {
  bugId: string;
  title: string;
  description: string;
  stepsToReduces: string;
  serverity: string;
  reportedDate: string;
  resolvedDate: string | null;
  priorityId: string;
  testRunId: string;
  testCaseId: string;
  reportedByTeamId: string;
  assginedToTeamId: string | null;
  priority?: Priority;
  testRun?: TestRun;
  testCase?: TestCase;
  reportedByTeam?: ReportedByTeam;
  assignedToTeam?: ReportedByTeam;
};

export type Priority = {
  priorityId: string;
  priorityName: string;
};

export type TestRun = {
  testRunId: string;
  title: string;
  bugs?: Bug[];
};

export type TestCase = {
  testCaseId: string;
  title: string;
  bugs?: Bug[];
};

export type ReportedByTeam = {
  teamId: string;
  teamName: string;
};

// Extended interfaces to handle nested structure from your API
interface ExtendedProductStatistic extends ProductStatistic {
  projects?: Array<{
    projectId: string;
    projectName: string;
    description: string;
    startDate: string;
    endDate: string;
    projectStatusDTO: {
      projectStatusId: string;
      projectStatusName: string;
    };
    tasks?: Array<{
      taskId: string;
      title: string;
      description: string;
      startDate: string;
      endDate: string;
      taskStatus: {
        taskStatusId: string;
        taskStatusName: string;
      };
      testCases?: TestCase[];
      testRuns?: TestRun[];
    }>;
  }>;
}

export default function DashboardPage() {
  const { user, logout, isLoading } = useAuth();
  const [products, setProducts] = useState<ExtendedProductStatistic[]>([]);
  const [productsLoading, setProductsLoading] = useState(true);
  const [productsError, setProductsError] = useState<string | null>(null);
  const [expandedProducts, setExpandedProducts] = useState<Set<string>>(new Set());

  // Fetch product statistics
  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await productService.getProductStatistics();
        // Type assertion to handle the extended structure
        setProducts(data as ExtendedProductStatistic[]);
        setProductsLoading(false);
      } catch (err) {
        setProductsError((err as Error).message);
        setProductsLoading(false);
      }
    };
    fetchData();
  }, []);

  // Helper function to get all bugs from a product
  const getBugsFromProduct = (product: ExtendedProductStatistic): Bug[] => {
    const bugs: Bug[] = [];
    if (product.projects) {
      product.projects.forEach(project => {
        if (project.tasks) {
          project.tasks.forEach(task => {
            if (task.testCases) {
              task.testCases.forEach(testCase => {
                if (testCase.bugs) {
                  bugs.push(...testCase.bugs);
                }
              });
            }
            if (task.testRuns) {
              task.testRuns.forEach(testRun => {
                if (testRun.bugs) {
                  bugs.push(...testRun.bugs);
                }
              });
            }
          });
        }
      });
    }
    return bugs;
  };

  // Helper function to get bug statistics
  const getBugStats = (bugs: Bug[]) => {
    const resolved = bugs.filter(bug => bug.resolvedDate !== null).length;
    const open = bugs.length - resolved;
    const critical = bugs.filter(bug => 
      bug.serverity.toLowerCase().includes('high') || 
      bug.serverity.toLowerCase().includes('critical')
    ).length;
    
    return { total: bugs.length, resolved, open, critical };
  };

  const toggleProductExpansion = (productId: string) => {
    const newExpanded = new Set(expandedProducts);
    if (newExpanded.has(productId)) {
      newExpanded.delete(productId);
    } else {
      newExpanded.add(productId);
    }
    setExpandedProducts(newExpanded);
  };

  if (isLoading || productsLoading) {
    return (
      <div className="flex h-[calc(100vh-64px)] items-center justify-center">
        <Loader2 className="h-10 w-10 animate-spin text-primary" />
      </div>
    );
  }

  if (productsError) {
    return <div>Error: {productsError}</div>;
  }

  const statusColors: { [key: string]: string } = {
    NotStarted: "bg-yellow-500",
    Completed: "bg-blue-500",
    OnHold: "bg-gray-500",
    Delayed: "bg-red-500",
    AtRisk: "bg-orange-500",
    Cancelled: "bg-purple-500",
    InProgress: "bg-green-500",
  };

  return (
    <RoleGuard requiredRoles={'ADMIN'}>
      <div className="container py-10">
        <h1 className="mb-8 text-3xl font-bold">Dashboard</h1>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card>
            <CardHeader>
              <CardTitle>Chào mừng trở lại</CardTitle>
              <CardDescription>Bảng điều khiển cá nhân của bạn</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <p><strong>Tên:</strong> {user?.fullName}</p>
                <p><strong>Email:</strong> {user?.email}</p>
                <p><strong>ID:</strong> {user?.id}</p>
              </div>
            </CardContent>
            <CardFooter>
              <Button variant="outline" onClick={logout}>Đăng xuất</Button>
            </CardFooter>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Thống kê</CardTitle>
              <CardDescription>Tổng quan hoạt động của bạn</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div className="flex justify-between">
                  <span>Dự án</span>
                  <span className="font-medium">7</span>
                </div>
                <div className="flex justify-between">
                  <span>Hoàn thành</span>
                  <span className="font-medium">4</span>
                </div>
                <div className="flex justify-between">
                  <span>Đang tiến hành</span>
                  <span className="font-medium">3</span>
                </div>
              </div>
            </CardContent>
            <CardFooter>
              <Button variant="ghost" className="w-full">Xem tất cả</Button>
            </CardFooter>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Hoạt động gần đây</CardTitle>
              <CardDescription>Cập nhật mới nhất</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="border-b pb-2">
                  <p className="text-sm font-medium">Dự án đã cập nhật</p>
                  <p className="text-xs text-muted-foreground">3 giờ trước</p>
                </div>
                <div className="border-b pb-2">
                  <p className="text-sm font-medium">Nhận được bình luận mới</p>
                  <p className="text-xs text-muted-foreground">Hôm qua</p>
                </div>
                <div>
                  <p className="text-sm font-medium">Nhiệm vụ đã hoàn thành</p>
                  <p className="text-xs text-muted-foreground">2 ngày trước</p>
                </div>
              </div>
            </CardContent>
            <CardFooter>
              <Button variant="ghost" className="w-full">Xem lịch sử</Button>
            </CardFooter>
          </Card>
        </div>

        {/* Thống kê sản phẩm */}
        <div className="mt-10">
          <h2 className="text-2xl font-bold mb-4">Thống kê sản phẩm</h2>
          <div className="space-y-6">
            {products.map((product) => {
              const total = product.totalProjects;
              const bugs = getBugsFromProduct(product);
              const bugStats = getBugStats(bugs);
              const isExpanded = expandedProducts.has(product.productId);

              return (
                <Card key={product.productId}>
                  <CardHeader className="bg-muted/50">
                    <div className="flex justify-between items-center">
                      <CardTitle className="flex items-center gap-4">
                        {product.productName}
                        <span className="text-sm text-muted-foreground">
                          Lần cập nhật cuối: {new Date(product.lastUpdated).toLocaleString()}
                        </span>
                      </CardTitle>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" size="icon">
                            <MoreHorizontal className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent>
                          <DropdownMenuItem asChild>
                            <Link href="/product">Danh sách sản phẩm</Link>
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </div>
                  </CardHeader>

                  <CardContent className="pt-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                      {/* Project Statistics */}
                      <div>
                        <div className="mb-4">
                          <span className="font-medium">Tổng số dự án: </span>
                          {total}
                        </div>

                        {total > 0 && (
                          <div className="flex items-start gap-6">
                            <div className="h-32 w-8 flex flex-col-reverse overflow-hidden rounded bg-gray-200">
                              {Object.entries(product.projectStatusDistribution).map(([status, count]) => (
                                count > 0 && (
                                  <div
                                    key={status}
                                    className={statusColors[status] || "bg-gray-300"}
                                    style={{ height: `${(count / total) * 100}%` }}
                                  />
                                )
                              ))}
                            </div>

                            <div className="flex flex-col gap-2">
                              {Object.entries(product.projectStatusDistribution).map(([status, count]) => (
                                <div key={status} className="text-sm">
                                  <span className={`inline-block w-2 h-2 ${statusColors[status] || "bg-gray-300"} rounded-full mr-2`} />
                                  {status}: {count}
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </div>

                      {/* Bug Statistics */}
                      <div>
                        <div className="mb-4 flex items-center gap-2">
                          <Bug className="h-4 w-4" />
                          <span className="font-medium">Thống kê lỗi</span>
                        </div>
                        
                        <div className="space-y-2">
                          <div className="flex justify-between items-center">
                            <span className="text-sm">Tổng số lỗi:</span>
                            <Badge variant="outline">{bugStats.total}</Badge>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm flex items-center gap-1">
                              <AlertTriangle className="h-3 w-3 text-red-500" />
                              Đang mở:
                            </span>
                            <Badge variant="destructive">{bugStats.open}</Badge>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm flex items-center gap-1">
                              <CheckCircle className="h-3 w-3 text-green-500" />
                              Đã giải quyết:
                            </span>
                            <Badge variant="secondary">{bugStats.resolved}</Badge>
                          </div>
                          <div className="flex justify-between items-center">
                            <span className="text-sm">Nghiêm trọng:</span>
                            <Badge variant="destructive">{bugStats.critical}</Badge>
                          </div>
                        </div>

                        {bugs.length > 0 && (
                          <Button 
                            variant="outline" 
                            size="sm" 
                            className="mt-3 w-full"
                            onClick={() => toggleProductExpansion(product.productId)}
                          >
                            {isExpanded ? 'Ẩn chi tiết lỗi' : 'Xem chi tiết lỗi'}
                          </Button>
                        )}
                      </div>
                    </div>

                    {/* Detailed Bug Information */}
                    {isExpanded && bugs.length > 0 && (
                      <div className="mt-6 border-t pt-6">
                        <h4 className="font-semibold mb-4">Chi tiết lỗi</h4>
                        <div className="space-y-4 max-h-96 overflow-y-auto">
                          {bugs.map((bug) => (
                            <Card key={bug.bugId} className="p-4">
                              <div className="space-y-2">
                                <div className="flex justify-between items-start">
                                  <h5 className="font-medium text-sm">{bug.title}</h5>
                                  <div className="flex gap-2">
                                    <Badge 
                                      variant={bug.resolvedDate ? "secondary" : "destructive"}
                                      className="text-xs"
                                    >
                                      {bug.resolvedDate ? "Đã giải quyết" : "Đang mở"}
                                    </Badge>
                                    <Badge variant="outline" className="text-xs">
                                      {bug.serverity}
                                    </Badge>
                                  </div>
                                </div>
                                
                                <p className="text-sm text-muted-foreground">{bug.description}</p>
                                
                                {bug.stepsToReduces && bug.stepsToReduces !== "N/A" && (
                                  <div className="text-xs">
                                    <span className="font-medium">Các bước khắc phục: </span>
                                    {bug.stepsToReduces}
                                  </div>
                                )}
                                
                                <div className="flex justify-between text-xs text-muted-foreground">
                                  <span>Báo cáo: {new Date(bug.reportedDate).toLocaleDateString()}</span>
                                  {bug.resolvedDate && (
                                    <span>Giải quyết: {new Date(bug.resolvedDate).toLocaleDateString()}</span>
                                  )}
                                </div>
                              </div>
                            </Card>
                          ))}
                        </div>
                      </div>
                    )}
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </div>
      </div>
    </RoleGuard>
  );
}