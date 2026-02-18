import { API_BASE_URL } from "../core/url";
import type { ApiResponseModel } from "../models/api-response-model";
import type {
  CategoryModel,
  createCategorySchemaType,
} from "../models/category-model";

export class CategoryService {
  private readonly baseUrl: string = `${API_BASE_URL}/Categories`;

  async createCategory(
    data: createCategorySchemaType,
  ): Promise<ApiResponseModel> {
    const response = await fetch(this.baseUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao adicionar categoria ao sistema") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async getAllCategories(): Promise<ApiResponseModel<CategoryModel[]>> {
    const response = await fetch(this.baseUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao obter lista de categorias") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }
}
