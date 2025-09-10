import PatientSidebar from '../../../../components/PatientSidebar/PatientSidebar'
import { useParams } from 'react-router-dom'
import { useProductItem } from '../../../../hooks/useProductItem'

export default function Product() {
  const { id } = useParams();

  const { product, loading, error, fetchProduct } = useProductItem(id);

  if(loading) return <div>Завантаження...</div>
  if(error) return <div>Помилка: {error}</div>
  if(!product) return <div>Товар не знайдений</div>

  console.log("Product is in Product.jsx", product);

  return (
    <div className="product-layout">
      <PatientSidebar />

      <div className="product-content">
        <h1 className="product-title">Товар</h1>
        <div className="product-details">
          <div className="product-image">
            {product.imageUrl ? (
              <img src={product.imageUrl} alt={product.name} loading="lazy" />
            ) : (
              <div className="product-image__placeholder">Фото</div>
            )}
          </div>
          <div className="product-info">
            <h2 className="product-name">{product.name}</h2>
            <p className="product-price">{product.price}</p>
          </div>
        </div>
      </div>
    </div>
  )
}