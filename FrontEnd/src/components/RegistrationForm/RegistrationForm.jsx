import { Button, Form, Input, Select, DatePicker, message } from "antd";
import { useMemo, useState } from "react";
import dayjs from "dayjs";
import { authService } from "../../services/authService";

const ROLE_IDS = {
    USER: "098f240f-01cf-4925-ba10-4983724f73c3",
    DOCTOR: "665c40a7-46ec-4564-a679-755f77c90472",
    PATIENT: "c1167d0e-ff05-4df7-bfb5-69baf52c174f",
};

const ROLE_OPTIONS = [
    { label: "Звичайний користувач", value: ROLE_IDS.USER, profileType: "User" },
    { label: "Лікар", value: ROLE_IDS.DOCTOR, profileType: "Doctor" },
    { label: "Пацієнт", value: ROLE_IDS.PATIENT, profileType: "Patient" },
];

export default function RegistrationForm() {
    const [form] = Form.useForm();
    const [loading, setLoading] = useState(false);

    const roleId = Form.useWatch('roleId', form);
    const selectedRole = useMemo(() => ROLE_OPTIONS.find(r => r.value === roleId), [roleId]);

    const onFinish = async (values) => {
        setLoading(true);
        try {
            const payload = { ...values };

            if (payload.dateOfBirth && dayjs.isDayjs(payload.dateOfBirth)) {
                payload.dateOfBirth = payload.dateOfBirth.toISOString();
            }

            if (payload.yearsOfExperience) payload.yearsOfExperience = Number(payload.yearsOfExperience);

            if (selectedRole?.profileType !== 'Patient') {
                delete payload.dateOfBirth;
                delete payload.medicalHistorySummary;
            }
            if (selectedRole?.profileType !== 'Doctor') {
                delete payload.specialization;
                delete payload.medicalLicenseNumber;
                delete payload.yearsOfExperience;
                delete payload.education;
                delete payload.biography;
            }

            await authService.register(payload);
            message.success('Реєстрація пройшла успішно. Ви можете увійти.');
            form.resetFields();
        } catch (err) {
            console.error('Registration error', err);
            const text = err?.response?.data?.message || err.message || 'Помилка реєстрації';
            message.error(text);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Form form={form} layout="vertical" onFinish={onFinish} initialValues={{ roleId: ROLE_IDS.USER }}>
            <Form.Item name="roleId" label="Оберіть роль" rules={[{ required: true, message: 'Оберіть роль' }]}>
                <Select options={ROLE_OPTIONS} placeholder="Виберіть..." />
            </Form.Item>

            <Form.Item name="email" label="Email" rules={[{ required: true, type: 'email', message: 'Введіть коректний Email' }]}>
                <Input />
            </Form.Item>

            <Form.Item name="password" label="Пароль" rules={[{ required: true, min: 8, message: 'Пароль має бути не менше 8 символів' }]}>
                <Input.Password />
            </Form.Item>

            <Form.Item name="firstName" label="Ім'я" rules={[{ required: true, message: 'Введіть ім\'я' }]}>
                <Input />
            </Form.Item>

            <Form.Item name="lastName" label="Прізвище" rules={[{ required: true, message: 'Введіть прізвище' }]}>
                <Input />
            </Form.Item>

            {selectedRole?.profileType === 'Patient' && (
                <>
                    <Form.Item name="dateOfBirth" label="Дата народження" rules={[{ required: true, message: 'Введіть дату народження' }]}>
                        <DatePicker style={{ width: '100%' }} />
                    </Form.Item>
                    <Form.Item name="medicalHistorySummary" label="Медична історія">
                        <Input.TextArea rows={3} />
                    </Form.Item>
                </>
            )}

            {selectedRole?.profileType === 'Doctor' && (
                <>
                    <Form.Item name="specialization" label="Спеціалізація" rules={[{ required: true, message: 'Введіть спеціалізацію' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="medicalLicenseNumber" label="Номер мед. ліцензії" rules={[{ required: true, message: 'Введіть номер ліцензії' }]}>
                        <Input />
                    </Form.Item>
                    <Form.Item name="yearsOfExperience" label="Досвід (роки)" rules={[{ required: true, message: 'Введіть досвід' }]}>
                        <Input type="number" />
                    </Form.Item>
                    <Form.Item name="education" label="Освіта" rules={[{ required: true, message: 'Введіть освіту' }]}>
                        <Input.TextArea rows={2} />
                    </Form.Item>
                    <Form.Item name="biography" label="Біографія">
                        <Input.TextArea rows={3} />
                    </Form.Item>
                </>
            )}

            <Form.Item>
                <Button type="primary" htmlType="submit" loading={loading}>
                    Зареєструватися
                </Button>
            </Form.Item>
        </Form>
    );
}
