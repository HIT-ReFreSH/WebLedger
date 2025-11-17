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
    <div>
      <Title level={3}>
        <FolderOutlined /> 分类管理
      </Title>
      <Text type="secondary">管理您的财务分类和层级结构</Text>

      <Space direction="vertical" size="large" style={{ width: '100%', marginTop: 16 }}>
        <Card
          title={
            <Space>
              <PlusOutlined />
              <span>新增/更新分类</span>
            </Space>
          }
        >
          <Form layout="inline" form={form} onFinish={onFinish}>
            <Form.Item
              name="name"
              rules={[{ required: true, message: '请输入分类名' }]}
            >
              <Input
                placeholder="分类名称（必填）"
                style={{ width: 200 }}
                prefix={<FolderOutlined />}
              />
            </Form.Item>
            <Form.Item name="superCategory">
              <Input
                placeholder="父分类（可选）"
                style={{ width: 200 }}
              />
            </Form.Item>
            <Form.Item>
              <Button
                type="primary"
                htmlType="submit"
                loading={save.isPending}
                icon={<SaveOutlined />}
              >
                保存
              </Button>
            </Form.Item>
          </Form>

          <div style={{ marginTop: 16, padding: 12, background: '#f5f5f5', borderRadius: 4 }}>
            <Text type="secondary" style={{ fontSize: '13px' }}>
              <strong>提示：</strong>
              <ul style={{ margin: '8px 0 0 0', paddingLeft: 20 }}>
                <li>如果分类名称已存在，将会更新该分类的信息</li>
                <li>父分类用于建立分类的层级关系</li>
                <li>删除分类前请确保没有条目使用该分类</li>
              </ul>
            </Text>
          </div>
        </Card>

        {isLoading ? (
          <div style={{ textAlign: 'center', padding: '30px' }}>
            <Spin size="large" tip="加载中..." />
          </div>
        ) : (
          <Card title={`分类列表 (共 ${rows.length} 个)`}>
            <Table
              rowKey={(r) => r.name}
              dataSource={rows}
              pagination={{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `共 ${total} 个分类` }}
              columns={[
                {
                  title: '分类名称',
                  dataIndex: 'name',
                  sorter: (a, b) => a.name.localeCompare(b.name),
                  render: (name: string) => (
                    <Space>
                      <FolderOutlined style={{ color: '#1890ff' }} />
                      <Tag color="blue">{name}</Tag>
                    </Space>
                  ),
                },
                {
                  title: '父分类',
                  dataIndex: 'superCategory',
                  render: (superCat: string | null) =>
                    superCat ? (
                      <Tag color="purple">{superCat}</Tag>
                    ) : (
                      <Text type="secondary">顶级分类</Text>
                    ),
                },
                {
                  title: '操作',
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