import { useEffect, useState, useCallback } from "react";
import { cartService } from "../../services/cartService";

export function useCart() {
  const [cart, setCart] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const refetchCart = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await cartService.getCart();
      setCart(response);
    } catch (error) {
      setError(error);
      console.error("Error fetching cart", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refetchCart();
  }, [refetchCart]);

  return { cart, loading, error, refetchCart };
}
