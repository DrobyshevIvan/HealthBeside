import { reviewService } from "../services/reviewService";
import { useState, useEffect } from "react";

export function useReviews(initialOptions = {}) {
  const [reviews, setReviews] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchReviews = async (options = {}) => {
    setLoading(true);
    setError(null);

    try {
      const mergedOptions = {
        ...initialOptions,
        ...options,
      };
      const reviews = await reviewService.getReviews(mergedOptions);
      setReviews(reviews);
    } catch (err) {
      setError(err.message);
      console.error("Error fetching reviews:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchReviews();
  }, []);

  return { reviews, loading, error, fetchReviews };
}
