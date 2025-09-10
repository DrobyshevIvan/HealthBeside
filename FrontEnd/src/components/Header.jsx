import React from "react";
import { Button, Menu } from "antd";
import { PlusOutlined, UserOutlined } from "@ant-design/icons";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import "../styles.css";

export default function Header() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const items = [
    { key: "/services", label: <Link to="/services">Сервіси</Link> },
    { key: "/forum", label: <Link to="/forum">Форум</Link> },
    { key: "/marketplace", label: <Link to="/marketplace">Маркетплейс</Link> },
    { key: "/contacts", label: <Link to="/contacts">Контакти</Link> },
    { key: "/chat", label: <Link to="/chat">Чат</Link> },
  ];

  const handleLoginClick = () => navigate("/authorization");

  return (
    <div className="hb-header">
      <div className="hb-header__inner container">
        <Link to="/" className="hb-logo" aria-label="HealthBeside home">
          <PlusOutlined className="hb-logo__icon" />
          <span className="hb-logo__text">HealthBeside</span>
        </Link>

        <div className="hb-nav-wrap">
          <Menu
            className="hb-nav"
            mode="horizontal"
            selectable={false}
            disabledOverflow
            items={items}
          />
        </div>

        <div className="hb-actions">
          {isAuthenticated ? (
            <Link to="/patient/profile" className="hb-user">
              <Button size="small" className="hb-login" icon={<UserOutlined />} type="default">Мій профіль</Button>
            </Link>
          ) : (
            <Button size="small" className="hb-login" type="primary" shape="round" onClick={handleLoginClick}>
              Увійти
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
