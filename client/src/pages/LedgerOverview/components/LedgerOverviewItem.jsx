import { useNavigate } from 'react-router-dom';
export default function LedgerOverviewItem({ props }) {
    const navigate = useNavigate();

    const handleClick = () => {
        // 进行编程式路由导航
        navigate('/legder/detail/1');
    };

    return (
        <div className="OverviewItem" onClick={handleClick}>
            <div className="num">50</div>
            <div className="infoContainer">
                <div className="title">宿舍电费</div>
                <div className="type">必须开销</div>
                <div className="time">2024:5:20 10:20</div>
            </div>
        </div>
    );
}