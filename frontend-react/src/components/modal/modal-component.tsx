import { useState } from "react";
import type { ModalNewPersonProps } from "../../models/modal-model";
import { PersonService } from "../../service/person-service";
import { CategoryService } from "../../service/category-service";
import { createPersonSchema } from "../../models/person-model";
import { createCategorySchema } from "../../models/category-model";
import { Porpose } from "../../enums/porpose";

export function useModal() {
  const [isOpen, setIsOpen] = useState(false);

  const open = () => setIsOpen(true);
  const close = () => setIsOpen(false);

  return { isOpen, open, close };
}

const personService = new PersonService();
const categoryService = new CategoryService();

export default function ModalNewItem({
  isOpen,
  onClose,
  onPersonAdded,
  isPerson,
}: ModalNewPersonProps) {
  const [formData, setFormData] = useState({ 
    name: "", 
    age: "",
    description: "",
    purpose: Porpose.BOTH
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === "age" || name === "purpose" 
        ? (value ? parseInt(value, 10) : "") 
        : value,
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
      if (isPerson) {
        const validatedData = createPersonSchema.parse({
          name: formData.name,
          age: formData.age,
        });

        const response = await personService.createPerson(validatedData);
        const userId = response.metadata.data;
        localStorage.setItem("id", userId);
        alert("Pessoa criada com sucesso");

        setFormData({ name: "", age: "", description: "", purpose: Porpose.BOTH });
      } else {
        const validatedData = createCategorySchema.parse({
          description: formData.description,
          purpose: formData.purpose,
        });

        await categoryService.createCategory(validatedData);
        alert("Categoria criada com sucesso");

        setFormData({ name: "", age: "", description: "", purpose: Porpose.BOTH });
      }

      onClose();
      onPersonAdded?.();
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

        if (metadata.Name) {
          backendErrors.name = metadata.Name[0];
        }
        if (metadata.Age) {
          backendErrors.age = metadata.Age[0];
        }
        if (metadata.Description) {
          backendErrors.description = metadata.Description[0];
        }
        if (metadata.Purpose) {
          backendErrors.purpose = metadata.Purpose[0];
        }

        setErrors(backendErrors);
      } else {
        setErrors({ submit: error.message || "Erro ao criar" });
      }
    } finally {
      setLoading(false);
    }
  };

  const title = isPerson ? "Nova Pessoa" : "Nova Categoria";

  return (
    <div
      id="authentication-modal"
      aria-hidden={!isOpen}
      className={`${isOpen ? "flex" : "hidden"} fixed top-0 right-0 left-0 z-50 h-[calc(100%-1rem)] max-h-full w-full items-center justify-center overflow-x-hidden overflow-y-auto backdrop-blur-sm md:inset-0`}
    >
      <div className="relative max-h-full w-full max-w-md p-4">
        <div className="relative rounded-lg bg-white shadow-sm">
          <div className="flex items-center justify-between rounded-t border-b border-gray-200 p-4 md:p-5">
            <h3 className="text-xl font-semibold text-gray-900">{title}</h3>
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

              {isPerson ? (
                <>
                  {/* FORM PESSOA */}
                  <div>
                    <label
                      htmlFor="name"
                      className="mb-2 block text-sm font-medium text-gray-900"
                    >
                      Nome
                    </label>
                    <input
                      type="text"
                      name="name"
                      id="name"
                      value={formData.name}
                      onChange={handleChange}
                      className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-green-500 focus:ring-green-500 ${
                        errors.name ? "border-red-500 bg-red-50" : "border-gray-300"
                      }`}
                      placeholder="João Silva"
                    />
                    {errors.name && (
                      <p className="mt-1 text-sm text-red-600">• {errors.name}</p>
                    )}
                  </div>

                  <div>
                    <label
                      htmlFor="age"
                      className="mb-2 block text-sm font-medium text-gray-900"
                    >
                      Idade
                    </label>
                    <input
                      type="number"
                      name="age"
                      id="age"
                      value={formData.age}
                      onChange={handleChange}
                      className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-green-500 focus:ring-green-500 ${
                        errors.age ? "border-red-500 bg-red-50" : "border-gray-300"
                      }`}
                      placeholder="28"
                    />
                    {errors.age && (
                      <p className="mt-1 text-sm text-red-600">• {errors.age}</p>
                    )}
                  </div>
                </>
              ) : (
                <>
                  {/* FORM CATEGORIA */}
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
                        errors.description ? "border-red-500 bg-red-50" : "border-gray-300"
                      }`}
                      placeholder="Descrição da categoria"
                    />
                    {errors.description && (
                      <p className="mt-1 text-sm text-red-600">• {errors.description}</p>
                    )}
                  </div>

                  <div>
                    <label
                      htmlFor="purpose"
                      className="mb-2 block text-sm font-medium text-gray-900"
                    >
                      Finalidade
                    </label>
                    <select
                      name="purpose"
                      id="purpose"
                      value={formData.purpose}
                      onChange={handleChange}
                      className={`block w-full rounded-lg border bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 ${
                        errors.purpose ? "border-red-500 bg-red-50" : "border-gray-300"
                      }`}
                    >
                      <option value="">Selecione a finalidade</option>
                      <option value={Porpose.EXPENSE}>Despesa</option>
                      <option value={Porpose.INCOME}>Receita</option>
                      <option value={Porpose.BOTH}>Ambas</option>
                    </select>
                    {errors.purpose && (
                      <p className="mt-1 text-sm text-red-600">• {errors.purpose}</p>
                    )}
                  </div>
                </>
              )}

              <button
                type="submit"
                disabled={loading}
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