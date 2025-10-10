import api from "../api/api";

export const profileService = {
  async updateProfileInfo(profileData) {
    const response = await api.put(`Profile/update-profile`, profileData, {
      withCredentials: true,
    });

    return response.data;
  },

  async getUserInfo() {
    const response = await api.get(`/Profile/get-user-info`, {
      withCredentials: true,
    });
    return response.data;
  },
};
