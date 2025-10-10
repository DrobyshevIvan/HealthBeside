import { useCallback, useState } from "react";
import { cartService } from "../../services/cartService";

export function useAddToCart() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const addToCart = useCallback(async ({ productId, quantity }) => {
    setLoading(true);
    setError(null);

    try {
      const result = await cartService.addToCart({ productId, quantity });
      return result;
    } catch (error) {
      setError(error);
      console.error("Error adding product to cart", error);
      throw error;
    } finally {
      setLoading(false);
    }
  }, []);

  return { addToCart, loading, error };
}
