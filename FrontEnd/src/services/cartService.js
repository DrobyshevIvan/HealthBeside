import api from "../api/api.js";

export const cartService = {
  async getCart() {
    const response = await api.get(`/MarketCart/get-cart`, {
      withCredentials: true,
    });

    return response.data;
  },

  async addToCart({ productId, quantity }) {
    const response = await api.post(
      `MarketCart/add-product`,
      { productId, quantity },
      { withCredentials: true }
    );

    return response.data;
  },

  async removeFromCart({ id }) {
    const response = await api.delete(`MarketCart/remove-product/${id}`, {
      withCredentials: true,
    });

    return response.data;
  },

  async updateProduct({ productId, quantity }) {
    const response = await api.put(
      `MarketCart/update-product`,
      { productId, quantity },
      { withCredentials: true }
    );

    return response.data;
  },
};
