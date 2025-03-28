import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import '../index.css'; // Імпортуємо стилі

console.log("Рендериться Playlists.tsx");

const Playlists: React.FC = () => {
  // Приклад даних плейлистів
  const playlists = [
    { id: '1', title: 'My Playlist 1', description: 'Some description', image: '/images/playlist1.jpg' },
    { id: '2', title: 'My Playlist 2', description: 'Some description', image: '/images/playlist2.jpg' },
    { id: '3', title: 'My Playlist 3', description: 'Some description', image: '/images/playlist3.jpg' },
  ];

  return (
    <div className="playlists-container">
      <h1>Плейлисти</h1>

      {/* Кнопка для створення нового плейлиста */}
      <Link to="/create-new" className="create-playlist-btn">
        Створити новий плейлист
      </Link>

      <div className="playlists-list">
        {playlists.map((playlist) => (
          <div key={playlist.id} className="playlist-item">
            <img src={playlist.image} alt={playlist.title} className="playlist-image" />
            <div className="playlist-info">
              <h2>{playlist.title}</h2>
              <p>{playlist.description}</p>
            </div>
            <div className="playlist-actions">
              <Link to={`/playlist/${playlist.id}`} className="view-button">
                Переглянути
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Playlists;
