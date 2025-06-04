'use client'

import { useState, useEffect } from 'react';
import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuTrigger, DropdownMenuContent, DropdownMenuItem } from "@/components/ui/dropdown-menu";
import { PlusCircle, MoreHorizontal } from "lucide-react";
import { productService } from '@/services/productService';
import { ProductStatistic } from '@/types/product';
import Link from 'next/link';

export default function ProductDashboard() {
  // State management for products, loading, and error handling
  const [products, setProducts] = useState<ProductStatistic[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch product statistics on component mount
  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await productService.getProductStatistics();
        setProducts(data);
        setLoading(false);
      } catch (err) {
        //setError(err.message);
        setLoading(false);
      }
    };
    fetchData();
  }, []);

  // Loading and error states
  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  // Color mapping for project statuses
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
    <div className="min-h-screen bg-muted/40">
      <div className="max-w-6xl mx-auto p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold">Product Statistics</h1>
        </div>

        <div className="space-y-6">
          {products.map((product) => {
            const total = product.totalProjects;

            return (
              <Card key={product.productId}>
                <CardHeader className="bg-muted/50">
                  <div className="flex justify-between items-center">
                    <CardTitle className="flex items-center gap-4">
                      {product.productName}
                      <span className="text-sm text-muted-foreground">
                        Last Updated: {new Date(product.lastUpdated).toLocaleString()}
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
                            <Link href="/product">Product List</Link>
                            </DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                </CardHeader>

                <CardContent className="pt-4">
                  <div className="mb-4">
                    <span className="font-medium">Total Projects: </span>
                    {total}
                  </div>

                  {total > 0 && (
                    <div className="flex items-start gap-6">
                      {/* Vertical Stacked Bar Chart */}
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

                      {/* Status Legend */}
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
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>
    </div>
  );
}