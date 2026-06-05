import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: { "/api": "http://localhost:5080" },
  },
  build: {
    outDir: "../api/wwwroot",
    emptyOutDir: true,
  },
});
