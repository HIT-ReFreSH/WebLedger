import { Card, Table, Typography, Tag, Spin, Alert, Statistic, Row, Col } from 'antd'
import { TagsOutlined } from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { SelectOption } from '../lib/types'
import { useMemo } from 'react'

const { Title, Text } = Typography

export default function Types() {
  const option: SelectOption = useMemo(() => {
    const now = new Date()
    const start = new Date(now)
    start.setFullYear(start.getFullYear() - 5)
    return {
      startTime: start.toISOString(),
      endTime: now.toISOString(),
      direction: null,
      category: null,
    }
  }, [])

  const { data, isLoading, isError, error } = useQuery({
    queryKey: ['types', option],
    queryFn: () => api.select(option),
  })

  const rows = Object.values(
    (data ?? []).reduce(
      (acc, e) => {
        const key = e.type
        if (!acc[key]) {
          acc[key] = {
            key,
            type: key,
            category: e.category || '(未分类)',
            count: 0,
            totalAmount: 0,
            incomeCount: 0,
            incomeAmount: 0,
            expenseCount: 0,
            expenseAmount: 0,
          }
        }
        acc[key].count += 1
        acc[key].totalAmount += Math.abs(e.amount)

        if (e.amount > 0) {
          acc[key].incomeCount += 1
          acc[key].incomeAmount += e.amount
        } else {
          acc[key].expenseCount += 1
          acc[key].expenseAmount += Math.abs(e.amount)
        }
        return acc
      },
      {} as Record<
        string,
        {
          key: string
          type: string
          category: string
          count: number
          totalAmount: number
          incomeCount: number
          incomeAmount: number
          expenseCount: number
          expenseAmount: number
        }
      >
    )
  )

  if (isLoading) {
    return (
      <div style={{ textAlign: 'center', padding: '50px' }}>
        <Spin size="large" tip="加载中..." />
      </div>
    )
  }

  if (isError) {
    return <Alert message="错误" description={String(error)} type="error" showIcon />
  }

  const totalTypes = rows.length
  const totalUsage = rows.reduce((sum, r) => sum + r.count, 0)
  const mostUsedType = rows.sort((a, b) => b.count - a.count)[0]

  return (
    <div style={{ width: '100%', maxWidth: '1600px', margin: '0 auto' }}>
      <div style={{
        marginBottom: 32,
        padding: '24px',
        background: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
        borderRadius: '16px',
        boxShadow: '0 8px 24px rgba(48, 207, 208, 0.25)'
      }}>
        <Title level={2} style={{ marginBottom: 8, color: '#fff' }}>
          <TagsOutlined /> 类型管理
        </Title>
        <Text style={{ fontSize: '15px', color: 'rgba(255, 255, 255, 0.9)' }}>
          查看所有财务条目类型的统计信息
        </Text>
      </div>

      <Row gutter={[24, 24]} style={{ marginBottom: 32 }}>
        <Col xs={24} sm={8}>
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
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>类型总数</Text>}
              value={totalTypes}
              suffix="个"
              valueStyle={{ color: '#2d3748', fontSize: '32px', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              border: 'none',
              background: 'linear-gradient(135deg, #d4fc79 0%, #96e6a1 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>总使用次数</Text>}
              value={totalUsage}
              suffix="次"
              valueStyle={{ color: '#22543d', fontSize: '32px', fontWeight: 700 }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card
            hoverable
            style={{
              borderRadius: '12px',
              boxShadow: '0 4px 12px rgba(0, 0, 0, 0.08)',
              border: 'none',
              background: 'linear-gradient(135deg, #ffeaa7 0%, #fdcb6e 100%)'
            }}
          >
            <Statistic
              title={<Text strong style={{ fontSize: '15px', color: '#2d3748' }}>最常使用</Text>}
              value={mostUsedType?.type || '-'}
              valueStyle={{ fontSize: '20px', color: '#2d3748', fontWeight: 600 }}
            />
            {mostUsedType && (
              <Text style={{ fontSize: '13px', color: '#2d3748', display: 'block', marginTop: 8 }}>
                使用 {mostUsedType.count} 次
              </Text>
            )}
          </Card>
        </Col>
      </Row>

      <Card
        title={<Text strong style={{ fontSize: '16px', color: '#2d3748' }}>类型统计列表</Text>}
        extra={
          <Text type="secondary" style={{ fontSize: '14px' }}>
            类型由条目自动创建，无需手动管理
          </Text>
        }
        style={{
          borderRadius: '16px',
          boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
          border: 'none',
          marginBottom: 24
        }}
      >
        <Table
          dataSource={rows}
          pagination={{
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 个类型`,
          }}
          style={{ borderRadius: '12px' }}
          columns={[
            {
              title: <Text strong>类型名称</Text>,
              dataIndex: 'type',
              sorter: (a, b) => a.type.localeCompare(b.type),
              render: (type: string) => (
                <Tag
                  color="geekblue"
                  style={{
                    fontSize: '14px',
                    padding: '4px 12px',
                    borderRadius: '6px',
                    fontWeight: 500
                  }}
                >
                  {type}
                </Tag>
              ),
            },
            {
              title: <Text strong>默认分类</Text>,
              dataIndex: 'category',
              render: (cat: string) => (
                <Tag
                  color={cat === '(未分类)' ? 'default' : 'purple'}
                  style={{
                    fontSize: '13px',
                    padding: '4px 12px',
                    borderRadius: '6px'
                  }}
                >
                  {cat}
                </Tag>
              ),
            },
            {
              title: <Text strong>总使用次数</Text>,
              dataIndex: 'count',
              sorter: (a, b) => a.count - b.count,
              render: (count: number) => (
                <Text strong style={{ fontSize: '15px' }}>{count} 次</Text>
              ),
            },
            {
              title: <Text strong>收入次数</Text>,
              dataIndex: 'incomeCount',
              sorter: (a, b) => a.incomeCount - b.incomeCount,
              render: (count: number) => (
                <Text style={{ color: '#22543d', fontSize: '14px', fontWeight: 500 }}>
                  {count} 次
                </Text>
              ),
            },
            {
              title: <Text strong>收入金额</Text>,
              dataIndex: 'incomeAmount',
              sorter: (a, b) => a.incomeAmount - b.incomeAmount,
              render: (amount: number) => (
                <Text style={{ color: '#22543d', fontSize: '14px', fontWeight: 500 }}>
                  +{amount.toFixed(2)} 元
                </Text>
              ),
            },
            {
              title: <Text strong>支出次数</Text>,
              dataIndex: 'expenseCount',
              sorter: (a, b) => a.expenseCount - b.expenseCount,
              render: (count: number) => (
                <Text style={{ color: '#c53030', fontSize: '14px', fontWeight: 500 }}>
                  {count} 次
                </Text>
              ),
            },
            {
              title: <Text strong>支出金额</Text>,
              dataIndex: 'expenseAmount',
              sorter: (a, b) => a.expenseAmount - b.expenseAmount,
              render: (amount: number) => (
                <Text style={{ color: '#c53030', fontSize: '14px', fontWeight: 500 }}>
                  -{amount.toFixed(2)} 元
                </Text>
              ),
            },
            {
              title: <Text strong>总金额</Text>,
              dataIndex: 'totalAmount',
              sorter: (a, b) => a.totalAmount - b.totalAmount,
              render: (amount: number) => (
                <Text strong style={{ fontSize: '15px' }}>{amount.toFixed(2)} 元</Text>
              ),
            },
          ]}
        />
      </Card>

      <Card
        title={<Text strong style={{ fontSize: '16px', color: '#2d3748' }}>说明</Text>}
        bordered={false}
        style={{
          borderRadius: '16px',
          boxShadow: '0 4px 20px rgba(0, 0, 0, 0.08)',
          background: 'linear-gradient(135deg, #e0c3fc 0%, #8ec5fc 100%)'
        }}
      >
        <div style={{
          fontSize: '14px',
          color: '#2d3748',
          padding: '16px',
          background: 'rgba(255, 255, 255, 0.8)',
          borderRadius: '8px'
        }}>
          <ul style={{ margin: 0, paddingLeft: 20, lineHeight: '1.8' }}>
            <li>
              <strong>类型</strong>：由创建条目时自动生成，无需手动管理
            </li>
            <li>
              <strong>默认分类</strong>：创建条目时为类型指定的分类
            </li>
            <li>
              <strong>统计范围</strong>：过去 5 年的所有数据
            </li>
            <li>如需修改类型的默认分类，可在创建条目时重新指定</li>
          </ul>
        </div>
      </Card>
    </div>
  )
}