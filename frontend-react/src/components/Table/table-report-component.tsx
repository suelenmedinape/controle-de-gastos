import { ChartLine, ChartPie, Dot } from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";

export default function TableReport() {
  const location = useLocation();
  const [isByPerson, setIsByPerson] = useState(true);

  useEffect(() => {
    setIsByPerson(location.pathname.includes("/byPerson"));
  }, [location.pathname]);

  return (
    <section className="py-8 antialiased md:py-16">
      <div className="mx-auto max-w-7xl px-4 2xl:px-0">
        <h2 className="text-xl font-semibold text-gray-900 sm:text-2xl">
          {isByPerson ? "Relatório por Pessoa" : "Relatório por Categoria"}
        </h2>
        <div className="mt-6 sm:mt-8 md:gap-6 lg:flex lg:items-start xl:gap-8">
          <div className="mx-auto w-full flex-none lg:max-w-2xl xl:max-w-4xl">
            <div className="space-y-6">
              <div className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm md:p-6">
                <div className="space-y-4 md:flex md:items-center md:justify-between md:gap-6 md:space-y-0">
                  <label className="sr-only">Choose quantity:</label>
                  <div className="flex items-center justify-between md:order-3 md:justify-end">
                    <div className="text-end md:order-4 md:w-32">
                      <p className="text-base font-bold text-gray-900">
                        $1,499
                      </p>
                    </div>
                  </div>

                  <div className="w-full min-w-0 flex-1 space-y-4 md:order-2 md:max-w-md">
                    <a
                      href="#"
                      className="text-base font-medium text-gray-900 hover:underline"
                    >
                      Nome da {isByPerson ? "Pessoa" : "Categoria"}
                    </a>

                    <div className="flex items-center gap-4">
                      <p className="inline-flex items-center text-sm font-medium text-gray-500">
                        Receitas: ...
                      </p>
                      <Dot
                        className="h-1 w-1 rounded-full bg-gray-500"
                        aria-hidden="true"
                      />
                      <p className="inline-flex items-center text-sm font-medium text-gray-500">
                        Despesas: ...
                      </p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div className="mx-auto mt-6 max-w-4xl flex-1 space-y-6 lg:mt-0 lg:w-full">
            <div className="space-y-4 rounded-lg border border-gray-200 bg-white p-4 shadow-sm sm:p-6">
              <div className="flex items-center gap-2">
                { isByPerson ? (
                  <ChartLine className="h-5 w-5 text-green-500" aria-hidden="true" />
                ) : (
                  <ChartPie className="h-5 w-5 text-green-500" aria-hidden="true" />
                ) }

                <p className="text-xl font-semibold text-gray-900">
                  Resumo Geral
                </p>
              </div>

              <div className="space-y-4">
                <div className="space-y-2">
                  <dl className="flex items-center justify-between gap-4">
                    <dt className="text-base font-normal text-gray-500">
                      Receitas
                    </dt>
                    <dd className="text-base font-medium text-green-600">
                      $7,592.00
                    </dd>
                  </dl>

                  <dl className="flex items-center justify-between gap-4">
                    <dt className="text-base font-normal text-gray-500">
                      Despezas
                    </dt>
                    <dd className="text-base font-medium text-red-600">
                      -$299.00
                    </dd>
                  </dl>
                </div>

                <dl className="flex items-center justify-between gap-4 border-t border-gray-200 pt-2">
                  <dt className="text-base font-bold text-gray-900">Saldo</dt>
                  <dd className="text-base font-bold text-gray-900">
                    $8,191.00
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}
