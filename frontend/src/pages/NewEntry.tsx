import { Button, Card, DatePicker, Form, Input, InputNumber, Select, Space, Typography, message, Row, Col, Divider } from 'antd'
import { PlusOutlined, SaveOutlined } from '@ant-design/icons'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { z } from 'zod'
import { api } from '../lib/api'
import type { Entry, Category } from '../lib/types'

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
      givenTime: new Date() as any,
    })
  }

  const quickFillExpense = () => {
    form.setFieldsValue({
      amount: -100,
      givenTime: new Date() as any,
    })
  }

  return (
    <div>
      <Title level={3}>
        <PlusOutlined /> 新建条目
      </Title>
      <Text type="secondary">添加新的收入或支出记录</Text>

      <Row gutter={16} style={{ marginTop: 16 }}>
        <Col xs={24} lg={16}>
          <Card
            title={
              <Space>
                <SaveOutlined />
                <span>条目信息</span>
              </Space>
            }
          >
            <Form
              form={form}
              layout="vertical"
              onFinish={onFinish}
              initialValues={{ givenTime: new Date() as any }}
            >
              <Form.Item
                name="amount"
                label="金额"
                rules={[{ required: true, message: '请输入金额' }]}
                extra="正数表示收入，负数表示支出"
              >
                <InputNumber
                  style={{ width: '100%' }}
                  placeholder="例如: 1000 或 -50.5"
                  step={0.01}
                  precision={2}
                  addonAfter="元"
                />
              </Form.Item>

              <Form.Item
                name="givenTime"
                label="时间"
                rules={[{ required: true, message: '请选择时间' }]}
              >
                <DatePicker showTime style={{ width: '100%' }} format="YYYY-MM-DD HH:mm:ss" />
              </Form.Item>

              <Form.Item
                name="type"
                label="类型"
                rules={[{ required: true, message: '请输入类型' }]}
                extra="例如: 餐饮、工资、交通等"
              >
                <Input
                  style={{ width: '100%' }}
                  placeholder="请输入条目类型"
                  maxLength={50}
                  showCount
                />
              </Form.Item>

              <Form.Item
                name="category"
                label="分类"
                extra="选择一个已有分类，或留空使用类型的默认分类"
              >
                <Select
                  allowClear
                  style={{ width: '100%' }}
                  placeholder="选择分类（可选）"
                  options={(categories ?? []).map((c: Category) => ({
                    label: c.name,
                    value: c.name,
                  }))}
                />
              </Form.Item>

              <Form.Item name="description" label="描述" extra="可选的备注信息">
                <Input.TextArea
                  rows={4}
                  placeholder="添加更多说明..."
                  maxLength={500}
                  showCount
                />
              </Form.Item>

              <Form.Item>
                <Space>
                  <Button
                    type="primary"
                    htmlType="submit"
                    loading={create.isPending}
                    icon={<SaveOutlined />}
                    size="large"
                  >
                    提交
                  </Button>
                  <Button onClick={() => form.resetFields()} size="large">
                    重置
                  </Button>
                </Space>
              </Form.Item>
            </Form>
          </Card>
        </Col>

        <Col xs={24} lg={8}>
          <Card title="快速操作" bordered={false}>
            <Space direction="vertical" style={{ width: '100%' }}>
              <Button type="dashed" block onClick={quickFillIncome}>
                快速填充收入示例
              </Button>
              <Button type="dashed" block onClick={quickFillExpense}>
                快速填充支出示例
              </Button>
            </Space>

            <Divider />

            <div style={{ fontSize: '13px', color: '#666' }}>
              <Text strong style={{ display: 'block', marginBottom: 8 }}>
                操作提示：
              </Text>
              <ul style={{ paddingLeft: 20, margin: 0 }}>
                <li>金额：正数代表收入，负数代表支出</li>
                <li>类型：如果是新类型且提供了分类，系统会自动创建该类型</li>
                <li>分类：可选，留空则使用类型的默认分类</li>
                <li>提交成功后表单会自动重置</li>
              </ul>
            </div>
          </Card>

          <Card title="最近分类" bordered={false} style={{ marginTop: 16 }}>
            {categories && categories.length > 0 ? (
              <div style={{ maxHeight: 200, overflow: 'auto' }}>
                {categories.slice(0, 10).map((cat: Category) => (
                  <div key={cat.name} style={{ padding: '4px 0' }}>
                    <Text>• {cat.name}</Text>
                    {cat.superCategory && (
                      <Text type="secondary" style={{ marginLeft: 8, fontSize: '12px' }}>
                        ({cat.superCategory})
                      </Text>
                    )}
                  </div>
                ))}
              </div>
            ) : (
              <Text type="secondary">暂无分类</Text>
            )}
          </Card>
        </Col>
      </Row>
    </div>
  )
}