import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import App from './App.jsx'
import { AuthProvider } from './context/AuthContext.jsx'
import Home from './pages/Home/Home.jsx'
import ProtectedRoute from './routes/ProtectedRoute.jsx'
import Authorization from "./pages/Authorization/Authorization.jsx";
import PatientProfile from './pages/PatientDashboard/Profile/PatientProfile.jsx'
import Catalog from './pages/PatientDashboard/Marketplace/Catalog/Catalog.jsx'
// import DoctorProfile from './pages/DoctorDashboard/Profile/DoctorProfile.jsx'
import Product from './pages/PatientDashboard/Marketplace/Product/Product.jsx'
import Cart from './pages/PatientDashboard/Marketplace/Cart/Cart.jsx'

const router = createBrowserRouter([
  {
    path: '/',
    element: (
      <AuthProvider>
        <App />
      </AuthProvider>
    ),
    children: [
      { index: true, element: <Home /> },   // /
      { path: 'authorization', element: <Authorization /> },
      { path: 'patient/profile', element: (
        <ProtectedRoute>
          <PatientProfile />
        </ProtectedRoute>
      ) },
      { path: 'marketplace', element: <Catalog /> },
      { path: 'marketplace/product/:id', element: <Product /> },
      { path: 'marketplace/cart', element: (
        <ProtectedRoute>
          <Cart />
        </ProtectedRoute>
      ) },
      // { path: 'doctor/profile', element: (
      //   <ProtectedRoute>
      //     <DoctorProfile />
      //   </ProtectedRoute>
      // ) },
    ],
  },
])

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
