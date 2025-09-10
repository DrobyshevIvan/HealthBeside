import { useState, useEffect } from "react";
import { productService } from "../services/productService";

export function useProducts(initialOptions = {}) {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [total, setTotal] = useState(0);

  const fetchProducts = async (options = {}) => {
    setLoading(true);
    setError(null);

    try {
      const mergedOptions = {
        ...initialOptions,
        ...options,
      };
      const { products, total } = await productService.getProducts(
        mergedOptions
      );
      console.log("Products are in useProducts.js", products);
      setProducts(products);
      setTotal(total);
    } catch (err) {
      setError(err.message);
      console.error("Error fetching products:", err);
    } finally {
      setLoading(false);
    }
  };

  // Завантаження тільки при монтажі
  useEffect(() => {
    fetchProducts();
  }, []); // Пустий масив = тільки при монтажі

  return { products, loading, error, fetchProducts, total };
}
