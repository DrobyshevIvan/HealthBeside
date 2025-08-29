import { Tabs } from "antd";
import LoginForm from "../../components/LoginForm/LoginForm.jsx";
import RegistrationForm from "../../components/RegistrationForm/RegistrationForm.jsx";
import "./Authorization.css";

export default function Authorization() {
  return (
    <div className="auth-page">
      <div className="auth-card">
        <h2 className="auth-title">Авторизація</h2>
        <Tabs
          defaultActiveKey="login"
          items={[
            { key: "login", label: "Вхід", children: <LoginForm /> },
            { key: "register", label: "Peєстрація", children: <RegistrationForm /> },
          ]}
        />
      </div>
    </div>
  );
}