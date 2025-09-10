import React, { useState } from 'react';
import { Button, Descriptions, Avatar } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { useAuth } from '../../../hooks/useAuth';
import PatientSidebar from '../../../components/PatientSidebar/PatientSidebar';
import './PatientProfile.css';
import EditProfileModal from '../../../components/EditProfile/EditProfileModal';

export default function PatientProfile() {
  const { user, logout } = useAuth();

  const [isEditOpen, setIsEditOpen] = useState(false);

  const handleOpenEdit = () => setIsEditOpen(true);
  const handleCloseEdit = () => setIsEditOpen(false);

  const handleSubmitEdit = async (submittedValues) => {
    // TODO: call API to update profile, then refresh user info

    // await api.updateProfile(submittedValues)
    console.log('Edit submit:', submittedValues);
    setIsEditOpen(false);
  };

  const handleLogout = () => {
    logout();
  }

  // Форматування дати народження
  const formatDateOfBirth = (dateString) => {
    if (!dateString) return '—';
    const date = new Date(dateString);
    const age = new Date().getFullYear() - date.getFullYear();
    return `${date.toLocaleDateString('uk-UA')} (${age} років)`;
  };

  return (
    <div className="patient-profile-layout">
      <PatientSidebar />
      
      <div className="profile-content">
        <div className="profile-header">
          <div className="profile-info">
            <Avatar 
              size={80} 
              icon={<UserOutlined />} 
              className="profile-avatar"
            />
            <div className="profile-details">
              <h1 className="profile-name">
                {user?.firstName + " " + user?.lastName || '—'}
              </h1>
              <div className="profile-basic-info">
                <p><strong>Дата народження:</strong> {formatDateOfBirth(user?.dateOfBirth)}</p>
                <p><strong>Стать:</strong> {user?.gender || '—'}</p>
                <p><strong>Місто:</strong> {user?.city || '—'}</p>
              </div>
            </div>
          </div>
          <div className="profile-actions">
            <Button type="primary" className="edit-profile-btn" onClick={handleOpenEdit}>
                Редагувати профіль
            </Button>
            <Button type="primary" className="logout-profile-btn" onClick={handleLogout}>
                Вийти
            </Button>
          </div>
        </div>

        <div className="profile-sections">
          <div className="profile-section">
            <h2 className="section-title">Контактна інформація</h2>
            <Descriptions column={1} bordered>
              <Descriptions.Item label="Email">
                {user?.email || '—'}
              </Descriptions.Item>
              <Descriptions.Item label="Номер телефону">
                {user?.phoneNumber || '—'}
              </Descriptions.Item>
            </Descriptions>
          </div>

          <div className="profile-section">
            <h2 className="section-title">Медичні відомості</h2>
            <div className="medical-info">
              <p className="medical-summary">
                {user?.medicalHistorySummary || 'Відсутні'}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Edit profile modal */}
      <EditProfileModal
        open={isEditOpen}
        onClose={handleCloseEdit}
        onSubmit={handleSubmitEdit}
        initialValues={{
          firstName: user?.firstName,
          lastName: user?.lastName,
          middleName: user?.middleName,
          dateOfBirth: user?.dateOfBirth,
          gender: user?.gender,
          city: user?.city,
          medicalHistorySummary: user?.medicalHistorySummary,
        }}
      />
    </div>
  );
}