import z from "zod";
import { Purpose } from "../enums/purpose";

export interface CategoryModel {
  id?: string;
  description: string;
  purpose: Purpose;
  totalIncome?: number;
  totalExpenses?: number;
  balance?: number;
  transactions?: any[];
}

export type CreateCategoryDTO = Omit<
  Required<CategoryModel>,
  "id" | "transactions" | "totalIncome" | "totalExpenses" | "balance"
>;

export const createCategorySchema = z.object({
  description: z.string().min(1, "A descrição não pode estar vazia").trim(),
  purpose: z
    .number()
    .int()
    .refine((val) => [1, 2, 3].includes(val), {
      message: "Finalidade deve ser 1 (Despesa), 2 (Receita) ou 3 (Ambas)",
    })
    .transform((v) => v as Purpose),
});

export type createCategorySchemaType = z.infer<typeof createCategorySchema>;
