import { NextResponse } from "next/server";
import { Product } from "@/types/product";

let products: Product[] = [
  {
    productId: "8fd6bc74-47bd-48a1-89e6-13ef1d2608b5",
    productName: "Hệ thống PMIS",
    description: "Hệ thống PMIS cho quản sản phẩm, dự án.",
    version: "1.0.2",
    createdAt: "2025-04-26 13:42:05.668856+07",
    lastUpdated: "2025-05-02 22:38:23.552276+07",
    createdBy: "d0fd2ff4-fa0e-4e74-b3b9-b09c10057268",
  },
];

export async function GET() {
  return NextResponse.json(products);
}
