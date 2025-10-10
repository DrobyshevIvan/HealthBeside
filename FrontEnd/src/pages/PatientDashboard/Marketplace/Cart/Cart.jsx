import PatientSidebar from '../../../../components/PatientSidebar/PatientSidebar'
import { useCart } from '../../../../hooks/Cart/useCart'
import './Cart.css'
import { Button, Card, Divider, Empty, InputNumber, List, Typography, Skeleton } from 'antd'
import React from 'react'
import { useRemoveFromCart } from '../../../../hooks/Cart/useRemoveFromCart'
import { useUpdateCartItem } from '../../../../hooks/Cart/useUpdateCartItem'

const { Title, Text } = Typography

export default function Cart() {
  const { cart, loading: cartLoading, refetchCart } = useCart();
  const { removeFromCart } = useRemoveFromCart();
  const { updateCartItem } = useUpdateCartItem();

  const formatPrice = (p) =>
    new Intl.NumberFormat('uk-UA', { style: 'currency', currency: 'UAH', maximumFractionDigits: 0 }).format(p ?? 0);

  const items = (cart?.cartItems ?? []).map(ci => {
    const p = ci?.product ?? {};
    return {
      id: ci?.id,               // cartItemId (не використовується для видалення, але залишимо)
      productId: p?.id,         // потрібен і для видалення, і для апдейту
      quantity: ci?.quantity ?? 0,
      name: p?.name ?? 'Товар',
      price: p?.price ?? 0,
      sku: p?.sku ?? '',
      imageUrl: p?.imageUrl ?? '',
      totalPrice: ci?.totalPrice ?? 0,
    };
  });

  if (cartLoading) {
    return (
      <div className="cart-layout">
        <PatientSidebar />
        <div className="cart-content">
          <Title level={2} className="cart-title">Кошик</Title>
          <Skeleton active />
        </div>
      </div>
    );
  }

  const handleCartItemRemove = async (productId) => {
    try {
      await removeFromCart({ id: productId });    // бек чекає productId
      await refetchCart();                          // це функція з useCart
    } catch (e) {
      console.error('Error in removing cart item', e);
    }
  };

  const handleQuantityChange = async (productId, nextQty) => {
    if (!productId || !nextQty) return;
    try {
      await updateCartItem({ productId, quantity: nextQty });
      await refetchCart();
    } catch (e) {
      console.error('Error updating quantity', e);
    }
  };

  return (
    <div className="cart-layout">
      <PatientSidebar />
      <div className="cart-content">
        <Title level={2} className="cart-title">Кошик</Title>

        {items.length === 0 ? (
          <Empty description="Кошик порожній" />
        ) : (
          <div className="cart-grid">
            <Card className="cart-list" bordered>
              <List
                itemLayout="horizontal"
                dataSource={items}
                renderItem={(item) => (
                  <List.Item>
                    <List.Item.Meta
                      avatar={
                        <div className="cart-image">
                          {item.imageUrl
                            ? <img src={item.imageUrl} alt={item.name} loading="lazy" />
                            : <div className="cart-image__placeholder">Фото</div>}
                        </div>
                      }
                      title={
                        <div className="cart-item-header">
                          <span className="cart-item-name">{item.name}</span>
                          <div className="cart-right">
                            <div className="cart-item-prices">
                              <div className="cart-unit">Ціна: {formatPrice(item.price)}</div>
                              <div className="cart-total">Разом: {formatPrice(item.price * item.quantity)}</div>
                            </div>
                            <div className="qty-wrap">
                              <span className="qty-label">Кількість:</span>
                              <InputNumber
                                min={1}
                                value={item.quantity}
                                onChange={(v) => handleQuantityChange(item.productId, v)}
                                className="qty-input"
                              />
                            </div>
                            <Button danger onClick={() => handleCartItemRemove(item.productId)}>Видалити</Button>
                          </div>
                        </div>
                      }
                      description={<div className="cart-item-sub">SKU: {item.sku}</div>}
                    />
                  </List.Item>
                )}
              />
            </Card>

            <Card className="cart-summary" bordered>
              <Title level={4}>Підсумок</Title>
              <div className="summary-row">
                <Text>Сума</Text>
                <Text strong>{formatPrice(cart?.totalPrice ?? 0)}</Text>
              </div>
              <div className="summary-row">
                <Text>Доставка</Text>
                <Text strong>{formatPrice(0)}</Text>
              </div>
              <Divider />
              <div className="summary-row total">
                <Text strong>До сплати</Text>
                <Text strong>{formatPrice(cart?.totalPrice ?? 0)}</Text>
              </div>
              <Button type="primary" className="checkout-btn" block>Перейти до оформлення</Button>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
}
