import { Button } from "antd";
import "./Home.css";
import doctorsImg from "../../assets/images/homePage/doctors.png";
import whyChooseUsImg from "../../assets/images/homePage/why-choose-us.png";
import { PlusOutlined, InstagramOutlined, FacebookOutlined, LinkedinOutlined } from "@ant-design/icons";
import Footer from "../../components/Footer";
import Header from "../../components/Header";

export default function Home() {
  return (
    <>
      <Header />
      <section className="home">
        <div className="container home__inner">
          <div className="home__content">
            <h1 className="home__title">
              Ваше здоров'я<br/>
              — поруч із вами
            </h1>
            <p className="home__subtitle">
              HealthBeside — це простий шлях до якісної медичної допомоги онлайн. Спілкуйтеся з лікарями,
              отримуйте консультації, купуйте медтехніку — все в одному місці.
            </p>
            <Button type="primary" shape="round" size="large" className="home__cta" color="#3461FF">
              Записатися до лікаря
            </Button>
          </div>

          <div className="home__imageWrap">
            <div className="home__shape" />
            <div className="home__imageMask">
              <img className="home__image" src={doctorsImg} alt="Лікарі HealthBeside" />
            </div>
          </div>
        </div>
      </section>

      <section className="services">
        <div className="container">
          <h2 className="services__title">Наші сервіси</h2>
          <div className="services__grid">
            <div className="service-card">
              <h3 className="service-card__title">Онлайн-консультації</h3>
              <p className="service-card__description">
                Спілкуйтесь з досвідченими лікарями просто з дому, у комфортній обстановці.
              </p>
            </div>
            <div className="service-card">
              <h3 className="service-card__title">Чат з лікарями</h3>
              <p className="service-card__description">
                Наші лікарі завжди на зв'язку, готові відповісти на запитання та допомогти з вибором дій.
              </p>
            </div>
            <div className="service-card">
              <h3 className="service-card__title">Форум</h3>
              <p className="service-card__description">
                Діліться досвідом, ставте запитання, читайте історії інших — і знаходьте відповіді та натхнення.
              </p>
            </div>
            <div className="service-card">
              <h3 className="service-card__title">Маркетплейс</h3>
              <p className="service-card__description">
                Ви можете легко знайти потрібне, порівняти, замовити — і отримати доставку додому.
              </p>
            </div>
          </div>
        </div>
      </section>

      <section className="how-it-works">
        <div className="container">
          <h2 className="how-it-works__title">Як це працює?</h2>
          <div className="how-it-works__steps">
            <div className="how-it-works__step">
              <div className="how-it-works__step-number">1</div>
              <div className="how-it-works__step-content">
                <h3 className="how-it-works__step-title">Зареєструйтеся або увійдіть</h3>
                <p className="how-it-works__step-description">
                  Створіть обліковий запис пацієнта чи лікаря.
                </p>
              </div>
            </div>
            <div className="how-it-works__step">
              <div className="how-it-works__step-number">2</div>
              <div className="how-it-works__step-content">
                <h3 className="how-it-works__step-title">Знайдіть лікаря та запишіться на прийом</h3>
                <p className="how-it-works__step-description">
                  Виберіть спеціаліста, зручний час - усе в декілька кліків.
                </p>
              </div>
            </div>
            <div className="how-it-works__step">
              <div className="how-it-works__step-number">3</div>
              <div className="how-it-works__step-content">
                <h3 className="how-it-works__step-title">Отримайте консультацію онлайн</h3>
                <p className="how-it-works__step-description">
                  Отримайте консультацію від лікаря, зручну для вас.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="why-choose-us">
        <div className="container">
          <h2 className="why-choose-us__title">Чому обирають HealthBeside?</h2>
          <div className="why-choose-us__content">
            <img className="why-choose-us__image" src={whyChooseUsImg} alt="Чому обирають нас" />
            <div className="why-choose-us__feature-cards">
            <div class="why-choose-us__card why-choose-us__card--solid">
              <h3 className="why-choose-us__card-title">Професійні лікарі</h3>
              <p className="why-choose-us__card-description">Ми працюємо лише з фахівцями, які мають підтверджену кваліфікацію, державну ліцензію та досвід у своїй галузі.  </p>
            </div>
            <div class="why-choose-us__card why-choose-us__card--light">
              <h3 className="why-choose-us__card-title">Онлайн - це зручно</h3>
              <p className="why-choose-us__card-description">Консультації з лікарем доступні у форматі відеозв’язку, аудіо чи звичайного чату — ви самі обираєте, як зручно.</p>
            </div>
            <div class="why-choose-us__card why-choose-us__card--solid">
              <h3 className="why-choose-us__card-title">Широкий вибір спеціалістів</h3>
              <p className="why-choose-us__card-description">На платформі ви знайдете лікарів понад 20 напрямів — від терапевтів і педіатрів до гастроентерологів та кардіологів.</p>
            </div>
            </div>
          </div>
        </div>
      </section>

      <section className="about">
        <div className="container">
          <div className="about__social">
            <div className="about__logo">
              <PlusOutlined className="about-logo__icon" />
              <span className="about-logo__text">HealthBeside</span>
            </div>
            <div className="about__social-links">
              <a href="#" className="about__social-link">
                <InstagramOutlined />
              </a>
              <a href="#" className="about__social-link">
                <FacebookOutlined />
              </a>
              <a href="#" className="about__social-link">
                <LinkedinOutlined />
              </a>
            </div>
          </div>
          <div className="about__content">
            <p className="about__description">
              HealthBeside — це цифровий медичний простір, який поєднує пацієнтів, лікарів і медтехніку на одній платформі. Ми допомагаємо зробити медицину доступною.
            </p>
            <div className="about__contacts">
              <h4 className="about__contacts-title">Контакти:</h4>
              <p className="about__phone">+38 (044) 123-45-67</p>
              <p className="about__email">support@healthbeside.com</p>
            </div>
          </div>
        </div>
      </section>
      <Footer />
    </>
  );
}