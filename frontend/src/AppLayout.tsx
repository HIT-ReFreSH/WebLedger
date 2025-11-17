import { Layout, Menu, Input, Space, Typography, theme, Breadcrumb, Tooltip } from 'antd'
import {
  DashboardOutlined,
  UnorderedListOutlined,
  PlusCircleOutlined,
  FolderOutlined,
  TagsOutlined,
  KeyOutlined,
  LockOutlined,
} from '@ant-design/icons'
import { Link, Outlet, useRouterState } from '@tanstack/react-router'
import { useEffect, useState } from 'react'

const { Header, Sider, Content, Footer } = Layout
const { Text, Title } = Typography

export function AppLayout() {
  const routerState = useRouterState()
  const [collapsed, setCollapsed] = useState(false)
  const [accessName, setAccessName] = useState<string>(localStorage.getItem('wl-access') || '')
  const [secret, setSecret] = useState<string>(localStorage.getItem('wl-secret') || '')
  const { token } = theme.useToken()

  useEffect(() => {
    localStorage.setItem('wl-access', accessName)
    localStorage.setItem('wl-secret', secret)
  }, [accessName, secret])

  const selectedKey = (() => {
    const path = routerState.location.pathname
    if (path === '/') return 'dashboard'
    if (path.startsWith('/entries')) return 'entries'
    if (path.startsWith('/entry')) return 'new-entry'
    if (path.startsWith('/categories')) return 'categories'
    if (path.startsWith('/types')) return 'types'
    return 'dashboard'
  })()

  const getBreadcrumb = () => {
    const path = routerState.location.pathname
    if (path === '/') return [{ title: '仪表盘' }]
    if (path.startsWith('/entries')) return [{ title: '条目列表' }]
    if (path.startsWith('/entry/new')) return [{ title: '新建条目' }]
    if (path.startsWith('/categories')) return [{ title: '分类管理' }]
    if (path.startsWith('/types')) return [{ title: '类型管理' }]
    return []
  }

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={setCollapsed}
        breakpoint="lg"
        style={{
          overflow: 'auto',
          height: '100vh',
          position: 'fixed',
          left: 0,
          top: 0,
          bottom: 0,
        }}
      >
        <div
          style={{
            color: token.colorTextBase,
            padding: 16,
            fontWeight: 700,
            fontSize: collapsed ? '14px' : '18px',
            textAlign: 'center',
            background: 'rgba(255, 255, 255, 0.1)',
            transition: 'all 0.2s',
          }}
        >
          {collapsed ? 'WL' : 'WebLedger'}
        </div>
        <Menu
          selectedKeys={[selectedKey]}
          theme="dark"
          mode="inline"
          items={[
            {
              key: 'dashboard',
              icon: <DashboardOutlined />,
              label: <Link to="/">仪表盘</Link>,
            },
            {
              key: 'entries',
              icon: <UnorderedListOutlined />,
              label: <Link to="/entries">条目列表</Link>,
            },
            {
              key: 'new-entry',
              icon: <PlusCircleOutlined />,
              label: <Link to="/entry/new">新建条目</Link>,
            },
            {
              key: 'categories',
              icon: <FolderOutlined />,
              label: <Link to="/categories">分类管理</Link>,
            },
            {
              key: 'types',
              icon: <TagsOutlined />,
              label: <Link to="/types">类型管理</Link>,
            },
          ]}
        />
      </Sider>
      <Layout style={{ marginLeft: collapsed ? 80 : 200, transition: 'all 0.2s' }}>
        <Header
          style={{
            background: token.colorBgContainer,
            padding: '0 24px',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            borderBottom: `1px solid ${token.colorBorderSecondary}`,
            position: 'sticky',
            top: 0,
            zIndex: 1,
          }}
        >
          <Breadcrumb items={getBreadcrumb()} />
          <Space size="middle" wrap>
            <Text type="secondary" style={{ fontSize: '13px' }}>
              <LockOutlined /> 访问控制
            </Text>
            <Tooltip title="访问名称 (wl-access)">
              <Input
                placeholder="wl-access"
                value={accessName}
                onChange={(e) => setAccessName(e.target.value)}
                style={{ width: 140 }}
                prefix={<KeyOutlined />}
                size="small"
              />
            </Tooltip>
            <Tooltip title="访问密钥 (wl-secret)">
              <Input.Password
                placeholder="wl-secret"
                value={secret}
                onChange={(e) => setSecret(e.target.value)}
                style={{ width: 160 }}
                size="small"
              />
            </Tooltip>
          </Space>
        </Header>
        <Content style={{ padding: 24, minHeight: 'calc(100vh - 64px - 70px)' }}>
          <Outlet />
        </Content>
        <Footer style={{ textAlign: 'center', background: token.colorBgContainer }}>
          <Text type="secondary">
            WebLedger ©{new Date().getFullYear()} - 现代化财务管理系统
          </Text>
        </Footer>
      </Layout>
    </Layout>
  )
}

export default AppLayout