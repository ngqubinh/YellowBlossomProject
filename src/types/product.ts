export interface ProductUser {
  fullName: string;
}

export interface Product {
  productId: string;
  productName: string;
  description: string;
  version: string;
  createdAt: string;
  lastUpdated: string;
  createdBy: string;
  user?: ProductUser | null;
  Message?: string;
}

export interface CreateProductRequest {
  productName: string;
  description: string;
}
export interface EditProductRequest {
  productName: string;
  description: string;
  version: string;
}

export type ProductStatistic = {
  productId: string;
  productName: string;
  lastUpdated: string;
  totalProjects: number;
  projectStatusDistribution: {
    Delayed: number;
    NotStarted: number;
  }
}

export interface ProductService {
  getAllProducts: () => Promise<Product[]>;
  createProduct: (model: CreateProductRequest) => Promise<Product>;
  editProduct: (productId: string, model: EditProductRequest) => Promise<Product>;
  deleteProduct: (productId: string) => Promise<boolean>;
  getProductStatistics: () => Promise<ProductStatistic[]>;
}