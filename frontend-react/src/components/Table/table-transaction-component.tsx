import TableHeaders from "./table-headers-component";

export default function TableTransaction() {
  return (
    <section className="bg-gray-50 p-3 sm:p-5">
      <div className="mx-auto max-w-7xl px-4 lg:px-12">
        <div className="relative overflow-hidden bg-white shadow-md sm:rounded-lg">
          <TableHeaders isByPerson={3} />

          {/* Tabela */}
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-gray-500">
              <thead className="bg-gray-50 text-xs text-gray-700 uppercase">
                <tr>
                  <th className="px-4 py-3">Descrição</th>
                  <th className="px-4 py-3">Pessoa</th>
                  <th className="px-4 py-3">Categoria</th>
                  <th className="px-4 py-3">Tipo</th>
                  <th className="px-4 py-3">Valor</th>
                </tr>
              </thead>

              <tbody>
                <tr className="border-b hover:bg-gray-50">
                  <th className="px-4 py-3">desc</th>
                  <td className="px-4 py-3">pessoa</td>
                  <td className="px-4 py-3">categ</td>
                  <td className="px-4 py-3">tipo</td>
                  <td className="px-4 py-3">valor</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </section>
  );
}