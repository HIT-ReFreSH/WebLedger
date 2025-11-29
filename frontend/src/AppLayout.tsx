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
    <Layout style={{ minHeight: '100vh', background: '#f5f7fa' }}>
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
          boxShadow: '2px 0 8px rgba(0, 0, 0, 0.1)',
        }}
        theme="dark"
      >
        <div
          style={{
            color: '#fff',
            padding: collapsed ? '16px 8px' : '20px 16px',
            fontWeight: 700,
            fontSize: collapsed ? '16px' : '20px',
            textAlign: 'center',
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            transition: 'all 0.3s',
            letterSpacing: collapsed ? '0px' : '1px',
          }}
        >
          {collapsed ? 'WL' : 'WebLedger'}
        </div>
        <Menu
          selectedKeys={[selectedKey]}
          theme="dark"
          mode="inline"
          style={{ marginTop: '8px' }}
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
      <Layout style={{ marginLeft: collapsed ? 80 : 200, transition: 'all 0.3s', background: '#f5f7fa' }}>
        <Header
          style={{
            background: '#fff',
            padding: '12px 32px',
            borderBottom: `1px solid ${token.colorBorderSecondary}`,
            position: 'sticky',
            top: 0,
            zIndex: 10,
            boxShadow: '0 2px 8px rgba(0, 0, 0, 0.06)',
            height: 'auto',
            minHeight: 'auto',
          }}
        >
          {/* 宽屏布局：所有元素在一行 */}
          <div className="header-wide" style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            flexWrap: 'nowrap',
            gap: '16px',
          }}>
            <Breadcrumb items={getBreadcrumb()} style={{ flex: '0 1 auto', minWidth: 0 }} />
            <Space size="middle" style={{ flex: '0 0 auto', whiteSpace: 'nowrap' }}>
              <Text type="secondary" style={{ fontSize: '14px', fontWeight: 500 }} className="access-label">
                <LockOutlined /> 访问控制
              </Text>
              <Tooltip title="访问名称 (wl-access)">
                <Input
                  placeholder="wl-access"
                  value={accessName}
                  onChange={(e) => setAccessName(e.target.value)}
                  style={{ width: 150, borderRadius: '6px' }}
                  prefix={<KeyOutlined />}
                  size="middle"
                />
              </Tooltip>
              <Tooltip title="访问密钥 (wl-secret)">
                <Input.Password
                  placeholder="wl-secret"
                  value={secret}
                  onChange={(e) => setSecret(e.target.value)}
                  style={{ width: 170, borderRadius: '6px' }}
                  size="middle"
                />
              </Tooltip>
            </Space>
          </div>

          {/* 窄屏布局：三行排列 */}
          <div className="header-narrow" style={{ display: 'none' }}>
            <div style={{ marginBottom: '12px' }}>
              <Breadcrumb items={getBreadcrumb()} />
            </div>
            <div style={{ display: 'flex', flexDirection: 'column', gap: '8px' }}>
              <Tooltip title="访问名称 (wl-access)">
                <Input
                  placeholder="wl-access"
                  value={accessName}
                  onChange={(e) => setAccessName(e.target.value)}
                  style={{ width: '100%', maxWidth: '320px', borderRadius: '6px' }}
                  prefix={<KeyOutlined />}
                  size="middle"
                />
              </Tooltip>
              <Tooltip title="访问密钥 (wl-secret)">
                <Input.Password
                  placeholder="wl-secret"
                  value={secret}
                  onChange={(e) => setSecret(e.target.value)}
                  style={{ width: '100%', maxWidth: '320px', borderRadius: '6px' }}
                  size="middle"
                />
              </Tooltip>
            </div>
          </div>
        </Header>
        <Content
          style={{
            padding: '32px',
            minHeight: 'calc(100vh - 150px)',
            background: '#f5f7fa',
          }}
        >
          <Outlet />
        </Content>
        <Footer
          style={{
            textAlign: 'center',
            background: '#fff',
            borderTop: `1px solid ${token.colorBorderSecondary}`,
            padding: '20px',
          }}
        >
          <Text type="secondary" style={{ fontSize: '14px' }}>
            WebLedger ©{new Date().getFullYear()} - 现代化财务管理系统
          </Text>
        </Footer>
      </Layout>
    </Layout>
  )
}

export default AppLayout