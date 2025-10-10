// EditProfileModal.jsx
import { Modal, Steps, Form, Button } from 'antd';
import PersonalStep from './steps/PersonalStep';
import MedicalStep from './steps/MedicalStep';
import { useState } from 'react';

export default function EditProfileModal({ open, onClose, initialValues, onSubmit }) {
  const [form] = Form.useForm();
  const [current, setCurrent] = useState(0);

  const steps = [
    { title: '1/2', content: <PersonalStep /> },
    { title: '2/2', content: <MedicalStep /> },
  ];

  const next = async () => {
    await form.validateFields();
    setCurrent((i) => i + 1);
  };
  const prev = () => setCurrent((i) => i - 1);

  const handleOk = async () => {
    await form.validateFields();
    // ВАЖЛИВО: зібрати всі значення, включно з полями з попереднього кроку
    const values = form.getFieldsValue(true);
    const payload = {
      ...values,
    };
    onSubmit(payload);
  };

  return (
    <Modal open={open} onCancel={onClose} footer={null} width={880} destroyOnClose title={`Редагувати профіль — ${steps[current].title}`}>
      <Steps current={current} items={steps.map(s => ({ title: s.title }))} style={{ marginBottom: 16 }} />
      <Form form={form} layout="vertical" initialValues={initialValues} preserve>
        {steps[current].content}
        <div style={{ display:'flex', justifyContent:'flex-end', gap:8, marginTop:16 }}>
          {current > 0 && <Button onClick={prev}>Назад</Button>}
          {current < steps.length - 1 ? (
            <Button type="primary" onClick={next}>Далі</Button>
          ) : (
            <Button type="primary" onClick={handleOk}>Зберегти</Button>
          )}
        </div>
      </Form>
    </Modal>
  );
}