import { useCallback, useState } from "react";
import { cartService } from "../../services/cartService";

export function useRemoveFromCart() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const removeFromCart = useCallback(async ({ id }) => {
    setLoading(true);
    setError(null);

    try {
      const result = await cartService.removeFromCart({ id });
      return result;
    } catch (error) {
      setError(error);
      console.error("Error deleting product from cart", error);
    } finally {
      setLoading(false);
    }
  }, []);

  return { removeFromCart, loading, error };
}
