import api from "../api/api.js";

export const authService = {
  async register({registrationData}) {
    const response = await api.post(
      `/Account/register`,
      {registrationData},
      {
        withCredentials: true,
      }
    );
    return response.data;
  },

  async login({ email, password }) {
    const response = await api.post(
      `/Account/login`,
      { email, password },
      {
        withCredentials: true,
      }
    );
    return response.data;
  },

  async refreshToken(credentials) {
    const response = await api.post(`/Account/refresh-token`, credentials, {
      withCredentials: true,
    });
    return response.data;
  },

  async getUserInfo() {
    const response = await api.get(`/Account/get-user-info`, {
      withCredentials: true,
    });
    return response.data;
  },

  async logout() {
    const response = await api.post(`/Account/logout`, {
      withCredentials: true,
    });
    return response.data;
  },
};
