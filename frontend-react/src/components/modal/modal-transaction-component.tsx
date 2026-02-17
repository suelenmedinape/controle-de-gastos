import { useState, useEffect } from "react";
import type { ModalNewTransactionProps } from "../../models/modal-model";
import { TransactionService } from "../../service/transaction-service";
import { PersonService } from "../../service/person-service";
import { CategoryService } from "../../service/category-service";
import { createTransactionSchema } from "../../models/transaction-model";
import { getPorposeLabel, Porpose } from "../../enums/porpose";
import type { PersonModel } from "../../models/person-model";
import type { CategoryModel } from "../../models/category-model";

export function useModal() {
  const [isOpen, setIsOpen] = useState(false);

  const open = () => setIsOpen(true);
  const close = () => setIsOpen(false);

  return { isOpen, open, close };
}

const transactionService = new TransactionService();
const personService = new PersonService();
const categoryService = new CategoryService();

export default function ModalTransaction({
  isOpen,
  onClose,
  onTransactionAdded,
}: ModalNewTransactionProps) {
  const [formData, setFormData] = useState({
    personId: "" as string,
    description: "",
    value: "" as string | number,
    type: Porpose.EXPENSE as number,
    categoryId: "" as string,
  });
  const [persons, setPersons] = useState<PersonModel[]>([]);
  const [categories, setCategories] = useState<CategoryModel[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(false);

  // Carregar pessoas e categorias quando o modal abre
  useEffect(() => {
    if (isOpen) {
      loadPersonsAndCategories();
    }
  }, [isOpen]);

  const loadPersonsAndCategories = async () => {
    setLoadingData(true);
    try {
      const [personsRes, categoriesRes] = await Promise.all([
        personService.getAllPersons(),
        categoryService.getAllCategories(),
      ]);
      const personsData = personsRes.metadata.data || [];
      const categoriesData = categoriesRes.metadata.data || [];
    
      
      setPersons(personsData);
      setCategories(categoriesData);
    } catch (error) {
      setPersons([]);
      setCategories([]);
    } finally {
      setLoadingData(false);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    if (errors[name]) {
      setErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[name];
        return newErrors;
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrors({});
    setLoading(true);

    try {
      // Converter valores
      const value = parseFloat(formData.value as any);
      const categoryId = String(formData.categoryId); // Manter como string
      const personId = String(formData.personId); // Manter como string
      const type = parseInt(formData.type as any);

      // Validar antes de enviar
      const validatedData = createTransactionSchema.parse({
        description: formData.description,
        value: value,
        type: type,
        categoryId: categoryId,
        personId: personId,
      });

      await transactionService.createTransaction(validatedData);
      alert("Transação criada com sucesso");

      setFormData({
        personId: "",
        description: "",
        value: "",
        type: Porpose.EXPENSE,
        categoryId: "",
      });

      onClose();
      onTransactionAdded?.();
    } catch (error: any) {
      if (error.issues) {
        const zodErrors: Record<string, string> = {};
        error.issues.forEach((issue: any) => {
          zodErrors[issue.path[0]] = issue.message;
        });
        setErrors(zodErrors);
      } else if (error.data?.metadata) {
        const backendErrors: Record<string, string> = {};
        const metadata = error.data.metadata;

        if (metadata.Description) {
          backendErrors.description = metadata.Description[0];
        }
        if (metadata.Value) {
          backendErrors.value = metadata.Value[0];
        }
        if (metadata.Type) {
          backendErrors.type = metadata.Type[0];
        }
        if (metadata.CategoryId) {
          backendErrors.categoryId = metadata.CategoryId[0];
        }
        if (metadata.PersonId) {
          backendErrors.personId = metadata.PersonId[0];
        }

        setErrors(backendErrors);
      } else {
        setErrors({ submit: error.message || "Erro ao criar transação" });
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      id="transaction-modal"
      aria-hidden={!isOpen}
      className={`${
        isOpen ? "flex" : "hidden"
      } fixed top-0 right-0 left-0 z-50 h-[calc(100%-1rem)] max-h-full w-full items-center justify-center overflow-x-hidden overflow-y-auto backdrop-blur-sm md:inset-0`}
    >
      <div className="relative max-h-full w-full max-w-md p-4">
        <div className="relative rounded-lg bg-white shadow-sm">
          <div className="flex items-center justify-between rounded-t border-b border-gray-200 p-4 md:p-5">
            <h3 className="text-xl font-semibold text-gray-900">
              Nova Transação
            </h3>
            <button
              onClick={onClose}
              type="button"
              className="end-2.5 ms-auto inline-flex h-8 w-8 items-center justify-center rounded-lg bg-transparent text-sm text-gray-400 hover:bg-gray-200 hover:text-gray-900"
            >
              <svg
                className="h-3 w-3"
                aria-hidden="true"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 14 14"
              >
                <path
                  stroke="currentColor"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth="2"
                  d="m1 1 6 6m0 0 6 6M7 7l6-6M7 7l-6 6"
                />
              </svg>
              <span className="sr-only">Close modal</span>
            </button>
          </div>

          <div className="p-4 md:p-5">
            <form className="space-y-4" onSubmit={handleSubmit}>
              {errors.submit && (
                <div className="rounded-lg border border-red-500 bg-red-50 p-3">
                  <p className="text-sm text-red-600">{errors.submit}</p>
                </div>
              )}

              {/* Pessoa */}
              <div>
                <label
                  htmlFor="personId"
                  className="mb-2 block text-sm font-medium text-gray-900"
                >
                  Pessoa
                </label>
                <select
                  name="personId"
                  id="personId"
                  value={formData.personId}
                  onChange={handleChange}
                  disabled={loadingData}
                  className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 disabled:opacity-50 ${
                    errors.personId ? "border-red-500 bg-red-50" : "border-gray-300"
                  }`}
                >
                  <option value="">Selecione a pessoa</option>
                  {persons.map((person) => (
                    <option key={person.id} value={person.id}>
                      {person.name}
                    </option>
                  ))}
                </select>
                {errors.personId && (
                  <p className="mt-1 text-sm text-red-600">
                    • {errors.personId}
                  </p>
                )}
              </div>

              {/* Descrição */}
              <div>
                <label
                  htmlFor="description"
                  className="mb-2 block text-sm font-medium text-gray-900"
                >
                  Descrição
                </label>
                <input
                  type="text"
                  name="description"
                  id="description"
                  value={formData.description}
                  onChange={handleChange}
                  className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 ${
                    errors.description
                      ? "border-red-500 bg-red-50"
                      : "border-gray-300"
                  }`}
                  placeholder="Descrição da transação"
                />
                {errors.description && (
                  <p className="mt-1 text-sm text-red-600">
                    • {errors.description}
                  </p>
                )}
              </div>

              {/* Valor e Tipo */}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label
                    htmlFor="value"
                    className="mb-2 block text-sm font-medium text-gray-900"
                  >
                    Valor (R$)
                  </label>
                  <input
                    type="number"
                    name="value"
                    id="value"
                    step="0.01"
                    min="0"
                    value={formData.value}
                    onChange={handleChange}
                    className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 ${
                      errors.value ? "border-red-500 bg-red-50" : "border-gray-300"
                    }`}
                    placeholder="0.00"
                  />
                  {errors.value && (
                    <p className="mt-1 text-sm text-red-600">
                      • {errors.value}
                    </p>
                  )}
                </div>

                <div>
                  <label
                    htmlFor="type"
                    className="mb-2 block text-sm font-medium text-gray-900"
                  >
                    Tipo
                  </label>
                  <select
                    name="type"
                    id="type"
                    value={formData.type}
                    onChange={handleChange}
                    className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 ${
                      errors.type ? "border-red-500 bg-red-50" : "border-gray-300"
                    }`}
                  >
                    <option value={Porpose.EXPENSE}>Despesa</option>
                    <option value={Porpose.INCOME}>Receita</option>
                  </select>
                  {errors.type && (
                    <p className="mt-1 text-sm text-red-600">
                      • {errors.type}
                    </p>
                  )}
                </div>
              </div>

              {/* Categoria */}
              <div>
                <label
                  htmlFor="categoryId"
                  className="mb-2 block text-sm font-medium text-gray-900"
                >
                  Categoria
                </label>
                <select
                  name="categoryId"
                  id="categoryId"
                  value={formData.categoryId}
                  onChange={handleChange}
                  disabled={loadingData}
                  className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 disabled:opacity-50 ${
                    errors.categoryId
                      ? "border-red-500 bg-red-50"
                      : "border-gray-300"
                  }`}
                >
                  <option value="">Selecione a categoria</option>
                  {categories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.description} ({getPorposeLabel(category.purpose)})
                    </option>
                  ))}
                </select>
                {errors.categoryId && (
                  <p className="mt-1 text-sm text-red-600">
                    • {errors.categoryId}
                  </p>
                )}
              </div>

              <button
                type="submit"
                disabled={loading || loadingData}
                className="w-full rounded-lg bg-blue-600 px-5 py-2.5 text-center text-sm font-medium text-white hover:bg-blue-700 focus:ring-4 focus:ring-blue-300 focus:outline-none disabled:cursor-not-allowed disabled:opacity-50"
              >
                {loading ? "Carregando..." : "Cadastrar"}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}