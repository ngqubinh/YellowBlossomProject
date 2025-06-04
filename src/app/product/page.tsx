"use client";

import React, { useEffect, useState } from "react";
import { ProductsTable } from "@/components/product/products-table";
import { productService } from "@/services/productService";
import { Product } from "@/types/product";
import { AddProductDialog } from "@/components/product/product-add-form";
import { EditProductDialog } from "@/components/product/product-edit-form";
import { toast } from "sonner";
import { DeleteProductDialog } from "@/components/product/product-delete-dialog";
import { Loader2 } from "lucide-react";

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  const [deletingProduct, setDeletingProduct] = useState<Product | null>(null);

  // Fetch product list
  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    setLoading(true);
    try {
      const fetchedProducts = await productService.getAllProducts();
      setProducts(fetchedProducts);
    } catch (err: any) {
      handleError(err, "fetch");
    } finally {
      setLoading(false);
    }
  };

  // Handle add product
  const handleAddProduct = async (formData: { productName: string; description: string }) => {
    setLoading(true);
    try {
      await productService.createProduct(formData);
      await fetchProducts();
      toast.success("Thêm sản phẩm thành công!");
    } catch (err: any) {
      handleError(err, "add");
    } finally {
      setLoading(false);
    }
  };

  // Handle edit product
  const handleEditProduct = async (formData: { productName: string; description: string, version: string }) => {
    if (!editingProduct) return;

    setLoading(true);
    try {
      await productService.editProduct(editingProduct.productId, formData);
      await fetchProducts();
      toast.success("Cập nhật sản phẩm thành công!");
      setEditingProduct(null);
    } catch (err: any) {
      handleError(err, "edit");
    } finally {
      setLoading(false);
    }
  };

  // Handle delete product
  const handleDeleteProduct = async () => {
    if (!deletingProduct) return;

    setLoading(true);
    try {
      await productService.deleteProduct(deletingProduct.productId);
      await fetchProducts();
      toast.success("Xóa sản phẩm thành công!");
      setDeletingProduct(null);
    } catch (err: any) {
      handleError(err, "delete");
    } finally {
      setLoading(false);
    }
  };

  // Handle error
  const handleError = (err: any, action: string) => {
    console.error(`Error ${action} product:`, err);
    const message = err.message || `Thao tác ${action} thất bại`;
    toast.error(message);
    setError(message);
  };

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-2xl font-bold mb-6">Danh sách sản phẩm</h1>
      
      <div className="mb-8 flex justify-between items-center">
        <AddProductDialog onSubmit={handleAddProduct} loading={loading} />
      </div>

      {error && <p className="text-red-600">Lỗi: {error}</p>}
      {loading && !products.length && (
        <div className="flex justify-center py-8">
          <Loader2 className="h-8 w-8 animate-spin text-blue-500" />
        </div>
      )}

      <ProductsTable 
        data={products} 
        onEdit={(product) => setEditingProduct(product)} 
        onDelete={(product) => setDeletingProduct(product)}
      />

      {editingProduct && (
        <EditProductDialog
          product={editingProduct}
          onSubmit={handleEditProduct}
          loading={loading}
          onOpenChange={(open) => !open && setEditingProduct(null)}
        />
      )}

      {deletingProduct && (
        <DeleteProductDialog
          productName={deletingProduct.productName}
          onDelete={handleDeleteProduct}
          loading={loading}
          onOpenChange={(open) => !open && setDeletingProduct(null)}
        />
      )}
    </div>
  );
}
