import api from "../api/api.js";

export const reviewService = {
  async getReviews(options = {}) {
    const {
      description,
      rating,
      userId,
      productId,

      orderBy,
      sortDirection,

      page = 1,
      pageSize = 6,
    } = options;

    const params = new URLSearchParams();

    if (description) params.append("description", description);
    if (rating) params.append("rating", rating);
    if (userId) params.append("userId", userId);
    if (productId) params.append("productId", productId);
    if (orderBy) params.append("orderBy", orderBy);
    if (sortDirection) params.append("sortDirection", sortDirection);

    params.append("page", page);
    params.append("pageSize", pageSize);

    const response = await api.get(
      `MarketReview/get-reviews?${params.toString()}`,
      { withCredentials: true }
    );

    const data = response.data;

    return data;
  },
};
