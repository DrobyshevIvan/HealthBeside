import { useContext } from "react";
import { AuthContext } from "../context/AuthContext";

export function useAuth() {
  const { user, login, logout, isAuthenticated, loading } =
    useContext(AuthContext);

  return {
    user,
    login,
    logout,
    isAuthenticated,
    loading,
  };
}
