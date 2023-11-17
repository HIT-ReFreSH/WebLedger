import { Link, useNavigate } from "react-router-dom";
import './index.scss';
import { BarsOutlined, BgColorsOutlined, EditOutlined, LineChartOutlined, SettingOutlined } from '@ant-design/icons';
interface HeaderItemProps {
    title?: string;
    to?: string;
    icon?: any;
    style?: object;
}
function HeaderItem(props: HeaderItemProps) {
    const navigate = useNavigate();
    const { title = 'title', to = './', icon = 'icon', style, ...other } = props;
    return (
        <div
            className="HeaderItem"
            onClick={() => navigate(to)}
            {...other}>
            {icon}
        </div>
    )
}
export default function CommonHeader() {
    return (
        <header className='CommonHeader'>
            <HeaderItem title='账单' to='/ledger/overview' icon={<BarsOutlined />} />
            <HeaderItem title='新增' to='/ledger/create' icon={<EditOutlined />} />
            <HeaderItem title='报告' to='/report/overview' icon={<LineChartOutlined />} />
            <HeaderItem title='设置' to='/setting' icon={<SettingOutlined />} />
        </header>
    );
}