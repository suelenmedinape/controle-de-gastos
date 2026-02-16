import type { ApiResponseModel } from "../models/api-response-model";
import type { CategoryModel, createCategorySchemaType } from "../models/category-model";

export class CategoryService {
  private readonly baseUrl: string = "http://localhost:5124/Categories";

  async createCategory(data: createCategorySchemaType): Promise<ApiResponseModel> {
    const response = await fetch(this.baseUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao criar pessoa") as any;
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
      const error = new Error("Erro ao obter pessoas") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }
}
