import { Card, Col, Row, Statistic, Typography, Divider, Table, Progress, Badge, Tag, Spin, Alert } from 'antd'
import { ArrowUpOutlined, ArrowDownOutlined, DollarOutlined, FileTextOutlined } from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { RecordedEntry, SelectOption } from '../lib/types'

const { Title, Text } = Typography

function sum(entries: RecordedEntry[]) {
  return entries.reduce((acc, e) => acc + e.amount, 0)
}

function byCategory(entries: RecordedEntry[]) {
  const map = new Map<string, number>()
  for (const e of entries) {
    const key = e.category ?? '(未分类)'
    map.set(key, (map.get(key) ?? 0) + e.amount)
  }
  return Array.from(map.entries())
    .sort((a, b) => Math.abs(b[1]) - Math.abs(a[1]))
    .slice(0, 10)
}

function byType(entries: RecordedEntry[]) {
  const map = new Map<string, number>()
  for (const e of entries) {
    map.set(e.type, (map.get(e.type) ?? 0) + 1)
  }
  return Array.from(map.entries())
    .sort((a, b) => b[1] - a[1])
    .slice(0, 5)
}

export default function Dashboard() {
  const now = new Date()
  const start = new Date(now)
  start.setDate(now.getDate() - 30)
  const option: SelectOption = {
    startTime: start.toISOString(),
    endTime: now.toISOString(),
    direction: null,
    category: null,
  }

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['dashboard', option],
    queryFn: () => api.select(option),
  })

  const entries = data ?? []
  const total = sum(entries)
  const income = entries.filter((e) => e.amount > 0).reduce((a, b) => a + b.amount, 0)
  const outcome = entries.filter((e) => e.amount < 0).reduce((a, b) => a + b.amount, 0)
  const incomeCount = entries.filter((e) => e.amount > 0).length
  const outcomeCount = entries.filter((e) => e.amount < 0).length

  if (isLoading) {
    return <div style={{ textAlign: 'center', padding: '50px' }}><Spin size="large" tip="加载中..." /></div>
  }

  if (isError) {
    return <Alert message="错误" description={String(error)} type="error" showIcon />
  }

  return (
    <div>
      <Title level={3}>
        <DollarOutlined /> 仪表盘概览
      </Title>
      <Text type="secondary">最近 30 天的财务数据统计</Text>

      <Divider />

      <Row gutter={[16, 16]}>
        <Col xs={24} sm={12} lg={6}>
          <Card hoverable>
            <Statistic
              title={<Text strong>净额</Text>}
              value={total}
              precision={2}
              valueStyle={{ color: total >= 0 ? '#3f8600' : '#cf1322' }}
              prefix={total >= 0 ? <ArrowUpOutlined /> : <ArrowDownOutlined />}
              suffix="元"
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card hoverable style={{ borderColor: '#52c41a' }}>
            <Statistic
              title={<Text strong>收入总计</Text>}
              value={income}
              precision={2}
              valueStyle={{ color: '#3f8600' }}
              prefix={<ArrowUpOutlined />}
              suffix="元"
            />
            <Text type="secondary" style={{ fontSize: '12px' }}>共 {incomeCount} 笔</Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card hoverable style={{ borderColor: '#ff4d4f' }}>
            <Statistic
              title={<Text strong>支出总计</Text>}
              value={Math.abs(outcome)}
              precision={2}
              valueStyle={{ color: '#cf1322' }}
              prefix={<ArrowDownOutlined />}
              suffix="元"
            />
            <Text type="secondary" style={{ fontSize: '12px' }}>共 {outcomeCount} 笔</Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card hoverable>
            <Statistic
              title={<Text strong>总条目数</Text>}
              value={entries.length}
              prefix={<FileTextOutlined />}
              suffix="笔"
            />
            <Text type="secondary" style={{ fontSize: '12px' }}>
              {income > 0 && Math.abs(outcome) > 0
                ? `收入占比 ${((income / (income + Math.abs(outcome))) * 100).toFixed(1)}%`
                : '暂无数据'}
            </Text>
          </Card>
        </Col>
      </Row>

      <Divider />

      <Row gutter={[16, 16]}>
        <Col xs={24} lg={14}>
          <Card title={<Title level={4} style={{ margin: 0 }}>按分类统计 Top 10</Title>} bordered={false}>
            <Table
              size="small"
              pagination={false}
              dataSource={byCategory(entries).map(([category, amount], idx) => ({
                key: category,
                rank: idx + 1,
                category,
                amount
              }))}
              columns={[
                {
                  title: '排名',
                  dataIndex: 'rank',
                  width: 60,
                  render: (rank: number) => (
                    <Badge
                      count={rank}
                      style={{ backgroundColor: rank <= 3 ? '#faad14' : '#d9d9d9' }}
                    />
                  )
                },
                {
                  title: '分类',
                  dataIndex: 'category',
                  render: (cat: string) => <Tag color="blue">{cat}</Tag>
                },
                {
                  title: '金额合计',
                  dataIndex: 'amount',
                  sorter: (a, b) => a.amount - b.amount,
                  render: (amount: number) => (
                    <Text strong style={{ color: amount >= 0 ? '#3f8600' : '#cf1322' }}>
                      {amount >= 0 ? '+' : ''}{amount.toFixed(2)} 元
                    </Text>
                  )
                },
                {
                  title: '占比',
                  dataIndex: 'amount',
                  render: (amount: number) => {
                    const totalAbs = income + Math.abs(outcome)
                    const percent = totalAbs > 0 ? (Math.abs(amount) / totalAbs) * 100 : 0
                    return (
                      <div style={{ width: 100 }}>
                        <Progress
                          percent={Number(percent.toFixed(1))}
                          size="small"
                          status={amount >= 0 ? 'success' : 'exception'}
                        />
                      </div>
                    )
                  }
                }
              ]}
            />
          </Card>
        </Col>

        <Col xs={24} lg={10}>
          <Card title={<Title level={4} style={{ margin: 0 }}>最常使用的类型 Top 5</Title>} bordered={false}>
            <Table
              size="small"
              pagination={false}
              dataSource={byType(entries).map(([type, count]) => ({ key: type, type, count }))}
              columns={[
                {
                  title: '类型',
                  dataIndex: 'type',
                  render: (type: string) => <Tag color="green">{type}</Tag>
                },
                {
                  title: '使用次数',
                  dataIndex: 'count',
                  render: (count: number) => <Text strong>{count} 次</Text>
                },
              ]}
            />
          </Card>

          <Card
            title={<Title level={4} style={{ margin: 0 }}>快速统计</Title>}
            bordered={false}
            style={{ marginTop: 16 }}
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
              <div>
                <Text type="secondary">平均每日收入：</Text>
                <Text strong style={{ color: '#3f8600', marginLeft: 8 }}>
                  {(income / 30).toFixed(2)} 元
                </Text>
              </div>
              <div>
                <Text type="secondary">平均每日支出：</Text>
                <Text strong style={{ color: '#cf1322', marginLeft: 8 }}>
                  {(Math.abs(outcome) / 30).toFixed(2)} 元
                </Text>
              </div>
              <div>
                <Text type="secondary">平均每笔金额：</Text>
                <Text strong style={{ marginLeft: 8 }}>
                  {entries.length > 0 ? ((income + Math.abs(outcome)) / entries.length).toFixed(2) : 0} 元
                </Text>
              </div>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  )
}