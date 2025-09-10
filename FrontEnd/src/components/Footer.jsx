import React from "react";
import "../styles.css";

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container">
        <div className="footer__inner">
          <div className="footer__copyright">
            © 2025 HealthBeside. Всі права захищено.
          </div>
          <div className="footer__links">
            <a href="#" className="footer__link">Умови використання</a>
            <a href="#" className="footer__link">Політика конфіденційності</a>
          </div>
        </div>
      </div>
    </footer>
  );
}
