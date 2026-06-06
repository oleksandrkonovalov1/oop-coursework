import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import App from "./App";
import AdminApp from "./AdminApp";
import UniversitiesPage from "./pages/public/UniversitiesPage";
import UniversityDetailsPage from "./pages/public/UniversityDetailsPage";
import SpecialtiesPage from "./pages/public/SpecialtiesPage";
import AdminUniversitiesPage from "./pages/admin/AdminUniversitiesPage";
import AdminUniversityDetailsPage from "./pages/admin/AdminUniversityDetailsPage";
import "./index.css";

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, refetchOnWindowFocus: false } },
});

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { index: true, element: <UniversitiesPage /> },
      { path: "universities", element: <UniversitiesPage /> },
      { path: "universities/:id", element: <UniversityDetailsPage /> },
      { path: "specialties", element: <SpecialtiesPage /> },
    ],
  },
  {
    path: "/admin",
    element: <AdminApp />,
    children: [
      { index: true, element: <AdminUniversitiesPage /> },
      { path: "universities/:id", element: <AdminUniversityDetailsPage /> },
    ],
  },
]);

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>,
);
