import { Users, Tag, ArrowRightLeft, BarChart3, Clock } from 'lucide-react';
import { useState } from 'react';

export default function NavBar() {
  const [activeItem, setActiveItem] = useState('pessoas');

  const menuItems = [
    {
      category: 'Cadastros',
      items: [
        { id: 'pessoas', label: 'Pessoas', icon: Users },
        { id: 'categorias', label: 'Categorias', icon: Tag },
        { id: 'transacoes', label: 'Transações', icon: ArrowRightLeft },
      ],
    },
    {
      category: 'Relatórios',
      items: [
        { id: 'por-pessoa', label: 'Por Pessoa', icon: BarChart3 },
        { id: 'por-categoria', label: 'Por Categoria', icon: Clock },
      ],
    },
  ];

  return (
    <aside className="w-64 bg-white border-r border-gray-200 h-screen overflow-y-auto">
      <div className="p-6 border-b border-gray-200">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-linear-to-br from-green-500 to-green-600 rounded-lg flex items-center justify-center">
            <svg className="w-6 h-6 text-white" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" viewBox="0 0 24 24">
            <path fill-rule="evenodd" d="M7 6a2 2 0 0 1 2-2h11a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2h-2v-4a3 3 0 0 0-3-3H7V6Z" clip-rule="evenodd"/>
            <path fill-rule="evenodd" d="M2 11a2 2 0 0 1 2-2h11a2 2 0 0 1 2 2v7a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2v-7Zm7.5 1a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5Z" clip-rule="evenodd"/>
            <path d="M10.5 14.5a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"/>
          </svg>
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
                      onClick={() => setActiveItem(item.id)}
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
