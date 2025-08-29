import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import App from './App.jsx'
import { AuthProvider } from './context/AuthContext.jsx'
import Home from './pages/Home.jsx'
import Login from './pages/Login.jsx'
import ProtectedRoute from './routes/ProtectedRoute.jsx'
import UserProfile from './pages/UserProfile/UserProfile.jsx'
import Authorization from "./pages/Authorization/Authorization.jsx";

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
      { path: 'user-info', element: (
        <ProtectedRoute>
          <UserProfile />
        </ProtectedRoute>
      ) }, // /user-info (без початкового '/')
    ],
  },
])

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
)
