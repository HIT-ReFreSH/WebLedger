import { Button, Card, DatePicker, Form, Input, InputNumber, Select, Space, Typography, message, Row, Col, Divider } from 'antd'
import { PlusOutlined, SaveOutlined } from '@ant-design/icons'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { z } from 'zod'
import { api } from '../lib/api'
import type { Entry, Category } from '../lib/types'
import dayjs from 'dayjs'

const { Title, Text } = Typography

const schema = z.object({
  amount: z.number().finite(),
  givenTime: z.date(),
  type: z.string().min(1, '类型不能为空'),
  category: z.string().optional(),
  description: z.string().optional(),
})

export default function NewEntry() {
  const [form] = Form.useForm()
  const queryClient = useQueryClient()
  const { data: categories } = useQuery({ queryKey: ['categories'], queryFn: api.getCategories })

  const create = useMutation({
    mutationFn: async (entry: Entry) => api.insertEntry(entry),
    onSuccess: (eid) => {
      message.success(`创建成功！条目 ID: ${eid}`)
      form.resetFields()
      queryClient.invalidateQueries({ queryKey: ['entries'] })
      queryClient.invalidateQueries({ queryKey: ['dashboard'] })
    },
    onError: (e) => message.error(`创建失败: ${String(e)}`),
  })

  const onFinish = (values: any) => {
    const parse = schema.safeParse({
      amount: values.amount,
      givenTime: values.givenTime?.toDate?.() ?? values.givenTime,
      type: values.type,
      category: values.category,
      description: values.description,
    })
    if (!parse.success) {
      message.error(parse.error.issues.map((i) => i.message).join('; '))
      return
    }
    const e: Entry = {
      amount: parse.data.amount,
      givenTime: parse.data.givenTime.toISOString(),
      type: parse.data.type,
      category: parse.data.category,
      description: parse.data.description,
    }
    create.mutate(e)
  }

  const quickFillIncome = () => {
    form.setFieldsValue({
      amount: 1000,
      givenTime: dayjs(),
    })
  }

  const quickFillExpense = () => {
    form.setFieldsValue({
      amount: -100,
      givenTime: dayjs(),
    })
  }

  return (
    <div style={{ width: '100%', maxWidth: '1400px', margin: '0 auto' }}>
      <div style={{
        marginBottom: 32,
        padding: '24px',
        background: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
        borderRadius: '16px',
        boxShadow: '0 8px 24px rgba(79, 172, 254, 0.25)'
      }}>
        <Title level={2} style={{ marginBottom: 8, color: '#fff' }}>
          <PlusOutlined /> 新建条目
        </Title>
        <Text style={{ fontSize: '15px', color: 'rgba(255, 255, 255, 0.9)' }}>
          添加新的收入或支出记录
        </Text>
      </div>

      <Row gutter={[24, 24]}>
        <Col xs={24} lg={15}>
          <Card
            title={
              <Space>
                <SaveOutlined />
                <Text strong style={{ fontSize: '16px', color: '#2d3748' }}>条目信息</Text>
              </Space>
            }
            style={{
              borderRadius: '16px',
              boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
              border: 'none'
            }}
          >
            <Form
              form={form}
              layout="vertical"
              onFinish={onFinish}
              initialValues={{ givenTime: dayjs() }}
            >
              <Form.Item
                name="amount"
                label={<Text strong style={{ fontSize: '15px' }}>金额</Text>}
                rules={[{ required: true, message: '请输入金额' }]}
                extra={<Text type="secondary">正数表示收入，负数表示支出</Text>}
              >
                <InputNumber
                  style={{ width: '100%', borderRadius: '8px' }}
                  placeholder="例如: 1000 或 -50.5"
                  step={0.01}
                  precision={2}
                  addonAfter="元"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="givenTime"
                label={<Text strong style={{ fontSize: '15px' }}>时间</Text>}
                rules={[{ required: true, message: '请选择时间' }]}
              >
                <DatePicker
                  showTime
                  style={{ width: '100%', borderRadius: '8px' }}
                  format="YYYY-MM-DD HH:mm:ss"
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="type"
                label={<Text strong style={{ fontSize: '15px' }}>类型</Text>}
                rules={[{ required: true, message: '请输入类型' }]}
                extra={<Text type="secondary">例如: 餐饮、工资、交通等</Text>}
              >
                <Input
                  style={{ width: '100%', borderRadius: '8px' }}
                  placeholder="请输入条目类型"
                  maxLength={50}
                  showCount
                  size="large"
                />
              </Form.Item>

              <Form.Item
                name="category"
                label={<Text strong style={{ fontSize: '15px' }}>分类</Text>}
                extra={<Text type="secondary">选择一个已有分类，或留空使用类型的默认分类</Text>}
              >
                <Select
                  allowClear
                  style={{ width: '100%', borderRadius: '8px' }}
                  placeholder="选择分类（可选）"
                  size="large"
                  options={(categories ?? []).map((c: Category) => ({
                    label: c.name,
                    value: c.name,
                  }))}
                />
              </Form.Item>

              <Form.Item
                name="description"
                label={<Text strong style={{ fontSize: '15px' }}>描述</Text>}
                extra={<Text type="secondary">可选的备注信息</Text>}
              >
                <Input.TextArea
                  rows={4}
                  placeholder="添加更多说明..."
                  maxLength={500}
                  showCount
                  style={{ borderRadius: '8px' }}
                />
              </Form.Item>

              <Form.Item>
                <Space size="middle">
                  <Button
                    type="primary"
                    htmlType="submit"
                    loading={create.isPending}
                    icon={<SaveOutlined />}
                    size="large"
                    style={{ borderRadius: '8px', paddingLeft: 32, paddingRight: 32 }}
                  >
                    提交
                  </Button>
                  <Button
                    onClick={() => form.resetFields()}
                    size="large"
                    style={{ borderRadius: '8px', paddingLeft: 32, paddingRight: 32 }}
                  >
                    重置
                  </Button>
                </Space>
              </Form.Item>
            </Form>
          </Card>
        </Col>

        <Col xs={24} lg={9}>
          <Card
            title={<Text strong style={{ fontSize: '16px', color: '#2d3748' }}>快速操作</Text>}
            bordered={false}
            style={{
              borderRadius: '16px',
              boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
              background: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)'
            }}
          >
            <Space direction="vertical" style={{ width: '100%' }} size="middle">
              <Button
                type="dashed"
                block
                onClick={quickFillIncome}
                size="large"
                style={{ borderRadius: '8px', height: '48px' }}
              >
                快速填充收入示例
              </Button>
              <Button
                type="dashed"
                block
                onClick={quickFillExpense}
                size="large"
                style={{ borderRadius: '8px', height: '48px' }}
              >
                快速填充支出示例
              </Button>
            </Space>

            <Divider />

            <div style={{
              fontSize: '14px',
              color: '#2d3748',
              padding: '16px',
              background: 'rgba(255, 255, 255, 0.7)',
              borderRadius: '8px'
            }}>
              <Text strong style={{ display: 'block', marginBottom: 12, fontSize: '15px' }}>
                操作提示：
              </Text>
              <ul style={{ paddingLeft: 20, margin: 0, lineHeight: '1.8' }}>
                <li>金额：正数代表收入，负数代表支出</li>
                <li>类型：如果是新类型且提供了分类，系统会自动创建该类型</li>
                <li>分类：可选，留空则使用类型的默认分类</li>
                <li>提交成功后表单会自动重置</li>
              </ul>
            </div>
          </Card>

          <Card
            title={<Text strong style={{ fontSize: '16px', color: '#2d3748' }}>最近分类</Text>}
            bordered={false}
            style={{
              marginTop: 24,
              borderRadius: '16px',
              boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
              background: 'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)'
            }}
          >
            {categories && categories.length > 0 ? (
              <div style={{
                maxHeight: 280,
                overflow: 'auto',
                padding: '12px',
                background: 'rgba(255, 255, 255, 0.7)',
                borderRadius: '8px'
              }}>
                {categories.slice(0, 10).map((cat: Category) => (
                  <div
                    key={cat.name}
                    style={{
                      padding: '8px 12px',
                      marginBottom: '8px',
                      background: '#fff',
                      borderRadius: '6px',
                      boxShadow: '0 2px 4px rgba(0, 0, 0, 0.05)'
                    }}
                  >
                    <Text style={{ fontSize: '14px' }}>• {cat.name}</Text>
                    {cat.superCategory && (
                      <Text type="secondary" style={{ marginLeft: 8, fontSize: '13px' }}>
                        ({cat.superCategory})
                      </Text>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <div style={{
                padding: '20px',
                textAlign: 'center',
                background: 'rgba(255, 255, 255, 0.7)',
                borderRadius: '8px'
              }}>
                <Text type="secondary">暂无分类</Text>
              </div>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  )
}