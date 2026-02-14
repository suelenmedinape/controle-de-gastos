import { Route, Routes } from "react-router-dom";
import NavBar from "./components/NavBar/NavBarComponent";

export default function App() {
  return (
    <div className="flex h-screen bg-gray-50">
      <NavBar />
      <main className="flex-1 overflow-y-auto pt-16 lg:pt-0">
        <Routes>
          {/* vazio pois ainda nao tenho as paginas */}
          {/* <Route path="/" element={} />
          <Route path="/pessoas" element={} />
          <Route path="/categorias" element={} />
          <Route path="/transacoes" element={} /> */}
        </Routes>
      </main>
    </div>
  );
}
