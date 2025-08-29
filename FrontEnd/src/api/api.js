import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5180/api",
  withCredentials: true,
});

export default api;

// Глобальний проміс, щоб не робити кілька refresh запитів одночасно
let refreshPromise = null;

api.interceptors.response.use(
  (response) => response,
  // Якщо помилка, то починаєтся виконання асинхронного хендлера
  async (error) => {
    // Зберігаємо оригінальний запит(щоб потім його повторити)
    const original = error.config;

    // Якщо помилка не 401 або запит вже був спробований, то повертаємо помилку
    if (
      error.response?.status !== 401 ||
      original?._retry ||
      original?.url?.includes("/Account/refresh-token")
    ) {
      return Promise.reject(error);
    }

    // Позначаємо, що для цього запиту вже була спроба оновити токен(щоб не зациклюватись)
    original._retry = true;

    // Якщо немає промісу, то створюємо новий
    if (!refreshPromise) {
      refreshPromise = api.post("/Account/refresh-token", {}).finally(() => {
        // Після успішного оновлення токену, скидаємо проміс
        refreshPromise = null;
      });
    }

    try {
      // Очікуємо на оновлення токену
      await refreshPromise;
      // Повторюємо оригінальний запит
      return api(original);
      // Якщо помилка при оновленні токену, то повертаємо помилку
    } catch (error) {
      return Promise.reject(error);
    }
  }
);
