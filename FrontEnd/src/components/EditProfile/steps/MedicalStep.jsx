// steps/MedicalStep.jsx
import { Form, Input } from 'antd';
export default function MedicalStep() {
  return (
    <Form.Item name="medicalHistorySummary" label="Медичні відомості">
      <Input.TextArea rows={6} />
    </Form.Item>
  );
}