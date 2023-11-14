import { Routes, Route, Navigate } from "react-router";
import LedgerCreate from "../pages/LedgerCreate";
import LedgerOverview from "../pages/LedgerOverview";
import ReportOverview from "../pages/ReportOverview";
import SettingPage from "../pages/SettingPage";
import LedgerDetail from "../pages/LedgerDetail";
import ReportDetail from "../pages/ReportDetail";

export default function RootRoute() {
    return (
        <Routes>
            <Route path="/ledger">
                <Route path="create" element={<LedgerCreate/>} />
                <Route path="overview" element={<LedgerOverview/>} />
                <Route path="detail/:ledger" element={<LedgerDetail/>} />
            </Route>
            <Route path="/report">
                <Route path="overview" element={<ReportOverview/>}/>
                <Route path="detail/:reportId" element={<ReportDetail/>}/>
            </Route>
            <Route path="/setting" element={<SettingPage/>} />
            <Route path="/" element={<Navigate to="/ledgder/overview" />} />
            <Route path="*" element={<h1>Not Found</h1>} />
        </Routes>
    );
}