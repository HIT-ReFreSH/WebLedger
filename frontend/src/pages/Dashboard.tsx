import { Card, Col, Row, Statistic, Typography, Table, Progress, Badge, Tag, Spin, Alert, Empty } from 'antd'
import { ArrowUpOutlined, ArrowDownOutlined, DollarOutlined, FileTextOutlined } from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { RecordedEntry, SelectOption } from '../lib/types'
import { useMemo } from 'react'

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
  const option: SelectOption = useMemo(() => {
    const now = new Date()
    const start = new Date(now)
    start.setDate(now.getDate() - 30)
    return {
      startTime: start.toISOString(),
      endTime: now.toISOString(),
      direction: null,
      category: null,
    }
  }, [])

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
  const hasData = entries.length > 0

  if (isLoading) {
    return <div style={{ textAlign: 'center', padding: '50px' }}><Spin size="large" tip="加载中..." /></div>
  }

  if (isError) {
    return <Alert message="错误" description={String(error)} type="error" showIcon />
  }

  return (
    <div style={{ width: '100%', maxWidth: '1600px', margin: '0 auto' }}>
      <div style={{
        marginBottom: 32,
        padding: '24px',
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        borderRadius: '16px',
        boxShadow: '0 8px 24px rgba(102, 126, 234, 0.25)'
      }}>
        <Title level={2} style={{ marginBottom: 8, color: '#fff' }}>
          <DollarOutlined /> 仪表盘概览
        </Title>
        <Text style={{ fontSize: '15px', color: 'rgba(255, 255, 255, 0.9)' }}>
          最近 30 天的财务数据统计
        </Text>
      </div>

      <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              border: 'none',
              background: total >= 0 ? 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)' : 'linear-gradient(135deg, #ffeaa7 0%, #fdcb6e 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px' }}>净额</Text>}
              value={total}
              precision={2}
              valueStyle={{
                color: total >= 0 ? '#2d3748' : '#c53030',
                fontSize: '28px',
                fontWeight: 700
              }}
              prefix={total >= 0 ? <ArrowUpOutlined /> : <ArrowDownOutlined />}
              suffix="元"
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(82, 196, 26, 0.15)',
              border: 'none',
              background: 'linear-gradient(135deg, #d4fc79 0%, #96e6a1 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>收入总计</Text>}
              value={income}
              precision={2}
              valueStyle={{ color: '#22543d', fontSize: '28px', fontWeight: 700 }}
              prefix={<ArrowUpOutlined />}
              suffix="元"
            />
            <Text style={{ fontSize: '13px', color: '#2d3748', marginTop: '8px', display: 'block' }}>
              共 {incomeCount} 笔
            </Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(255, 77, 79, 0.15)',
              border: 'none',
              background: 'linear-gradient(135deg, #ffeaa7 0%, #fdcb6e 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>支出总计</Text>}
              value={Math.abs(outcome)}
              precision={2}
              valueStyle={{ color: '#c53030', fontSize: '28px', fontWeight: 700 }}
              prefix={<ArrowDownOutlined />}
              suffix="元"
            />
            <Text style={{ fontSize: '13px', color: '#2d3748', marginTop: '8px', display: 'block' }}>
              共 {outcomeCount} 笔
            </Text>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              border: 'none',
              background: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>总条目数</Text>}
              value={entries.length}
              valueStyle={{ color: '#2d3748', fontSize: '28px', fontWeight: 700 }}
              prefix={<FileTextOutlined />}
              suffix="笔"
            />
            <Text style={{ fontSize: '13px', color: '#2d3748', marginTop: '8px', display: 'block' }}>
              {income > 0 && Math.abs(outcome) > 0
                ? `收入占比 ${((income / (income + Math.abs(outcome))) * 100).toFixed(1)}%`
                : '暂无数据'}
            </Text>
          </Card>
        </Col>
      </Row>

      <Row gutter={[24, 24]}>
        <Col xs={24} lg={14}>
          <Card
            title={<Title level={4} style={{ margin: 0, color: '#2d3748' }}>按分类统计 Top 10</Title>}
            bordered={false}
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              overflow: 'hidden'
            }}
          >
            <Table
              size="middle"
              pagination={false}
              dataSource={byCategory(entries).map(([category, amount], idx) => ({
                key: category,
                rank: idx + 1,
                category,
                amount
              }))}
              style={{ borderRadius: '8px' }}
              columns={[
                {
                  title: '排名',
                  dataIndex: 'rank',
                  width: 80,
                  render: (rank: number) => (
                    <Badge
                      count={rank}
                      style={{
                        backgroundColor: rank <= 3 ? '#f59e0b' : '#94a3b8',
                        fontSize: '14px',
                        fontWeight: 'bold'
                      }}
                    />
                  )
                },
                {
                  title: '分类',
                  dataIndex: 'category',
                  render: (cat: string) => (
                    <Tag color="blue" style={{ fontSize: '14px', padding: '4px 12px', borderRadius: '6px' }}>
                      {cat}
                    </Tag>
                  )
                },
                {
                  title: '金额合计',
                  dataIndex: 'amount',
                  sorter: (a, b) => a.amount - b.amount,
                  render: (amount: number) => (
                    <Text strong style={{
                      color: amount >= 0 ? '#22543d' : '#c53030',
                      fontSize: '15px'
                    }}>
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
                      <div style={{ width: 120 }}>
                        <Progress
                          percent={Number(percent.toFixed(1))}
                          size="small"
                          status={amount >= 0 ? 'success' : 'exception'}
                          strokeColor={amount >= 0 ? '#52c41a' : '#ff4d4f'}
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
          <Card
            title={<Title level={4} style={{ margin: 0, color: '#2d3748' }}>最常使用的类型 Top 5</Title>}
            bordered={false}
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              overflow: 'hidden'
            }}
          >
            <Table
              size="middle"
              pagination={false}
              dataSource={byType(entries).map(([type, count]) => ({ key: type, type, count }))}
              columns={[
                {
                  title: '类型',
                  dataIndex: 'type',
                  render: (type: string) => (
                    <Tag color="green" style={{ fontSize: '14px', padding: '4px 12px', borderRadius: '6px' }}>
                      {type}
                    </Tag>
                  )
                },
                {
                  title: '使用次数',
                  dataIndex: 'count',
                  render: (count: number) => (
                    <Text strong style={{ fontSize: '15px' }}>{count} 次</Text>
                  )
                },
              ]}
            />
          </Card>

          <Card
            title={<Title level={4} style={{ margin: 0, color: '#2d3748' }}>快速统计</Title>}
            bordered={false}
            style={{
              marginTop: 24,
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              background: 'linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%)'
            }}
          >
            <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <div style={{
                padding: '12px',
                background: 'rgba(255, 255, 255, 0.7)',
                borderRadius: '8px'
              }}>
                <Text style={{ color: '#4a5568', fontSize: '14px' }}>平均每日收入：</Text>
                <Text strong style={{ color: '#22543d', marginLeft: 8, fontSize: '16px' }}>
                  {(income / 30).toFixed(2)} 元
                </Text>
              </div>
              <div style={{
                padding: '12px',
                background: 'rgba(255, 255, 255, 0.7)',
                borderRadius: '8px'
              }}>
                <Text style={{ color: '#4a5568', fontSize: '14px' }}>平均每日支出：</Text>
                <Text strong style={{ color: '#c53030', marginLeft: 8, fontSize: '16px' }}>
                  {(Math.abs(outcome) / 30).toFixed(2)} 元
                </Text>
              </div>
              <div style={{
                padding: '12px',
                background: 'rgba(255, 255, 255, 0.7)',
                borderRadius: '8px'
              }}>
                <Text style={{ color: '#4a5568', fontSize: '14px' }}>平均每笔金额：</Text>
                <Text strong style={{ color: '#2d3748', marginLeft: 8, fontSize: '16px' }}>
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