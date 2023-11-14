import LedgerOverviewItem from "./components/LedgerOverviewItem";
import './index.scss';
export default function LedgerOverview() {
    return (
        <div className="LedgerOverview">
            <h1>LedgerOverview</h1>
            <div className="OverviewList">
                {
                    [1,1,1,1].map((item, index) => (
                        <LedgerOverviewItem key={index} />
                    ))
                }
            </div>
        </div>
    );
}