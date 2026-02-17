export enum Porpose {
  EXPENSE = 1,
  INCOME = 2,
  BOTH = 3,
}

export function getPorposeLabel(purpose: Porpose | number | undefined | null): string {
  switch (purpose) {
    case Porpose.EXPENSE:
    case 1:
      return "Despesa";
    case Porpose.INCOME:
    case 2:
      return "Receita";
    case Porpose.BOTH:
    case 3:
      return "Ambas";
    default:
      return "Desconhecido";
  }
}