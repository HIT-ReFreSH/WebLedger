import { useMemo, useState } from 'react'
import { Button, DatePicker, Radio, Select, Space, Table, Typography, Popconfirm, message, Card, Tag, Statistic, Row, Col, Spin, Alert } from 'antd'
import { ReloadOutlined, DeleteOutlined, FilterOutlined } from '@ant-design/icons'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { RecordedEntry, SelectOption, Category } from '../lib/types'

const { RangePicker } = DatePicker
const { Title, Text } = Typography

export default function Entries() {
  const queryClient = useQueryClient()
  const [direction, setDirection] = useState<boolean | null>(null)
  const [category, setCategory] = useState<string | null>(null)
  const [range, setRange] = useState<[Date, Date]>(() => {
    const end = new Date()
    const start = new Date()
    start.setMonth(end.getMonth() - 1)
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
    <div>
      <Title level={3}>
        <FilterOutlined /> 条目列表
      </Title>
      <Text type="secondary">查询、筛选和管理您的财务条目</Text>

      <Card style={{ marginTop: 16 }} bordered={false}>
        <Space direction="vertical" size="middle" style={{ width: '100%' }}>
          <Space wrap>
            <RangePicker
              showTime
              value={[range[0] as any, range[1] as any]}
              onChange={(v) => {
                const s = v?.[0]?.toDate() ?? range[0]
                const e = v?.[1]?.toDate() ?? range[1]
                setRange([s, e])
              }}
            />
            <Radio.Group value={direction} onChange={(e) => setDirection(e.target.value)}>
              <Radio.Button value={null}>全部</Radio.Button>
              <Radio.Button value={true}>收入</Radio.Button>
              <Radio.Button value={false}>支出</Radio.Button>
            </Radio.Group>
            <Select
              allowClear
              placeholder="按分类筛选"
              style={{ minWidth: 160 }}
              value={category ?? undefined}
              onChange={(v) => setCategory(v ?? null)}
              options={(categories ?? []).map((c: Category) => ({ label: c.name, value: c.name }))}
            />
            <Button
              type="primary"
              icon={<ReloadOutlined />}
              onClick={() => queryClient.invalidateQueries({ queryKey: ['entries'] })}
            >
              刷新
            </Button>
          </Space>

          <Row gutter={16}>
            <Col xs={24} sm={8}>
              <Card size="small">
                <Statistic
                  title="查询结果"
                  value={rows.length}
                  suffix="笔"
                  valueStyle={{ fontSize: '20px' }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card size="small">
                <Statistic
                  title="收入"
                  value={incomeAmount}
                  precision={2}
                  suffix="元"
                  valueStyle={{ color: '#3f8600', fontSize: '20px' }}
                />
              </Card>
            </Col>
            <Col xs={24} sm={8}>
              <Card size="small">
                <Statistic
                  title="支出"
                  value={Math.abs(outcomeAmount)}
                  precision={2}
                  suffix="元"
                  valueStyle={{ color: '#cf1322', fontSize: '20px' }}
                />
              </Card>
            </Col>
          </Row>

          {isLoading ? (
            <div style={{ textAlign: 'center', padding: '30px' }}>
              <Spin tip="加载中..." />
            </div>
          ) : (
            <Table
              rowKey={(r) => r.id}
              dataSource={rows}
              pagination={{ pageSize: 10, showSizeChanger: true, showTotal: (total) => `共 ${total} 条` }}
              scroll={{ x: 800 }}
              columns={[
                {
                  title: '时间',
                  dataIndex: 'givenTime',
                  sorter: (a, b) => new Date(a.givenTime).getTime() - new Date(b.givenTime).getTime(),
                  render: (t) => new Date(t).toLocaleString('zh-CN'),
                  width: 180,
                },
                {
                  title: '类型',
                  dataIndex: 'type',
                  render: (type: string) => <Tag color="geekblue">{type}</Tag>,
                },
                {
                  title: '分类',
                  dataIndex: 'category',
                  render: (cat: string) => cat ? <Tag color="purple">{cat}</Tag> : <Text type="secondary">-</Text>,
                },
                {
                  title: '金额',
                  dataIndex: 'amount',
                  sorter: (a, b) => a.amount - b.amount,
                  render: (amount: number) => (
                    <Text strong style={{ color: amount >= 0 ? '#3f8600' : '#cf1322' }}>
                      {amount >= 0 ? '+' : ''}{amount.toFixed(2)} 元
                    </Text>
                  ),
                },
                {
                  title: '描述',
                  dataIndex: 'description',
                  ellipsis: true,
                  render: (desc: string) => desc || <Text type="secondary">-</Text>,
                },
                {
                  title: '操作',
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
                      <Button danger size="small" icon={<DeleteOutlined />}>
                        删除
                      </Button>
                    </Popconfirm>
                  ),
                },
              ]}
            />
          )}
        </Space>
      </Card>
    </div>
  )
}