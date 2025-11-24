import { useMemo, useState } from 'react'
import { Button, DatePicker, Radio, Select, Space, Table, Typography, Popconfirm, message, Card, Tag, Statistic, Row, Col, Spin, Alert } from 'antd'
import { ReloadOutlined, DeleteOutlined, FilterOutlined } from '@ant-design/icons'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { RecordedEntry, SelectOption, Category } from '../lib/types'
import dayjs, { Dayjs } from 'dayjs'

const { RangePicker } = DatePicker
const { Title, Text } = Typography

export default function Entries() {
  const queryClient = useQueryClient()
  const [direction, setDirection] = useState<boolean | null>(null)
  const [category, setCategory] = useState<string | null>(null)
  const [range, setRange] = useState<[Dayjs, Dayjs]>(() => {
    const end = dayjs()
    const start = dayjs().subtract(1, 'month')
    return [start, end]
  })

  const option: SelectOption = useMemo(() => ({
    startTime: range[0].toISOString(),
    endTime: range[1].toISOString(),
    direction,
    category,
  }), [direction, category, range])

  const { data: categories } = useQuery({ queryKey: ['categories'], queryFn: api.getCategories })
  const { data, isLoading, isError, error } = useQuery({ queryKey: ['entries', option], queryFn: () => api.select(option) })

  const del = useMutation({
    mutationFn: (id: string) => api.deleteEntry(id),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['entries'] }); message.success('已删除') },
    onError: (e) => message.error(String(e)),
  })

  const rows: RecordedEntry[] = data ?? []
  const totalAmount = rows.reduce((sum, r) => sum + r.amount, 0)
  const incomeAmount = rows.filter(r => r.amount > 0).reduce((sum, r) => sum + r.amount, 0)
  const outcomeAmount = rows.filter(r => r.amount < 0).reduce((sum, r) => sum + r.amount, 0)

  if (isError) {
    return <Alert message="错误" description={String(error)} type="error" showIcon />
  }

  return (
    <div style={{ width: '100%', maxWidth: '1600px', margin: '0 auto' }}>
      <div style={{
        marginBottom: 32,
        padding: '24px',
        background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
        borderRadius: '16px',
        boxShadow: '0 8px 24px rgba(240, 147, 251, 0.25)'
      }}>
        <Title level={2} style={{ marginBottom: 8, color: '#fff' }}>
          <FilterOutlined /> 条目列表
        </Title>
        <Text style={{ fontSize: '15px', color: 'rgba(255, 255, 255, 0.9)' }}>
          查询、筛选和管理您的财务条目
        </Text>
      </div>

      <Card
        bordered={false}
        style={{
          borderRadius: '16px',
          boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
          overflow: 'hidden'
        }}
      >
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          <div style={{
            padding: '20px',
            background: 'linear-gradient(135deg, #e0c3fc 0%, #8ec5fc 100%)',
            borderRadius: '12px'
          }}>
            <Space wrap size="middle">
              <RangePicker
                showTime
                value={range}
                onChange={(v) => {
                  if (v && v[0] && v[1]) {
                    setRange([v[0], v[1]])
                  }
                }}
                style={{ borderRadius: '8px' }}
              />
              <Radio.Group value={direction} onChange={(e) => setDirection(e.target.value)}>
                <Radio.Button value={null} style={{ borderRadius: '8px 0 0 8px' }}>全部</Radio.Button>
                <Radio.Button value={true}>收入</Radio.Button>
                <Radio.Button value={false} style={{ borderRadius: '0 8px 8px 0' }}>支出</Radio.Button>
              </Radio.Group>
              <Select
                allowClear
                placeholder="按分类筛选"
                style={{ minWidth: 180, borderRadius: '8px' }}
                value={category ?? undefined}
                onChange={(v) => setCategory(v ?? null)}
                options={(categories ?? []).map((c: Category) => ({ label: c.name, value: c.name }))}
              />
              <Button
                type="primary"
                icon={<ReloadOutlined />}
                onClick={() => queryClient.invalidateQueries({ queryKey: ['entries'] })}
                style={{ borderRadius: '8px' }}
                size="large"
              >
                刷新
              </Button>
            </Space>
          </div>

          <Row gutter={[16, 16]}>
            <Col xs={24} sm={8}>
              <Card
                size="small"
                style={{
                  borderRadius: '12px',
                  background: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)',
                  border: 'none',
                  boxShadow: '0 2px 8px rgba(0, 0, 0, 0.06)'
                }}
              >
                <Statistic
                  title={<Text strong style={{ color: '#2d3748' }}>查询结果</Text>}
                  value={rows.length}
                  suffix="笔"
                  valueStyle={{ fontSize: '24px', color: '#2d3748', fontWeight: 700 }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card
                size="small"
                style={{
                  borderRadius: '12px',
                  background: 'linear-gradient(135deg, #d4fc79 0%, #96e6a1 100%)',
                  border: 'none',
                  boxShadow: '0 2px 8px rgba(0, 0, 0, 0.06)'
                }}
              >
                <Statistic
                  title={<Text strong style={{ color: '#2d3748' }}>收入</Text>}
                  value={incomeAmount}
                  precision={2}
                  suffix="元"
                  valueStyle={{ color: '#22543d', fontSize: '24px', fontWeight: 700 }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card
                size="small"
                style={{
                  borderRadius: '12px',
                  background: 'linear-gradient(135deg, #ffeaa7 0%, #fdcb6e 100%)',
                  border: 'none',
                  boxShadow: '0 2px 8px rgba(0, 0, 0, 0.06)'
                }}
              >
                <Statistic
                  title={<Text strong style={{ color: '#2d3748' }}>支出</Text>}
                  value={Math.abs(outcomeAmount)}
                  precision={2}
                  suffix="元"
                  valueStyle={{ color: '#c53030', fontSize: '24px', fontWeight: 700 }}
                />
              </Card>
            </Col>
          </Row>

          {isLoading ? (
            <div style={{
              textAlign: 'center',
              padding: '60px',
              background: 'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)',
              borderRadius: '12px'
            }}>
              <Spin size="large" tip="加载中..." />
            </div>
          ) : (
            <div style={{ marginTop: '24px' }}>
              <Table
                rowKey={(r) => r.id}
                dataSource={rows}
                pagination={{
                  pageSize: 10,
                  showSizeChanger: true,
                  showTotal: (total) => `共 ${total} 条`,
                  style: { marginTop: '24px' }
                }}
                scroll={{ x: 800 }}
                style={{ borderRadius: '12px' }}
                columns={[
                  {
                    title: <Text strong>时间</Text>,
                    dataIndex: 'givenTime',
                    sorter: (a, b) => new Date(a.givenTime).getTime() - new Date(b.givenTime).getTime(),
                    render: (t) => (
                      <Text style={{ fontSize: '14px' }}>
                        {new Date(t).toLocaleString('zh-CN')}
                      </Text>
                    ),
                    width: 180,
                  },
                  {
                    title: <Text strong>类型</Text>,
                    dataIndex: 'type',
                    render: (type: string) => (
                      <Tag color="geekblue" style={{ fontSize: '13px', padding: '4px 10px', borderRadius: '6px' }}>
                        {type}
                      </Tag>
                    ),
                  },
                  {
                    title: <Text strong>分类</Text>,
                    dataIndex: 'category',
                    render: (cat: string) => cat ? (
                      <Tag color="purple" style={{ fontSize: '13px', padding: '4px 10px', borderRadius: '6px' }}>
                        {cat}
                      </Tag>
                    ) : <Text type="secondary">-</Text>,
                  },
                  {
                    title: <Text strong>金额</Text>,
                    dataIndex: 'amount',
                    sorter: (a, b) => a.amount - b.amount,
                    render: (amount: number) => (
                      <Text strong style={{
                        color: amount >= 0 ? '#22543d' : '#c53030',
                        fontSize: '15px',
                        fontWeight: 600
                      }}>
                        {amount >= 0 ? '+' : ''}{amount.toFixed(2)} 元
                      </Text>
                    ),
                  },
                  {
                    title: <Text strong>描述</Text>,
                    dataIndex: 'description',
                    ellipsis: true,
                    render: (desc: string) => desc || <Text type="secondary">-</Text>,
                  },
                  {
                    title: <Text strong>操作</Text>,
                    dataIndex: 'id',
                    fixed: 'right',
                    width: 100,
                    render: (id: string) => (
                      <Popconfirm
                        title="确认删除"
                        description="删除后无法恢复，确定要删除此条目吗？"
                        onConfirm={() => del.mutate(id)}
                        okText="确定"
                        cancelText="取消"
                      >
                        <Button
                          danger
                          size="small"
                          icon={<DeleteOutlined />}
                          style={{ borderRadius: '6px' }}
                        >
                          删除
                        </Button>
                      </Popconfirm>
                    ),
                  },
                ]}
              />
            </div>
          )}
        </Space>
      </Card>
    </div>
  )
}