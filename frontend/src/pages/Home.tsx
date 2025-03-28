import React, { useState } from "react";
import { Link } from "react-router-dom"; // Імпортуємо Link для роутингу
import "../index.css"; // Імпортуємо стилі з index.css

const App: React.FC = () => {
  const [activeMenu, setActiveMenu] = useState<string | null>(null);
  const [searchQuery, setSearchQuery] = useState<string>("");

  // Функція для перемикання меню
  const toggleMenu = (id: string) => {
    setActiveMenu(activeMenu === id ? null : id);
  };

  // Закриття меню
  const closeMenu = () => {
    setActiveMenu(null);
  };

  // Функція для обробки пошукового запиту
  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  };

  // Функція для очищення пошукового запиту
  const clearSearch = () => {
    setSearchQuery("");
  };

  return (
    <div>
      <header>
        <nav className="container">
          <div>
            <a href="#" className="logo">
              <img src="/images/logo.png" alt="логотип" />
            </a>
            <Link to="/" className="btn1 btn-primary" aria-label="Домашня сторінка">
              <img src="/images/icons_main-content/icon_home.svg" alt="іконка домашньої сторінки" />
            </Link>
          </div>

          <div className="searchContainer">
            <input 
              type="text" 
              id="mainSearch" 
              name="mainSearch" 
              className="form-control" 
              placeholder="Пошук" 
              aria-label="Пошук на сайті" 
              value={searchQuery}
              onChange={handleSearchChange}
            />
            {searchQuery && (
              <button 
                onClick={clearSearch} 
                className="clear-search-btn"
                aria-label="Очистити пошук"
                disabled={!searchQuery}
              >
                Очистити
              </button>
            )}
          </div>
          <div>
            <button type="button" className="btn btn-container" id="premium" aria-label="Premium підписка">
              <p>Premium</p>
            </button>
            <button type="button" className="btn2 btn-primary" id="messages" aria-label="Повідомлення">
              <img src="/images/icons_main-content/icon_messages.svg" alt="іконка повідомлень" />
            </button>
            <button type="button" className="btn btn-primary" id="profile" aria-label="Профіль користувача">
              <img src="/images/ava_example.svg" alt="аватар користувача" />
            </button>
          </div>
        </nav>
      </header>

      <div className="container">
        <section className="top-navigation">
          <div className="logo">
            <button className="favorite-button" onClick={() => toggleMenu("library")}>
              <img src="/images/path-to-your-heart-ico.jpg" className="favorite-icon" alt="Моя бібліотека" />
              <span>Моя бібліотека</span>
            </button>
            <button className="favorite-button" onClick={() => toggleMenu("favorites")}>
              <img src="/images/path-to-your-heart-icon.jpg" className="favorite-icon" alt="Улюблені пісні" />
              <span>Улюблені пісні</span>
            </button>
          </div>
          
          <div className="navigation-buttons">
            <Link to="/all" className="nav-button">Усе</Link>
            <Link to="/music" className="nav-button">Музика</Link>
            <Link to="/podcasts" className="nav-button">Подкасти</Link>
          </div>
        </section>

        <section className="bottom-navigation">
          <div className="search-bar-container">
            <input
              type="text"
              className="search-bar"
              placeholder="Пошук у розділі 'Моя бібліотека'"
              aria-label="Пошук у бібліотеці"
              value={searchQuery}
              onChange={handleSearchChange}
            />
            {searchQuery && (
              <button onClick={clearSearch} className="clear-search-btn" aria-label="Очистити пошук">
                Очистити
              </button>
            )}
          </div>

          <div className="navigation-buttons">
            <Link to="/playlists">
              <button className="nav-button">Плейлисти</button>
            </Link>
            <Link to="/artists">
              <button className="nav-button">Виконавці</button>
            </Link>
            <Link to="/albums">
              <button className="nav-button">Альбоми</button>
            </Link>
            <Link to="/podcasts">
              <button className="nav-button">Подкасти</button>
            </Link>
          </div>

          {/* Кнопка для створення нового елемента, що веде на відповідну сторінку */}
          <Link to="/create-new" className="filter plus">
            +
          </Link>
        </section>

        <main className="library-content">
          {[{ id: "playlist1", title: "Мій плейлист №1", subtitle: "Плейлист • 2025", image: null },
            { id: "artist1", title: "Taylor Swift", subtitle: "Виконавець", image: "images/TS.png" },
            { id: "album1", title: "The Tortured Poets Department", subtitle: "Альбом • 2024", image: "images/ALTS1.png" },
            { id: "podcast1", title: "Подкаст", subtitle: "Подкаст • 2024", image: "images/Rectangle 16.png" }].map((item) => (
              <div key={item.id} className="library-item" id={item.id}>
                {item.image ? <img src={item.image} alt={item.title} className="item-img" /> : <span className="icon"></span>}
                <div className="item-text">
                  <h3>{item.title}</h3>
                  <p>{item.subtitle}</p>
                </div>
                <span className="more-options" onClick={() => toggleMenu(item.id)}>⋮</span>
                {activeMenu === item.id && (
                  <div className="options-menu active">
                    <span className="close-menu" onClick={closeMenu}>×</span>
                    <ul>
                      <li><a href="#">Редагувати</a></li>
                      <li><a href="#">Видалити</a></li>
                      <li><a href="#">Додати в плейлист</a></li>
                    </ul>
                  </div>
                )}
              </div>
            ))}
        </main>
      </div>

      <footer>
        <div className="container">
          <div className="column">
            <h4>Компанія</h4>
            <ul>
              <li><a href="#">Про нас</a></li>
              <li><a href="#">Вакансії</a></li>
            </ul>
          </div>
          <div className="column">
            <h4>Спільноти</h4>
            <ul>
              <li><a href="#">Для виконавців</a></li>
              <li><a href="#">Для розробників</a></li>
              <li><a href="#">Для інвесторів</a></li>
              <li><a href="#">Для рекламодавців</a></li>
            </ul>
          </div>
          <div className="column">
            <h4>Корисні посилання</h4>
            <ul>
              <li><a href="#">Підтримка</a></li>
              <li><a href="#">Мобільний додаток</a></li>
            </ul>
          </div>
          <div className="column">
            <h4>Підписки</h4>
            <ul>
              <li><a href="#">Premium Individual</a></li>
              <li><a href="#">Premium Duo</a></li>
              <li><a href="#">Premium Family</a></li>
              <li><a href="#">Premium Student</a></li>
            </ul>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default App;
