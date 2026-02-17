import { Route, Routes } from "react-router-dom";
import NavBar from "./components/nav-bar/nav-bar-component";
import TableRecords from "./components/table/table-records-component";
import TableTransaction from "./components/table/table-transaction-component";
import TableReport from "./components/table/table-report-component";

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
