import api from "../api/api.js";

export const productService = {
  async getProducts(options = {}) {
    const {
      name,
      sku,
      categoryId,
      minPrice,
      maxPrice,

      orderBy,
      sortDirection,

      page = 1,
      pageSize = 12,
    } = options;

    const params = new URLSearchParams();

    if (name) params.append("name", name);
    if (sku) params.append("sku", sku);
    if (categoryId) params.append("categoryId", categoryId);
    if (minPrice) params.append("minPrice", minPrice);
    if (maxPrice) params.append("maxPrice", maxPrice);
    if (orderBy) params.append("orderBy", orderBy);
    if (sortDirection) params.append("sortDirection", sortDirection);

    params.append("page", page);
    params.append("pageSize", pageSize);

    const response = await api.get(
      `/MarketProduct/get-products?${params.toString()}`,
      { withCredentials: true }
    );

    const data = response.data;
    // Support both "items/total" or direct array responses for backward compatibility
    const products = data.products;
    const total = data.total;

    return { products, total };
  },

  async getProductById(id) {
    const response = await api.get(`/MarketProduct/get-product/${id}`, {
      withCredentials: true,
    });
    return response.data;
  },
};
