import { Users, Tag, ArrowRightLeft, BarChart3, Clock, CircleDollarSign } from 'lucide-react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

export default function NavBar() {
  const [activeItem, setActiveItem] = useState('pessoas');
  const navigate = useNavigate();

  const menuItems = [
    {
      category: 'Cadastros',
      items: [
        { id: '/person', label: 'Pessoas', icon: Users },
        { id: '/category', label: 'Categorias', icon: Tag },
        { id: '/transaction', label: 'Transações', icon: ArrowRightLeft },
      ],
    },
    {
      category: 'Relatórios',
      items: [
        { id: '/byPerson', label: 'Por Pessoa', icon: BarChart3 },
        { id: '/byCategory', label: 'Por Categoria', icon: Clock },
      ],
    },
  ];

  const handleNavigation = (path: string) => {
    setActiveItem(path);
    navigate(path);
  };

  return (
    <aside className="w-64 bg-white border-r border-gray-200 h-screen overflow-y-auto">
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-linear-to-br from-green-500 to-green-600 rounded-lg flex items-center justify-center">
            <CircleDollarSign className="w-5 h-5 text-white" />
          </div>
          <div>
            <h1 className="text-lg font-bold text-gray-900">Residenciais</h1>
            <p className="text-xs text-gray-500">Controle de gastos</p>
          </div>
        </div>
      </div>

      <nav className="p-4 space-y-8">
        {menuItems.map((section, idx) => (
          <div key={idx}>
            <h2 className="px-4 py-2 text-xs font-semibold text-gray-500 uppercase tracking-wider">
              {section.category}
            </h2>
            <ul className="space-y-1">
              {section.items.map((item) => {
                const Icon = item.icon;
                const isActive = activeItem === item.id;

                return (
                  <li key={item.id}>
                    <button
                      onClick={() => handleNavigation(item.id)}
                      className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg text-sm font-medium transition-colors ${
                        isActive
                          ? 'bg-green-50 text-green-600'
                          : 'text-gray-600 hover:bg-gray-50'
                      }`}
                    >
                      <Icon className="w-5 h-5 shrink-0" />
                      <span>{item.label}</span>
                    </button>
                  </li>
                );
              })}
            </ul>
          </div>
        ))}
      </nav>
    </aside>
  );
}
