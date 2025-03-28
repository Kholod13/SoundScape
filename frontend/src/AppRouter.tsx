import React from "react";
import { Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import Playlists from "./pages/Playlists";
import Artists from "./pages/Artists";
import Albums from "./pages/Albums";
import Podcasts from "./pages/Podcasts";
import CreateNew from "./pages/CreateNew";

const AppRouter: React.FC = () => {
  return (
    <Routes>
      <Route path="/" element={<Home />} /> {/* Головна сторінка */}
      <Route path="/home" element={<Home />} />
      <Route path="/playlists" element={<Playlists />} />
      <Route path="/artists" element={<Artists />} />
      <Route path="/albums" element={<Albums />} />
      <Route path="/podcasts" element={<Podcasts />} />
      <Route path="/create-new" element={<CreateNew />} />
    </Routes>
  );
};

export default AppRouter;
