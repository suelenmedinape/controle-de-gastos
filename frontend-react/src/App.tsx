import { Route, Routes } from "react-router-dom";
import NavBar from "./components/NavBar/nav-bar-component";
import TableRecords from "./components/Table/table-records-component";
import TableTransaction from "./components/Table/table-transaction-component";
import TableReport from "./components/Table/table-report-component";

export default function App() {
  return (
    <div className="flex h-screen bg-gray-50">
      <NavBar />
      <main className="flex-1 overflow-y-auto pt-16 lg:pt-0">
        <Routes>
          <Route path="/person" element={<TableRecords />} />
          <Route path="/category" element={<TableRecords />} />
          <Route path="/transaction" element={<TableTransaction />} />
          <Route path="/byPerson" element={<TableReport />} />
          <Route path="/byCategory" element={<TableReport />} />
        </Routes>
      </main>
    </div>
  );
}
