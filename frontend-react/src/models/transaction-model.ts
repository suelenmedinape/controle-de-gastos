import z from "zod";
import type { Purpose } from "../enums/purpose";

export interface TransactionModel {
  id?: string;
  description: string;
  value: number;
  type: Purpose;
  categoryId: string;
  personId: string;
}

export interface ReportPersonDTO {
  financialTotals: {
    id: string;
    name: string;
    age: number;
    totalIncome: number;
    totalExpenses: number;
    balance: number;
  }[];
  totalSummary: {
    totalIncome: number;
    totalExpenses: number;
    netBalance: number;
  };
}

export interface ReportCategoryDTO {
  financialTotals: {
    id: string;
    description: string;
    purpose: number;
    totalIncome: number;
    totalExpenses: number;
    balance: number;
  }[];
  totalSummary: {
    totalIncome: number;
    totalExpenses: number;
    netBalance: number;
  };
}

export type createTransactionSchemaType = Omit<TransactionModel, "id">;
export type listTransactionDTO = Omit<
  TransactionModel,
  "categoryId" | "personId"
> & {
  categoryDescription: string;
  personName: string;
};

const guidRegex =
  /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

export const createTransactionSchema = z.object({
  description: z
    .string()
    .min(1, "Descrição é obrigatória")
    .max(400, "Descrição não pode ter mais de 400 caracteres"),
  value: z
    .number()
    .positive("Valor deve ser positivo")
    .refine((val) => val >= 0.01, "Valor deve ser pelo menos 0.01"),
  type: z
    .number()
    .int()
    .refine((val) => [1, 2, 3].includes(val), {
      message: "Finalidade deve ser 1 (Despesa), 2 (Receita) ou 3 (Ambas)",
    })
    .transform((v) => v as Purpose),
  categoryId: z
    .string()
    .refine((val) => guidRegex.test(val), "CategoryId deve ser um GUID válido"),
  personId: z
    .string()
    .refine((val) => guidRegex.test(val), "PersonId deve ser um GUID válido"),
});

export type CreateTransactionDTO = z.infer<typeof createTransactionSchema>;
