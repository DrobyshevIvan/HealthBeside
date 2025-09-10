import React, { useState, useMemo } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { PlusOutlined } from '@ant-design/icons';
import './PatientSidebar.css';

export default function PatientSidebar() {
  const location = useLocation();
  const [isMarketplaceOpen, setIsMarketplaceOpen] = useState(
    location.pathname.startsWith('/marketplace')
  );

  const menuItems = [
    { key: 'records', label: 'Записи', path: '/patient/records' },
    { key: 'consultations', label: 'Консультації', path: '/patient/consultations' },
    // Marketplace is handled separately as collapsible
    { key: 'profile', label: 'Профіль', path: '/patient/profile' },
  ];

  const settingsItems = [
    { key: 'settings', label: 'Налаштування', path: '/patient/settings' },
  ];

  const marketplaceActive = useMemo(() => location.pathname.startsWith('/marketplace'), [location.pathname]);

  return (
    <div className="patient-sidebar">
      <div className="sidebar-logo">
        <Link to="/" className="sidebar-logo" aria-label="HealthBeside home">
          <PlusOutlined className="sidebar-logo__icon" />
          <span className="sidebar-logo__text">HealthBeside</span>
        </Link>
      </div>
      
      <nav className="sidebar-nav">
        {menuItems.map(item => (
          <Link
            key={item.key}
            to={item.path}
            className={`nav-item ${location.pathname === item.path ? 'active' : ''}`}
          >
            {item.label}
          </Link>
        ))}

        {/* Collapsible Marketplace */}
        <button
          type="button"
          className={`nav-item nav-item--button ${marketplaceActive ? 'active' : ''}`}
          onClick={() => setIsMarketplaceOpen(v => !v)}
          aria-expanded={isMarketplaceOpen}
        >
          Маркетплейс
          <span className={`submenu-arrow ${isMarketplaceOpen ? 'open' : ''}`} />
        </button>
        {isMarketplaceOpen && (
          <div className="nav-submenu">
            <Link
              to="/marketplace"
              className={`nav-subitem ${location.pathname === '/marketplace' ? 'active' : ''}`}
            >
              Товари
            </Link>
            <Link
              to="/marketplace/orders"
              className={`nav-subitem ${location.pathname.startsWith('/marketplace/orders') ? 'active' : ''}`}
            >
              Замовлення
            </Link>
            <Link
              to="/marketplace/cart"
              className={`nav-subitem ${location.pathname.startsWith('/marketplace/cart') ? 'active' : ''}`}
            >
              Кошик
            </Link>
          </div>
        )}
      </nav>

      <div className="sidebar-separator"></div>

      <nav className="sidebar-settings">
        {settingsItems.map(item => (
          <Link
            key={item.key}
            to={item.path}
            className="nav-item"
          >
            {item.label}
          </Link>
        ))}
      </nav>
    </div>
  );
}