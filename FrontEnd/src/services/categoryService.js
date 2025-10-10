import api from "../api/api.js";

export const categoryService = {
  async getCategories() {
    const response = await api.get(`/MarketCategory/get-categories`, {
      withCredentials: true,
    });
    return response.data;
  }
}