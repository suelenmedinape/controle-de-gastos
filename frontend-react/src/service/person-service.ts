import type { ApiResponseModel } from "../models/api-response-model";
import type { createPersonSchemaType, PersonModel } from "../models/person-model";

export class PersonService {
  private readonly baseUrl: string = "http://localhost:5124/Person";

  async createPerson(data: createPersonSchemaType): Promise<ApiResponseModel> {
    const response = await fetch(this.baseUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao adicionar pessoa ao sistema") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async updatePerson(id: string, data: createPersonSchemaType): Promise<ApiResponseModel> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao atualizar pessoa") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async getAllPersons(): Promise<ApiResponseModel<PersonModel[]>> {
    const response = await fetch(this.baseUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao obter lista de pessoas") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async deletePerson(id: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}/${id}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
    });
    
    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao tentar deletar pessoa") as any;
      error.data = errorData;
      throw error;
    }
  }
}
