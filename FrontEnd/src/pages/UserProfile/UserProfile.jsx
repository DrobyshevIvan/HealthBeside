import {useAuth} from "../../hooks/useAuth.js";
import {Avatar, Card, Descriptions, Result, Skeleton} from "antd";
import LoginForm from "../../components/LoginForm/LoginForm.jsx";
import {UserOutlined} from "@ant-design/icons";

export default function UserProfile() {
    const { user, isAuthenticated, loading } = useAuth();

    if (loading) return <Skeleton active avatar paragraph={{rows: 4}}/>;
    if (!isAuthenticated) return <Result status="403" titile="Увійдіть, будь ласка"/>;

    return (
        <div className="profile-wrapper">
            <Card
                className="profile-card"
                title="Мій профіль"
                extra={<Avatar size={40} icon={<UserOutlined />} />}
            >
                <Descriptions column={2} bordered size="middle">
                    <Descriptions.Item label="Ім'я та прізвище">{user?.fullName ?? '-'}</Descriptions.Item>
                    <Descriptions.Item label="Email" span={2}>{user?.email ?? '—'}</Descriptions.Item>
                    <Descriptions.Item label="ID" span={2}><code>{user?.id ?? '—'}</code></Descriptions.Item>
                </Descriptions>
            </Card>
        </div>
    )
}