import z from "zod";

export interface PersonModel {
  id?: string;
  name: string;
  age: number;
  transactions?: any[];
}

export type CreatePersonDTO = Omit<Required<PersonModel>, "id" | "transactions">;

export const createPersonSchema = z.object({
  name: z
    .string()
    .min(1, "O nome não pode estar vazio")
    .trim(),
  age: z
    .number()
    .int("A idade deve ser um número inteiro")
    .positive("A idade deve ser um número positivo")
    .min(1, "A idade deve ser maior que 0"),
});

export type createPersonSchemaType = z.infer<typeof createPersonSchema>;