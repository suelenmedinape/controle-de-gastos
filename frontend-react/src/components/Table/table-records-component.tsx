import { Pencil, Trash2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import TableHeaders from "./table-headers-component";
import ModalNewItem, { useModal } from "../modal/modal-component";
import { PersonService } from "../../service/person-service";
import type { PersonModel } from "../../models/person-model";
import { CategoryService } from "../../service/category-service";
import type { CategoryModel } from "../../models/category-model";
import { getPorposeLabel } from "../../enums/porpose";
import PurposeBadge from "../badges/purpose-component";

const personService = new PersonService();
const categoryService = new CategoryService();

export default function TableRecords() {
  const location = useLocation();
  const [isByPerson, setIsByPerson] = useState(true);
  const [persons, setPersons] = useState<PersonModel[]>([]);
  const [categories, setCategories] = useState<CategoryModel[]>([]);
  const [loading, setLoading] = useState(false);
  const { isOpen, open, close } = useModal();

  const fetchs = async () => {
    try {
      if (!isByPerson) {
        setLoading(true);
        const response = await categoryService.getAllCategories();
        setCategories(response.metadata.data || []);
        console.log(response.metadata.data || []);
      } else {
        setLoading(true);
        const response = await personService.getAllPersons();
        setPersons(response.metadata.data || []);
      }
      
    } catch (error) {
      console.error(error);
      setCategories([]);
      setPersons([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const newIsByPerson = location.pathname.includes("/person");
    setIsByPerson(newIsByPerson);
    fetchs();
  }, [location.pathname]);

  useEffect(() => {
    fetchs();
  }, []);

  const handlePersonAdded = () => {
    fetchs();
    close();
  };

  return (
    <section className="bg-gray-50 p-3 py-8 sm:p-5 md:py-16">
      <div className="mx-auto max-w-7xl px-4 lg:px-12">
        <div className="relative overflow-hidden bg-white shadow-md sm:rounded-lg">
          <TableHeaders isByPerson={isByPerson ? 1 : 2} onOpenModal={open} />

          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-gray-500">
              <thead className="bg-gray-50 text-xs text-gray-700 uppercase">
                <tr>
                  <th className="px-4 py-3">Nome</th>
                  <th className="px-4 py-3 text-right">
                    {isByPerson ? "Idade" : "Finalidade"}
                  </th>
                  <th className="px-4 py-3 text-right">Ações</th>
                </tr>
              </thead>

              <tbody>
                {loading ? (
                  <tr>
                    <td colSpan={3} className="px-4 py-3 text-center">
                      Carregando...
                    </td>
                  </tr>
                ) : (isByPerson ? persons.length > 0 : categories.length > 0) ? (
                  (isByPerson ? persons : categories).map((item) => (
                    <tr key={item.id} className="border-b hover:bg-gray-50">
                      <th className="px-4 py-3 font-medium whitespace-nowrap text-gray-900">
                        {isByPerson ? (item as PersonModel).name : (item as CategoryModel).description}
                      </th>
                      <td className="px-4 py-3 text-right">
                        {isByPerson 
                          ? `${(item as PersonModel).age} anos` 
                          : <PurposeBadge purpose={(item as CategoryModel).purpose} />
                        }
                      </td>
                      <td className="px-4 py-3 text-right">
                        <div className="flex justify-end gap-3">
                          <button className="text-green-600 transition hover:text-green-800">
                            <Pencil className="h-5 w-5" />
                          </button>
                          <button className="text-red-600 transition hover:text-red-800">
                            <Trash2 className="h-5 w-5" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={3} className="px-4 py-3 text-center">
                      Nenhum registro encontrado
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <ModalNewItem
        isOpen={isOpen}
        onClose={close}
        onPersonAdded={handlePersonAdded}
        isPerson={isByPerson}
      />
    </section>
  );
}