import { Link, useNavigate } from "react-router-dom";
import './index.scss';
import { BarsOutlined, BgColorsOutlined, EditOutlined, LineChartOutlined, SettingOutlined } from '@ant-design/icons';
interface HeaderItemProps {
    title?: string;
    to?: string;
    icon?: any;
}
function HeaderItem(props: HeaderItemProps) {
    const navigate = useNavigate();
    const { title = 'title', to = './', icon = 'icon', ...other } = props;
    return (
        <div {...other}>
            {icon}
        </div>
    )
}
export default function CommonHeader() {
    return (
        <header className='CommonHeader'>
            <HeaderItem title='账单' to='/' icon={<BarsOutlined />} />
            <HeaderItem title='新增' to='/' icon={<EditOutlined />} />
            <HeaderItem title='报告' to='/' icon={<LineChartOutlined />} />
            <HeaderItem title='设置' to='/' icon={<SettingOutlined />} />
        </header>
    );
}