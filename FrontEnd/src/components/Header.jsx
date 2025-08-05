import { Link, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';

function Header() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const auth = localStorage.getItem("isAuthenticated") === "true";
    setIsAuthenticated(auth);
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("isAuthenticated");
    // можна також викликати axios на logout endpoint
    navigate("/login");
    window.location.reload(); // оновимо header
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light">
      <div className="container d-flex justify-content-between align-items-center">
        <Link className="navbar-brand" to="/">Мій Сайт</Link>
        <div>
          {!isAuthenticated ? (
            <>
              <Link className="btn btn-outline-primary me-2" to="/login">Увійти</Link>
              <Link className="btn btn-primary" to="/register">Зареєструватися</Link>
            </>
          ) : (
            <>
              <Link className="btn btn-outline-success me-2" to="/profile">Кабінет</Link>
              <button className="btn btn-danger" onClick={handleLogout}>Вийти</button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}

export default Header;
