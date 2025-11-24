import { Button, Card, Form, Input, Space, Table, Typography, Popconfirm, message, Tag, Spin, Alert } from 'antd'
import { PlusOutlined, DeleteOutlined, FolderOutlined, SaveOutlined } from '@ant-design/icons'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { Category } from '../lib/types'

const { Title, Text } = Typography

export default function Categories() {
  const [form] = Form.useForm()
  const queryClient = useQueryClient()
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['categories'],
    queryFn: api.getCategories,
  })

  const save = useMutation({
    mutationFn: (c: Category) => api.addOrUpdateCategory(c),
    onSuccess: () => {
      message.success('保存成功！')
      queryClient.invalidateQueries({ queryKey: ['categories'] })
      form.resetFields()
    },
    onError: (e) => message.error(`保存失败: ${String(e)}`),
  })

  const remove = useMutation({
    mutationFn: (name: string) => api.removeCategory(name),
    onSuccess: () => {
      message.success('删除成功！')
      queryClient.invalidateQueries({ queryKey: ['categories'] })
    },
    onError: (e) => message.error(`删除失败: ${String(e)}`),
  })

  const onFinish = (values: any) => {
    const c: Category = {
      name: values.name,
      superCategory: values.superCategory || null,
    }
    save.mutate(c)
  }

  const rows = data ?? []

  if (isError) {
    return <Alert message="错误" description={String(error)} type="error" showIcon />
  }

  return (
    <div style={{ width: '100%', maxWidth: '1400px', margin: '0 auto' }}>
      <div style={{
        marginBottom: 32,
        padding: '24px',
        background: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
        borderRadius: '16px',
        boxShadow: '0 8px 24px rgba(250, 112, 154, 0.25)'
      }}>
        <Title level={2} style={{ marginBottom: 8, color: '#fff' }}>
          <FolderOutlined /> 分类管理
        </Title>
        <Text style={{ fontSize: '15px', color: 'rgba(255, 255, 255, 0.9)' }}>
          管理您的财务分类和层级结构
        </Text>
      </div>

      <Space direction="vertical" size="large" style={{ width: '100%' }}>
        <Card
          title={
            <Space>
              <PlusOutlined />
              <Text strong style={{ fontSize: '16px', color: '#2d3748' }}>新增/更新分类</Text>
            </Space>
          }
          style={{
            borderRadius: '16px',
            boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
            border: 'none',
            background: 'linear-gradient(135deg, #e0c3fc 0%, #8ec5fc 100%)'
          }}
        >
          <div style={{
            padding: '20px',
            background: 'rgba(255, 255, 255, 0.9)',
            borderRadius: '12px'
          }}>
            <Form layout="inline" form={form} onFinish={onFinish}>
              <Form.Item
                name="name"
                rules={[{ required: true, message: '请输入分类名' }]}
                style={{ marginBottom: '16px' }}
              >
                <Input
                  placeholder="分类名称（必填）"
                  style={{ width: 240, borderRadius: '8px' }}
                  prefix={<FolderOutlined />}
                  size="large"
                />
              </Form.Item>
              <Form.Item name="superCategory" style={{ marginBottom: '16px' }}>
                <Input
                  placeholder="父分类（可选）"
                  style={{ width: 240, borderRadius: '8px' }}
                  size="large"
                />
              </Form.Item>
              <Form.Item style={{ marginBottom: '16px' }}>
                <Button
                  type="primary"
                  htmlType="submit"
                  loading={save.isPending}
                  icon={<SaveOutlined />}
                  size="large"
                  style={{ borderRadius: '8px', paddingLeft: 32, paddingRight: 32 }}
                >
                  保存
                </Button>
              </Form.Item>
            </Form>
          </div>

          <div style={{
            marginTop: 20,
            padding: '16px',
            background: 'rgba(255, 255, 255, 0.7)',
            borderRadius: '8px'
          }}>
            <Text strong style={{ fontSize: '14px', color: '#2d3748', display: 'block', marginBottom: 8 }}>
              提示：
            </Text>
            <ul style={{ margin: '8px 0 0 0', paddingLeft: 20, color: '#4a5568', lineHeight: '1.8' }}>
              <li>如果分类名称已存在，将会更新该分类的信息</li>
              <li>父分类用于建立分类的层级关系</li>
              <li>删除分类前请确保没有条目使用该分类</li>
            </ul>
          </div>
        </Card>

        {isLoading ? (
          <div style={{
            textAlign: 'center',
            padding: '60px',
            background: 'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)',
            borderRadius: '16px'
          }}>
            <Spin size="large" tip="加载中..." />
          </div>
        ) : (
          <Card
            title={
              <Text strong style={{ fontSize: '16px', color: '#2d3748' }}>
                分类列表 (共 {rows.length} 个)
              </Text>
            }
            style={{
              borderRadius: '16px',
              boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
              border: 'none'
            }}
          >
            <Table
              rowKey={(r) => r.name}
              dataSource={rows}
              pagination={{
                pageSize: 10,
                showSizeChanger: true,
                showTotal: (total) => `共 ${total} 个分类`
              }}
              style={{ borderRadius: '12px' }}
              columns={[
                {
                  title: <Text strong>分类名称</Text>,
                  dataIndex: 'name',
                  sorter: (a, b) => a.name.localeCompare(b.name),
                  render: (name: string) => (
                    <Space>
                      <FolderOutlined style={{ color: '#667eea', fontSize: '16px' }} />
                      <Tag
                        color="blue"
                        style={{
                          fontSize: '14px',
                          padding: '4px 12px',
                          borderRadius: '6px',
                          fontWeight: 500
                        }}
                      >
                        {name}
                      </Tag>
                    </Space>
                  ),
                },
                {
                  title: <Text strong>父分类</Text>,
                  dataIndex: 'superCategory',
                  render: (superCat: string | null) =>
                    superCat ? (
                      <Tag
                        color="purple"
                        style={{
                          fontSize: '14px',
                          padding: '4px 12px',
                          borderRadius: '6px'
                        }}
                      >
                        {superCat}
                      </Tag>
                    ) : (
                      <Text type="secondary" style={{ fontSize: '14px' }}>顶级分类</Text>
                    ),
                },
                {
                  title: <Text strong>操作</Text>,
                  dataIndex: 'name',
                  width: 120,
                  render: (name: string) => (
                    <Popconfirm
                      title="确认删除"
                      description={`确定要删除分类 "${name}" 吗？`}
                      onConfirm={() => remove.mutate(name)}
                      okText="确定"
                      cancelText="取消"
                    >
                      <Button
                        danger
                        size="small"
                        icon={<DeleteOutlined />}
                        loading={remove.isPending}
                        style={{ borderRadius: '6px' }}
                      >
                        删除
                      </Button>
                    </Popconfirm>
                  ),
                },
              ]}
            />
          </Card>
        )}
      </Space>
    </div>
  )
}