import type { ApiResponseModel } from "../models/api-response-model";
import type { createTransactionSchemaType, listTransactionDTO, ReportCategoryDTO, ReportPersonDTO } from "../models/transaction-model";

export class TransactionService {
  private readonly baseUrl: string = "http://localhost:5124/Transaction";

  async createTransaction(data: createTransactionSchemaType): Promise<ApiResponseModel> {
    const response = await fetch(this.baseUrl, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao criar transferencia") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async getAllTransaction(): Promise<ApiResponseModel<listTransactionDTO[]>> {
    const response = await fetch(this.baseUrl, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao obter lista de transações") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async getAllTransactionPersons(): Promise<ApiResponseModel<ReportPersonDTO>> {
    const response = await fetch(`${this.baseUrl}/ReportPerson`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao obter relatório por pessoa") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }

  async getAllTransactionCategory(): Promise<ApiResponseModel<ReportCategoryDTO>> {
    const response = await fetch(`${this.baseUrl}/ReportCategory`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error("Erro ao obter relatório por categoria") as any;
      error.data = errorData;
      throw error;
    }

    return response.json();
  }
}
