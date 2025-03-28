import React from "react";
import ReactDOM from "react-dom/client";
import AppRouter from "./AppRouter";
import { BrowserRouter } from "react-router-dom";
import "./index.css";
<div id="root"></div>


ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <BrowserRouter>
      <AppRouter />
    </BrowserRouter>
  </React.StrictMode>
);
