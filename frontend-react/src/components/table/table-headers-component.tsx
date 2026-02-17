import { ArrowDownUp, Plus, Tags, Users } from "lucide-react";
import type { TableHeadersPropsWithModal } from "../../models/table-header-model";

export default function TableHeaders({
  isByPerson,
  onOpenModal,
}: TableHeadersPropsWithModal) {
  return (
    <div className="flex flex-col items-center justify-between space-y-3 p-4 md:flex-row md:space-y-0 md:space-x-4">
      <div className="flex w-full items-center gap-2 md:w-1/2">
        {(() => {
          switch (isByPerson) {
            case 1:
              return <Users className="h-6 w-6 text-green-800" />;
            case 2:
              return <Tags className="h-6 w-6 text-green-800" />;
            case 3:
              return <ArrowDownUp className="h-6 w-6 text-green-800" />;
            default:
              return null;
          }
        })()}
        <span className="font-semibold text-gray-700">
          {(() => {
            switch (isByPerson) {
              case 1:
                return "Pessoas";
              case 2:
                return "Categorias";
              case 3:
                return "Transações";
              default:
                return null;
            }
          })()}
        </span>
      </div>

      <div className="flex w-full justify-end md:w-auto">
        <button
          onClick={onOpenModal}
          type="button"
          className="flex items-center justify-center gap-2 rounded-lg bg-green-600 px-4 py-2 text-sm font-medium text-white transition hover:bg-green-700 focus:ring-4 focus:ring-green-300 focus:outline-none"
        >
          <Plus className="h-4 w-4" />
          Adicionar
        </button>
      </div>
    </div>
  );
}
