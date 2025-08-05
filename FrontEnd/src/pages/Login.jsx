import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";

function Login() {
  const [formData, setFormData] = useState({ email: "", password: "" });
  const [message, setMessage] = useState("");
  const navigate = useNavigate();

  const handleChange = e => {
    setFormData(prev => ({
      ...prev,
      [e.target.name]: e.target.value
    }));
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      await axios.post("https://localhost:7049/api/account/account/login", formData, {
        withCredentials: true
      });
      localStorage.setItem("isAuthenticated", "true");
      // ✅ редірект на особистий кабінет
      navigate("/profile");
    } catch (error) {
      console.error("Login failed:", error);
      setMessage(error.response?.data || "Login error");
    }
  };

  return (
    <div className="container">
      <h2>Увійти</h2>
      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label className="form-label">Email</label>
          <input
            type="email"
            name="email"
            className="form-control"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="mb-3">
          <label className="form-label">Пароль</label>
          <input
            type="password"
            name="password"
            className="form-control"
            value={formData.password}
            onChange={handleChange}
            required
          />
        </div>
        <button type="submit" className="btn btn-primary">Увійти</button>

        {message && <p className="text-danger mt-3">{message}</p>}
      </form>
    </div>
  );
}

export default Login;
