import React, { useState } from 'react';
import { Input, Select, Pagination, Card, Button, Skeleton, Empty, Rate } from 'antd';
import PatientSidebar from '../../../../components/PatientSidebar/PatientSidebar';
import './Catalog.css';
import { useProducts } from '../../../../hooks/useProducts';
import { useNavigate } from 'react-router-dom';
import {useCategories} from "../../../../hooks/useCategories.js";

const { Meta } = Card;

export default function Catalog() {
  const { products, loading: productsLoading, fetchProducts, total } = useProducts({
    pageSize: 12,
  });
  const { categories } = useCategories();

  const categoryOptions = (categories ?? []).map(c => ({
    value: c.id,
    label: c.name,
  }));

  const navigate = useNavigate();

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [categoryId, setCategoryId] = useState(null);
  const [sku, setSku] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [orderBy, setOrderBy] = useState(null);
  const [sortDirection, setSortDirection] = useState(null);

  const createOptions = (overrides = {}) => {
    return {
      name: search || undefined,
      categoryId: categoryId || undefined,
      sku: sku || undefined,
      minPrice: minPrice ? parseFloat(minPrice) : undefined,
      maxPrice: maxPrice ? parseFloat(maxPrice) : undefined,
      orderBy: orderBy || undefined,
      sortDirection: sortDirection || undefined,
      page: page,
      pageSize: 12,
      ...overrides,
    };
  };

  const handleSearch = (value) => {
    setSearch(value);
    setPage(1);
    fetchProducts(createOptions({ name: value || undefined, page: 1 }));
  };

  const handleSkuChange = (value) => {
    setSku(value);
    setPage(1);
    fetchProducts(createOptions({ sku: value || undefined, page: 1 }));
  };

  const handleCategoryChange = (value) => {
    setCategoryId(value);
    setPage(1);
    fetchProducts(createOptions({ categoryId: value || undefined, page: 1 }));
  };

  const handlePriceChange = (min, max) => {
    setMinPrice(min);
    setMaxPrice(max);
    setPage(1);
    fetchProducts(
      createOptions({
        minPrice: min ? parseFloat(min) : undefined,
        maxPrice: max ? parseFloat(max) : undefined,
        page: 1,
      })
    );
  };

  const handleSort = (nextOrderBy, nextDirection) => {
    setOrderBy(nextOrderBy ?? null);
    setSortDirection(nextDirection ?? null);
    setPage(1);
    fetchProducts(
      createOptions({ orderBy: nextOrderBy ?? undefined, sortDirection: nextDirection ?? undefined, page: 1 })
    );
  };

  const handlePageChange = (newPage) => {
    setPage(newPage);
    fetchProducts(createOptions({ page: newPage }));
  };

  const handleResetFilters = () => {
    setSearch('');
    setSku('');
    setCategoryId(null);
    setMinPrice('');
    setMaxPrice('');
    setOrderBy(null);
    setSortDirection(null);
    setPage(1);
    fetchProducts({ page: 1, pageSize: 12 });
  };

  const openProduct = (id) => {
    navigate(`/marketplace/product/${id}`);
  };

  return (
    <div className="catalog-layout">
      <PatientSidebar />

      <div className="catalog-content">
        <div className="catalog-header">
          <h1 className="catalog-title">Каталог товарів</h1>

          <div className="catalog-filters">
            <Input
              className="name-input"
              placeholder="Пошук по назві"
              value={search}
              onChange={(e) => handleSearch(e.target.value)}
            />
            <Input
              className="sku-input"
              placeholder="Пошук по артикулу"
              value={sku}
              onChange={(e) => handleSkuChange(e.target.value)}
            />
            {/* TODO: Додати щоб категорії підгружались з API */}
            <Select
              className="category-select"
              placeholder="Виберіть категорію"
              value={categoryId}
              onChange={(value) => handleCategoryChange(value)}
              allowClear
              options={categoryOptions}
            />
            <Input
              className="min-price"
              placeholder="Мінімальна ціна"
              type="number"
              value={minPrice}
              onChange={(e) => handlePriceChange(e.target.value, maxPrice)}
            />
            <Input
              className="max-price"
              placeholder="Максимальна ціна"
              type="number"
              value={maxPrice}
              onChange={(e) => handlePriceChange(minPrice, e.target.value)}
            />
            {/* TODO: Додати щоб опції підгружалися з api */}
            <Select
              className="order-by"
              placeholder="Сортувати за"
              value={orderBy ?? undefined}
              onChange={(value) => handleSort(value, sortDirection)}
              allowClear
              options={[
                { value: 'Name', label: 'Назва' },
                { value: 'Price', label: 'Ціна' },
                { value: 'Reviews', label: 'Відгуки' },
              ]}
            />
            <Select
              className="order-by"
              placeholder="Напрямок"
              value={sortDirection ?? undefined}
              onChange={(value) => handleSort(orderBy, value)}
              allowClear
              options={[
                { value: 'Ascending', label: 'За зростанням' },
                { value: 'Descending', label: 'За спаданням' },
              ]}
            />
            <Button className="reset-btn" type="primary" onClick={handleResetFilters}>
              Скинути фільтри
            </Button>
          </div>
        </div>

        {productsLoading ? (
          <Skeleton active />
        ) : total === 0 ? (
          <Empty description="Нічого не знайдено" />
        ) : (
          <div className="catalog-grid">
            {products.map((product) => (
              <Card key={product.id} className="product-card" onClick={() => openProduct(product.id)}>
                <div className="product-image">
                  {product.imageUrl ? (
                    <img src={product.imageUrl} alt={product.name} loading="lazy" />
                  ) : (
                    <div className="product-image__placeholder">Фото</div>
                  )}
                </div>
                <Meta 
                  title={product.name} 
                  description={
                    <div className="review-meta">
                      <p className="product-price">{product.price} грн</p>
                      <div className="rating-row">
                        <Rate disabled allowHalf value={Number(product.averageRating) || 0}></Rate>
                        <span>({product.reviewsCount})</span>
                      </div>
                    </div>
                  }
                />
                <div className="sku-text">SKU: {product.sku}</div>
              </Card>
            ))}
          </div>
        )}

        <div className="catalog-pagination">
          <Pagination current={page} pageSize={12} onChange={handlePageChange} total={total} />
        </div>
      </div>
    </div>
  );
}
