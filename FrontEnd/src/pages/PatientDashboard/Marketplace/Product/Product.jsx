import PatientSidebar from '../../../../components/PatientSidebar/PatientSidebar'
import { useParams } from 'react-router-dom'
import { useProductItem } from '../../../../hooks/useProductItem'
import './Product.css'
import {Skeleton, Empty, Button, InputNumber, Card, Input, Select, Rate, Avatar, Tag, Descriptions, Typography, Badge, message} from 'antd'
import React, { useState } from 'react';
import { useReviews } from '../../../../hooks/useReviews'
import { useAddToCart } from '../../../../hooks/Cart/useAddToCart'

const { Meta } = Card;
const { Title, Paragraph, Text } = Typography;

export default function Product() {
  const { id } = useParams();

  const { product, loading: productLoading } = useProductItem(id);
  const { reviews, loading: reviewsLoading, fetchReviews } = useReviews({ productId: id });
  const { addToCart, loading: addingToCart } = useAddToCart();

  const [quantity, setQuantity] = useState(1);

  const [description, setDescription] = useState('');
  const [rating, setRating] = useState(null);
  const [orderBy, setOrderBy] = useState(null);
  const [sortDirection, setSortDirection] = useState(null);

  const [messageApi, contextHolder] = message.useMessage();

  const createOptions = (overrides = {}) => {
    return {
      description: description || undefined,
      rating: rating ?? undefined,
      orderBy: orderBy ?? undefined,
      sortDirection: sortDirection ?? undefined,
      ...overrides,
    };
  };

  const handleDescriptionChange = (value) => {
    setDescription(value);
    fetchReviews(createOptions({ productId: id, description: value }));
  };

  const handleRatingChange = (value) => {
    setRating(value);
    fetchReviews(createOptions({ productId: id, rating: value }));
  };

  const handleSort = (nextOrderBy, nextDirection) => {
    setOrderBy(nextOrderBy ?? null);
    setSortDirection(nextDirection ?? null);
    fetchReviews(createOptions({ productId: id, sortDirection: nextDirection, orderBy: nextOrderBy }));
  }

  const handleResetFilters = () => {
    setOrderBy(null);
    setSortDirection(null);
    setDescription('');
    setRating(null);
    fetchReviews({ productId: id });
  }

  const handleAddToCart = async (productId, quantity) => {
    try {
      await addToCart({ productId, quantity });
      messageApi.success(`${product?.name} Успішно додано до кошика!`);
    } catch (e) {
      console.error("Error in adding product to cart", e);
      messageApi.error(`Не вдалося додати товар до кошика. ${e?.response?.data?.message || e?.message || 'Спробуйте ще раз.'}`);
    }
  };

  const formatDate = (dateStr) => {
    try {
      const date = new Date(dateStr);
      return new Intl.DateTimeFormat('uk-UA', { dateStyle: 'medium', timeStyle: 'short' }).format(date);
    } catch {
      return '';
    }
  }

  const formatPrice = (p) =>
    new Intl.NumberFormat('uk-UA', { style: 'currency', currency: 'UAH', maximumFractionDigits: 0 }).format(p ?? 0);

  return (
    <div className="product-layout">
      {contextHolder}
      <PatientSidebar />

      <div className="product-content">
        <h1 className="product-title">Товар</h1>
        {productLoading ? (
          <Skeleton active />
        ) : product === null ? (
          <Empty description="Товар не знайдений" />
        ) : (
          <div className="product-details">
            <div className="product-image">
              {product.imageUrl ? (
                <img src={product.imageUrl} alt={product.name} loading="lazy" />
              ) : (
                <div className="product-image__placeholder">Фото</div>
              )}
            </div>
            <div className="product-info">
              <Title level={2} className="product-name">{product.name}</Title>
              <Paragraph className="product-price">{formatPrice(product.price)}</Paragraph>
              <Paragraph className="product-description">{product.description}</Paragraph>

              <div className="product-meta">
                <Tag color="geekblue">SKU: {product.sku}</Tag>
                {product.category?.name && <Tag color="green">Категорія: {product.category.name}</Tag>}
                <Badge count={product.quantity} color="#000" style={{ backgroundColor: '#111' }}>
                  <Tag className="qty-tag">В наявності</Tag>
                </Badge>
              </div>

              <div className="product-buy__container">
                <InputNumber className="product-buy__quantity" value={quantity} onChange={setQuantity} min={1} max={Math.max(1, product.quantity || 100)} />
                <Button 
                  className="product-buy__button" 
                  type="primary" 
                  disabled={product.quantity === 0} 
                  loading={addingToCart}
                  onClick={() => handleAddToCart(id, quantity)}>
                  {addingToCart ? 'Додаємо...' : 'Додати до кошика'}
                </Button>
              </div>
            </div>
          </div>
        )}
        <div className="product-reviews">
          <div className="product-reviews-header">
            <h1 className="reviews-title">Відгуки</h1>
            <div className="reviews-filters">
              <Input
                className="description-input"
                placeholder="Пошук по опису"
                value={description}
                onChange={(e) => handleDescriptionChange(e.target.value)}
                allowClear
              />
              <Select
                className="rating-select"
                placeholder="Рейтинг"
                value={rating ?? undefined}
                onChange={(value) => handleRatingChange(value)}
                allowClear
                options={[
                  {value: null, label: 'Будь-який'},
                  {value: 1, label: '1'},
                  {value: 2, label: '2'},
                  {value: 3, label: '3'},
                  {value: 4, label: '4'},
                  {value: 5, label: '5'},
                ]}
              />
              <Select
                className="order-by"
                placeholder="Сортувати за"
                value={orderBy ?? undefined}
                onChange={(value) => handleSort(value, sortDirection)}
                allowClear
                options={[
                  {value: 'Rating', label: 'Рейтинг'},
                  {value: 'CreatedOn', label: 'Дата відгуку'}
                ]}
              />
              <Select
                className="order-by"
                placeholder="Напрямок"
                value={sortDirection ?? undefined}
                onChange={(value) => handleSort(orderBy, value)}
                allowClear
                options={[
                  {value: 'Ascending', label: 'За зростанням'},
                  {value: 'Descending', label: 'За спаданням'}
                ]}
              />
              <Button className="reset-btn" type="primary" onClick={handleResetFilters}>
                Скинути фільтри
              </Button>
            </div>
          </div>

          {reviewsLoading ? (
            <Skeleton active />
          ) : reviews.length === 0 ? (
            <Empty description="Ніхто ще не залишив відгук" />
          ) : (
            <div className="reviews-details">
              {reviews.map((review) => (
                <Card key={review.id} className="review-card">
                  <Meta
                    avatar={<Avatar>{review.user?.firstName?.[0] ?? 'U'}</Avatar>}
                    title={review.user ? `${review.user.firstName ?? ''} ${review.user.lastName ?? ''}`.trim() : 'Користувач'}
                    description={
                      <div className="review-meta">
                        <div className="review-row">
                          <Rate disabled value={review.rating} />
                          <Tag color="blue" className="review-date">{formatDate(review.createdOn)}</Tag>
                        </div>
                        <div className="review-description">{review.description}</div>
                      </div>
                    }
                  />
                </Card>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  )
}