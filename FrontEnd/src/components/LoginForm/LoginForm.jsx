import {Button, Form, Input} from "antd";
import {useNavigate} from "react-router-dom";
import {useAuth} from "../../hooks/useAuth.js";

export default function LoginForm() {
    const navigate = useNavigate();
    const { login, loading} = useAuth();

    const onFinish = async ({email, password}) => {
        try {
            await login({email, password});
            navigate('/user-info', { replace: true });
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