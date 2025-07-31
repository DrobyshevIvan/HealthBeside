import { Link } from 'react-router-dom'

function Header() {
  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light">
      <div className="container d-flex justify-content-between align-items-center">
        <Link className="navbar-brand" to="/">Мій Сайт</Link>
        <div>
          <Link className="btn btn-outline-primary me-2" to="/login">Увійти</Link>
          <Link className="btn btn-primary" to="/register">Зареєструватися</Link>
        </div>
      </div>
    </nav>
  )
}
export default Header
