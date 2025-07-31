function Register() {
  return (
    <div className="container">
      <h2>Реєстрація</h2>
      <form>
        <div className="mb-3">
          <label className="form-label">Ім’я</label>
          <input type="text" className="form-control" />
        </div>
        <div className="mb-3">
          <label className="form-label">Email</label>
          <input type="email" className="form-control" />
        </div>
        <div className="mb-3">
          <label className="form-label">Пароль</label>
          <input type="password" className="form-control" />
        </div>
        <button className="btn btn-primary">Зареєструватися</button>
      </form>
    </div>
  )
}

export default Register
