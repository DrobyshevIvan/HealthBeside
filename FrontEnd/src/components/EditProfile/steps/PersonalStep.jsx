// steps/PersonalStep.jsx
import { Form, Input, DatePicker, Select } from 'antd';

export default function PersonalStep() {
  return (
    <>
      <Form.Item name="lastName" label="Прізвище*" rules={[{ required: true }]}>
        <Input />
      </Form.Item>
      <Form.Item name="firstName" label="Ім’я*" rules={[{ required: true }]}>
        <Input />
      </Form.Item>
      <Form.Item name="dateOfBirth" label="Дата народження*" rules={[{ required: true }]}>
        <DatePicker format="DD.MM.YYYY" style={{ width: '100%' }} />
      </Form.Item>
      {/* <Form.Item name="gender" label="Стать*" rules={[{ required: true }]}>
        <Select options={[{ value:'female', label:'Жіноча' }, { value:'male', label:'Чоловіча' }]} />
      </Form.Item> */}
      {/* <Form.Item name="city" label="Місто">
        <Input placeholder="Введіть населений пункт..." />
      </Form.Item> */}
    </>
  );
}