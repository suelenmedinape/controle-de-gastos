import { ChartLine, ChartPie } from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { TransactionService } from "../../service/transaction-service";
import type {
  ReportCategoryDTO,
  ReportPersonDTO,
} from "../../models/transaction-model";
import { getPurposeLabel } from "../../enums/purpose";

const transactionService = new TransactionService();

export default function TableReport() {
  const location = useLocation();
  const [isByPerson, setIsByPerson] = useState(true);
  const [report, setReport] = useState<ReportPersonDTO | null>(null);
  const [reportCategory, setReportCategory] =
    useState<ReportCategoryDTO | null>(null);
  const [loading, setLoading] = useState(false);

  const fetchData = async (isPerson: boolean) => {
    try {
      setLoading(true);
      if (isPerson) {
        const response = await transactionService.getAllTransactionPersons();
        setReport(response.metadata.data || null);
        setReportCategory(null);
      } else {
        const response = await transactionService.getAllTransactionCategory();
        setReportCategory(response.metadata.data || null);
        setReport(null);
      }
    } catch (error) {
      console.error(error);
      setReport(null);
      setReportCategory(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const newIsByPerson = location.pathname.includes("/byPerson");
    setIsByPerson(newIsByPerson);
    fetchData(newIsByPerson);
  }, [location.pathname]);

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat("pt-BR", {
      style: "currency",
      currency: "BRL",
    }).format(value);
  };

  if (loading) {
    return (
      <section className="py-8 antialiased md:py-16">
        <div className="mx-auto max-w-7xl px-4 2xl:px-0">
          <p className="text-center text-gray-500">Carregando relatório...</p>
        </div>
      </section>
    );
  }

  const currentReport = isByPerson ? report : reportCategory;

  if (!currentReport) {
    return (
      <section className="py-8 antialiased md:py-16">
        <div className="mx-auto max-w-7xl px-4 2xl:px-0">
          <p className="text-center text-gray-500">Nenhum dado disponível</p>
        </div>
      </section>
    );
  }

  return (
    <section className="py-8 antialiased md:py-16">
      <div className="mx-auto max-w-7xl px-4 2xl:px-0">
        <h2 className="mb-6 text-xl font-semibold text-gray-900 sm:text-2xl">
          {isByPerson ? "Relatório por Pessoa" : "Relatório por Categoria"}
        </h2>

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
          {/* Lista de Pessoas/Categorias - 2 colunas */}
          <div className="space-y-4 lg:col-span-2">
            {currentReport.financialTotals.map((item: any) => (
              <div
                key={item.id}
                className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm md:p-6"
              >
                <div className="flex items-center justify-between">
                  <div className="flex-1">
                    <h3 className="text-base font-medium text-gray-900">
                      {isByPerson ? item.name : item.description}
                    </h3>
                    <div className="mt-2 flex items-center gap-4 text-sm">
                      {!isByPerson && item.purpose && (
                        <>
                          <span className="text-gray-600">
                            {getPurposeLabel(item.purpose)}
                          </span>
                          <span className="text-gray-400">•</span>
                        </>
                      )}
                      <span className="text-gray-600">
                        Receitas: {formatCurrency(item.totalIncome)}
                      </span>
                      <span className="text-gray-400">•</span>
                      <span className="text-gray-600">
                        Despesas: {formatCurrency(item.totalExpenses)}
                      </span>
                    </div>
                  </div>
                  <div className="text-right">
                    <p
                      className={`text-lg font-bold ${
                        item.balance >= 0 ? "text-green-600" : "text-red-600"
                      }`}
                    >
                      {formatCurrency(item.balance)}
                    </p>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Resumo Geral - 1 coluna */}
          <div>
            <div className="sticky top-20 rounded-lg border border-gray-200 bg-white p-4 shadow-sm md:p-6">
              <div className="mb-6 flex items-center gap-2">
                {isByPerson ? (
                  <ChartLine className="h-5 w-5 text-green-500" />
                ) : (
                  <ChartPie className="h-5 w-5 text-green-500" />
                )}
                <h3 className="text-lg font-semibold text-gray-900">
                  Resumo Geral
                </h3>
              </div>

              <div className="space-y-4">
                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <dt className="text-sm font-normal text-gray-600">
                      Receitas
                    </dt>
                    <dd className="text-sm font-medium text-green-600">
                      {formatCurrency(currentReport.totalSummary.totalIncome)}
                    </dd>
                  </div>

                  <div className="flex items-center justify-between">
                    <dt className="text-sm font-normal text-gray-600">
                      Despesas
                    </dt>
                    <dd className="text-sm font-medium text-red-600">
                      {formatCurrency(currentReport.totalSummary.totalExpenses)}
                    </dd>
                  </div>
                </div>

                <div className="border-t border-gray-200 pt-3">
                  <div className="flex items-center justify-between">
                    <dt className="text-sm font-bold text-gray-900">Saldo</dt>
                    <dd
                      className={`text-base font-bold ${
                        currentReport.totalSummary.netBalance >= 0
                          ? "text-green-600"
                          : "text-red-600"
                      }`}
                    >
                      {formatCurrency(currentReport.totalSummary.netBalance)}
                    </dd>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
