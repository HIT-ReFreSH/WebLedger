import { Card, Table, Typography, Tag, Spin, Alert, Statistic, Row, Col } from 'antd'
import { TagsOutlined } from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { api } from '../lib/api'
import type { SelectOption } from '../lib/types'

const { Title, Text } = Typography

export default function Types() {
  const now = new Date()
  const start = new Date(now)
  start.setFullYear(start.getFullYear() - 5)
  const option: SelectOption = {
    startTime: start.toISOString(),
    endTime: now.toISOString(),
    direction: null,
    category: null,
  }

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
    <div>
      <Title level={3}>
        <TagsOutlined /> 类型管理
      </Title>
      <Text type="secondary">查看所有财务条目类型的统计信息</Text>

      <Row gutter={[16, 16]} style={{ marginTop: 16 }}>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic title="类型总数" value={totalTypes} suffix="个" />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic title="总使用次数" value={totalUsage} suffix="次" />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="最常使用"
              value={mostUsedType?.type || '-'}
              valueStyle={{ fontSize: '16px' }}
            />
            {mostUsedType && (
              <Text type="secondary" style={{ fontSize: '12px' }}>
                使用 {mostUsedType.count} 次
              </Text>
            )}
          </Card>
        </Col>
      </Row>

      <Card
        title="类型统计列表"
        extra={
          <Text type="secondary" style={{ fontSize: '13px' }}>
            类型由条目自动创建，无需手动管理
          </Text>
        }
        style={{ marginTop: 16 }}
      >
        <Table
          dataSource={rows}
          pagination={{
            pageSize: 10,
            showSizeChanger: true,
            showTotal: (total) => `共 ${total} 个类型`,
          }}
          columns={[
            {
              title: '类型名称',
              dataIndex: 'type',
              sorter: (a, b) => a.type.localeCompare(b.type),
              render: (type: string) => (
                <Tag color="geekblue" style={{ fontSize: '14px' }}>
                  {type}
                </Tag>
              ),
            },
            {
              title: '默认分类',
              dataIndex: 'category',
              render: (cat: string) => (
                <Tag color={cat === '(未分类)' ? 'default' : 'purple'}>{cat}</Tag>
              ),
            },
            {
              title: '总使用次数',
              dataIndex: 'count',
              sorter: (a, b) => a.count - b.count,
              render: (count: number) => <Text strong>{count} 次</Text>,
            },
            {
              title: '收入次数',
              dataIndex: 'incomeCount',
              sorter: (a, b) => a.incomeCount - b.incomeCount,
              render: (count: number) => (
                <Text style={{ color: '#3f8600' }}>{count} 次</Text>
              ),
            },
            {
              title: '收入金额',
              dataIndex: 'incomeAmount',
              sorter: (a, b) => a.incomeAmount - b.incomeAmount,
              render: (amount: number) => (
                <Text style={{ color: '#3f8600' }}>+{amount.toFixed(2)} 元</Text>
              ),
            },
            {
              title: '支出次数',
              dataIndex: 'expenseCount',
              sorter: (a, b) => a.expenseCount - b.expenseCount,
              render: (count: number) => (
                <Text style={{ color: '#cf1322' }}>{count} 次</Text>
              ),
            },
            {
              title: '支出金额',
              dataIndex: 'expenseAmount',
              sorter: (a, b) => a.expenseAmount - b.expenseAmount,
              render: (amount: number) => (
                <Text style={{ color: '#cf1322' }}>-{amount.toFixed(2)} 元</Text>
              ),
            },
            {
              title: '总金额',
              dataIndex: 'totalAmount',
              sorter: (a, b) => a.totalAmount - b.totalAmount,
              render: (amount: number) => <Text strong>{amount.toFixed(2)} 元</Text>,
            },
          ]}
        />
      </Card>

      <Card
        style={{ marginTop: 16 }}
        title="说明"
        bordered={false}
        type="inner"
        size="small"
      >
        <div style={{ fontSize: '13px', color: '#666' }}>
          <ul style={{ margin: 0, paddingLeft: 20 }}>
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