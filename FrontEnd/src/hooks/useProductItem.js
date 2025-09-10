import { useState, useEffect } from "react";
import { productService } from "../services/productService";

export function useProductItem(id) {
  const [product, setProduct] = useState();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchProduct = async (id) => {
    setLoading(true);
    setError(null);

    try {
      const product = await productService.getProductById(id);
      setProduct(product);
    } catch (err) {
      setError(err.message);
      console.error("Error fetching product:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!id) return;
    fetchProduct(id);
  }, [id]);

  return { product, loading, error, fetchProduct };
}
