import { useCallback, useState } from "react";
import { cartService } from "../../services/cartService";

export function useUpdateCartItem() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const updateCartItem = useCallback(async ({ productId, quantity }) => {
    setLoading(true);
    setError(null);

    try {
      const result = await cartService.updateProduct({ productId, quantity });
      return result;
    } catch (error) {
      setError(error);
      console.error("Error updating cart item", error);
    } finally {
      setLoading(false);
    }
  }, []);

  return { updateCartItem, loading, error };
}
