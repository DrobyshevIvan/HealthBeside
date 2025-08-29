import {
  createContext,
  useState,
  useEffect,
  useMemo,
  useCallback,
} from "react";
import api from "../api/api.js";
import { authService } from "../services/authService";

export const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const isAuthenticated = !!user;

  const getUserInfo = useCallback(async () => {
    try {
      const data = await authService.getUserInfo();
      setUser(data);
      setError(null);
    } catch (err) {
      setUser(null);
      console.log(err);
    }
  }, []);

  const login = useCallback(
    async (credentials) => {
      setLoading(true);
      try {
        await authService.login(credentials);
        await getUserInfo();
      } finally {
        setLoading(false);
      }
    },
    [getUserInfo]
  );

  const logout = useCallback(async () => {
    setLoading(true);
    try {
      await authService.logout();
    } finally {
      setUser(null);
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // Перевірка сесії при старті застосунку
    (async () => {
      try {
        await getUserInfo();
      } finally {
        setLoading(false);
      }
    })();
  }, [getUserInfo]);

  const value = useMemo(
    () => ({
      user,
      isAuthenticated,
      loading,
      error,
      login,
      logout,
      getUserInfo,
      api, // експортуємо інстанс для запитів у компонентах/хуках
    }),
    [user, loading, error, isAuthenticated, login, logout, getUserInfo]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
