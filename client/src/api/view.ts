import axios from "axios";
import { useUserStore } from "@/stores/user";
import type { Entry } from "@/api/record";
const getHeaders = () => {
    const userStore = useUserStore();
    const headers = {
        "Access-Control-Allow-Origin": "*",
        "wl-access": userStore.access,
        "wl-secret": userStore.secret,
    };
    return headers;
}
export interface Template {
    name: string;
    categories: string[];
    isIncome: boolean;
}
export interface View {
    name: string;
    startTime: string;
    endTime: string;
    templateName: string;
}
export interface ViewAutomation {
    type:number;
    templateName:string;
}
export interface ViewDetail{
    raw:[];
    byCategory:Record<string,number>;
    byTime:Record<string,number>;
    category:{
        name:string;
        amount:number;
    }[];
    time:{
        time:string;
        amount:number;
    }[];
    total:number;
}
export enum LedgerViewAutomationType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Quarterly = 3,
    Yearly = 4
}
export const getTemplates = () => {
    return axios.get<string[]>("/api/ledger/view-templates", { headers: getHeaders() });
}
export const getTemplate = (name: string) => {
    return axios.get<Template>(`/api/ledger/view-template?name=${name}`, { headers: getHeaders() });
}
export const updateTemplate = (template: Template) => {
    return axios.put("/api/ledger/view-template", template, { headers: getHeaders() });
}
export const deleteTemplate = (template: string) => {
    return axios.delete(`/api/ledger/view-templates?template=${template}`, { headers: getHeaders() });
}
export const getViewAutomation = () => {
    return axios.get<ViewAutomation[]>("/api/ledger/view-automation", { headers: getHeaders() });
}
export const addViewAutomation = (automation: ViewAutomation) => {
    return axios.post("/api/ledger/view-automation/add", automation, { headers: getHeaders() });
}
export const deleteViewAutomation = (automation: ViewAutomation) => {
    return axios.post("/api/ledger/view-automation/remove", automation, { headers: getHeaders() });
}

export const addView = (view: View) => {
    return axios.post("/api/ledger/view", view, { headers: getHeaders() });
}
export const getViews = () => {
    return axios.get<string[]>("/api/ledger/view", { headers: getHeaders() });
}
export const deleteView = (view: string) => {
    return axios.delete(`/api/ledger/view?view=${view}`, { headers: getHeaders() });
}
export const getViewDetail = (viewName: string) => {
    return axios.post(`/api/ledger/query`, {viewName,limit:-10}, { headers: getHeaders() });
}
export const downloadView = (viewName: string) => {
    return axios.post(`/api/ledger/query-graphical`, {viewName:"上周报表",limit:-10}, { headers: getHeaders() });
}

