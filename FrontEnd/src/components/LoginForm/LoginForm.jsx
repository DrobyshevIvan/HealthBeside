import {Button, Form, Input} from "antd";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../../hooks/useAuth.js";
import { useEffect } from "react";

export default function LoginForm() {
    const navigate = useNavigate();
    const { login, loading, user } = useAuth();

    useEffect(() => {
        if (user && !loading) {
            const roles = Array.isArray(user.roles)
                ? user.roles
                : (user.role ? [user.role] : []);

            if (roles.some(r => /^(Patient|User)$/i.test(r))) {
                navigate('/patient/profile', { replace: true });
            } else if (roles.some(r => /^Doctor$/i.test(r))) {
                navigate('/doctor/profile', { replace: true });
            } else {
                console.log('Fallback to user-info, roles were:', roles);
                navigate('/patient/profile', { replace: true });
            }
        }
    }, [user, loading, navigate]);

    const onFinish = async ({email, password}) => {
        try {
            await login({email, password});
        } catch (err) {
            console.log(err);
        }
    }

    return (
        <Form layout='vertical' onFinish={onFinish}>
            <Form.Item name="email" label="Email" rules={[{required: true, type: 'email'}]}>
                <Input/>
            </Form.Item>
            <Form.Item name="password" label="Пароль" rules={[{required: true}]}>
                <Input.Password />
            </Form.Item>
            <Button type='primary' htmlType='submit' loading={loading}>
                Увійти
            </Button>
        </Form>
    )
}