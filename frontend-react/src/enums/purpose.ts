export enum Purpose {
  EXPENSE = 1,
  INCOME = 2,
  BOTH = 3,
}

export function getPurposeLabel(
  purpose: Purpose | number | undefined | null,
): string {
  switch (purpose) {
    case Purpose.EXPENSE:
      return "Despesa";
    case Purpose.INCOME:
      return "Receita";
    case Purpose.BOTH:
      return "Ambas";
    default:
      return "Desconhecido";
  }
}