export default function PurposeBadge({ purpose }: { purpose: number | undefined | null }) {
  const label = getPorposeLabel(purpose);
  const colorClass = getColorClass(purpose);

  return (
    <span className={`inline-flex items-center px-2 py-1 text-xs font-medium rounded ${colorClass}`}>
      {label}
    </span>
  );
}

function getColorClass(purpose: number | undefined | null): string {
  switch (purpose) {
    case Porpose.EXPENSE:
    case 1:
      return "bg-red-100 text-red-800";
    case Porpose.INCOME:
    case 2:
      return "bg-green-100 text-green-800";
    case Porpose.BOTH:
    case 3:
      return "bg-blue-100 text-blue-800";
    default:
      return "bg-gray-100 text-gray-800";
  }
}

import { Porpose, getPorposeLabel } from "../../enums/porpose";