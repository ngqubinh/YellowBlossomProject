import { CreateProductRequest, Product, ProductService, ProductStatistic } from "@/types/product";
import axios from "axios";
import authService from "./authService";
import { toast } from "sonner";

export const productService: ProductService = {
  async getAllProducts() {
      const token = authService.getToken();
      if (!token) {
        throw new Error("User not authenticated");
      }

      try {
        const response = await axios.get("http://localhost:5250/api/product/products", {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
            'Accept': 'application/json'
          },
          responseType: 'json'
        });

        console.log("Raw response:", response);

        if (!response.data) {
          throw new Error("Empty response from server");
        }

        // Handle case where response might be a string that needs parsing
        const data = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;

        if (Array.isArray(data) && data.length > 0 && data[0].Message) {
          throw new Error(data[0].Message);
        }

        if (!Array.isArray(data)) {
          throw new Error("Expected array of products but got different data structure");
        }

        return data;
      } catch (error) {
        if (axios.isAxiosError(error)) {
          console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
            config: error.config
          });
          const errorMessage = error.response?.data?.Message || 
                            (typeof error.response?.data === 'string' ? error.response.data : error.message);
          throw new Error(errorMessage || "Failed to fetch products");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to fetch products");
      }
  },
  
  async createProduct(model: CreateProductRequest): Promise<Product> {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated");
    }

    try {
      const response = await axios.post(
        "http://localhost:5250/api/product/products",
        model,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          responseType: "json",
        }
      );

      console.log("Raw response:", response);

      if (!response.data) {
        throw new Error("Empty response from server");
      }

      // Lấy dữ liệu từ response.data, xử lý trường hợp dữ liệu trả về là dạng chuỗi
      const data =
        typeof response.data === "string"
          ? JSON.parse(response.data)
          : response.data;

      // Nếu API trả về lỗi thông qua thuộc tính Message bên trong đối tượng
      if (data.Message) {
        throw new Error(data.Message);
      }

      // Giả định API trả về một đối tượng Product hợp lệ
      return data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Axios error:", {
          status: error.response?.status,
          data: error.response?.data,
          message: error.message,
          config: error.config,
        });
        const errorMessage =
          error.response?.data?.Message ||
          (typeof error.response?.data === "string"
            ? error.response.data
            : error.message);
        throw new Error(errorMessage || "Failed to create product");
      }
      console.error("Unexpected error:", error);
      throw new Error(
        error instanceof Error ? error.message : "Failed to create product"
      );
    }
  },

  async editProduct(productId: string, model: CreateProductRequest): Promise<Product> {
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated");
    }

    try {
      const response = await axios.put(
        `http://localhost:5250/api/product/products/${productId}`, model,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          responseType: "json",
        }
      );

      console.log("Raw response:", response);

      if (!response.data) {
        throw new Error("Empty response from server");
      }

      // Lấy dữ liệu từ response.data, xử lý trường hợp dữ liệu trả về là dạng chuỗi
      const data =
        typeof response.data === "string"
          ? JSON.parse(response.data)
          : response.data;

      // Nếu API trả về lỗi thông qua thuộc tính Message bên trong đối tượng
      if (data.Message) {
        throw new Error(data.Message);
      }

      // Giả định API trả về một đối tượng Product hợp lệ
      return data;
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Axios error:", {
          status: error.response?.status,
          data: error.response?.data,
          message: error.message,
          config: error.config,
        });
        const errorMessage =
          error.response?.data?.Message ||
          (typeof error.response?.data === "string"
            ? error.response.data
            : error.message);
        throw new Error(errorMessage || "Failed to edit product");
      }
      console.error("Unexpected error:", error);
      throw new Error(
        error instanceof Error ? error.message : "Failed to edit product"
      );
    }
  },

  async deleteProduct(productId: string): Promise<boolean> {  
    const token = authService.getToken();
    if (!token) {
      throw new Error("User not authenticated");
    }

    try {
      const response = await axios.delete(`http://localhost:5250/api/product/products/${productId}`, {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
          Accept: "application/json",
        },
      });

      console.log("Raw response:", response);

      if (response.status === 200) {
        toast.success("Xóa sản phẩm thành công!");
        return true;
      } else {
        throw new Error("Failed to delete product");
      }
    } catch (error) {
      if (axios.isAxiosError(error)) {
        console.error("Axios error:", {
          status: error.response?.status,
          data: error.response?.data,
          message: error.message,
          config: error.config,
        });
        throw new Error(error.response?.data?.Message || error.message || "Failed to delete product");
      }
      console.error("Unexpected error:", error);
      throw new Error(error instanceof Error ? error.message : "Failed to delete product");
    }
  },

  async getProductStatistics() {
      const token = authService.getToken();
      if (!token) {
        throw new Error("User not authenticated");
      }

      try {
        const response = await axios.get("http://localhost:5250/api/product/statistics", {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
            'Accept': 'application/json'
          },
          responseType: 'json'
        });

        console.log("Raw response:", response);

        if (!response.data) {
          throw new Error("Empty response from server");
        }

        // Handle case where response might be a string that needs parsing
        const data = typeof response.data === 'string' ? JSON.parse(response.data) : response.data;

        if (Array.isArray(data) && data.length > 0 && data[0].Message) {
          throw new Error(data[0].Message);
        }

        if (!Array.isArray(data)) {
          throw new Error("Expected array of products but got different data structure");
        }

        return data as ProductStatistic[];
      } catch (error) {
        if (axios.isAxiosError(error)) {
          console.error("Axios error:", {
            status: error.response?.status,
            data: error.response?.data,
            message: error.message,
            config: error.config
          });
          const errorMessage = error.response?.data?.Message || 
                            (typeof error.response?.data === 'string' ? error.response.data : error.message);
          throw new Error(errorMessage || "Failed to fetch product statistics.");
        }
        console.error("Unexpected error:", error);
        throw new Error(error instanceof Error ? error.message : "Failed to fetch product statistics.");
      }
  },
}