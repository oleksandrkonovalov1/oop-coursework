import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import App from "./App";
import UniversitiesPage from "./pages/UniversitiesPage";
import UniversityDetailsPage from "./pages/UniversityDetailsPage";
import SpecialtiesPage from "./pages/SpecialtiesPage";
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
      { path: "universities", element: <UniversitiesPage /> }, // аліас — у спеці сторінка описана як /universities
      { path: "universities/:id", element: <UniversityDetailsPage /> },
      { path: "specialties", element: <SpecialtiesPage /> },
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
