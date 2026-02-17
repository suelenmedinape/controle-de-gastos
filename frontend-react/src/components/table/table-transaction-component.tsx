import { useEffect, useState } from "react";
import ModalTransaction, { useModal } from "../modal/modal-transaction-component";
import TableHeaders from "./table-headers-component";
import { TransactionService } from "../../service/transaction-service";
import type { listTransactionDTO } from "../../models/transaction-model";
import { getPorposeLabel } from "../../enums/porpose";

const transactionService = new TransactionService();

export default function TableTransaction() {
  const [transactions, setTransactions] = useState<listTransactionDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const { isOpen, open, close } = useModal();

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const response = await transactionService.getAllTransaction();
      setTransactions(response.metadata.data || []);
    } catch (error) {
      console.error("Erro ao carregar transações:", error);
      setTransactions([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTransactions();
  }, []);

  const handleTransactionAdded = () => {
    fetchTransactions();
    close();
  };

  const formatValue = (value: number, type: number) => {
    const formatted = new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(value);

    return type === 2 ? `+${formatted}` : `-${formatted}`;
  };

  return (
    <section className="bg-gray-50 p-3 py-8 sm:p-5 md:py-16">
      <div className="mx-auto max-w-7xl px-4 lg:px-12">
        <div className="relative overflow-hidden bg-white shadow-md sm:rounded-lg">
          <TableHeaders isByPerson={3} onOpenModal={open} />

          {/* Tabela */}
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-gray-500">
              <thead className="bg-gray-50 text-xs text-gray-700 uppercase">
                <tr>
                  <th className="px-4 py-3">Descrição</th>
                  <th className="px-4 py-3">Pessoa</th>
                  <th className="px-4 py-3">Categoria</th>
                  <th className="px-4 py-3">Tipo</th>
                  <th className="px-4 py-3 text-right">Valor</th>
                </tr>
              </thead>

              <tbody>
                {loading ? (
                  <tr>
                    <td colSpan={5} className="px-4 py-3 text-center">
                      Carregando...
                    </td>
                  </tr>
                ) : transactions.length > 0 ? (
                  transactions.map((transaction) => (
                    <tr key={transaction.id} className="border-b hover:bg-gray-50">
                      <th className="px-4 py-3 font-medium whitespace-nowrap text-gray-900">
                        {transaction.description}
                      </th>
                      <td className="px-4 py-3">
                        {transaction.personName}
                      </td>
                      <td className="px-4 py-3">
                        {transaction.categoryDescription}
                      </td>
                      <td className="px-4 py-3">
                        <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${
                          transaction.type === 2
                            ? "bg-green-100 text-green-800"
                            : "bg-red-100 text-red-800"
                        }`}>
                          {getPorposeLabel(transaction.type)}
                        </span>
                      </td>
                      <td className={`px-4 py-3 text-right font-medium ${
                        transaction.type === 2
                          ? "text-green-600"
                          : "text-red-600"
                      }`}>
                        {formatValue(transaction.value, transaction.type)}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={5} className="px-4 py-3 text-center">
                      Nenhuma transação encontrada
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <ModalTransaction
        isOpen={isOpen}
        onClose={close}
        onTransactionAdded={handleTransactionAdded}
      />
    </section>
  );
}