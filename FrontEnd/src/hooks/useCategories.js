import {useEffect, useState} from "react";
import {categoryService} from "../services/categoryService.js";

export function useCategories() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const fetchCategories = async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await categoryService.getCategories();
      setCategories(response);
    } catch (error) {
      setError(error);
      console.error("Error fetching categories", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    fetchCategories();
  }, []);

  return { categories, loading, error };
}