import axios from "axios";
import { useUserStore } from "@/stores/user";

const getHeaders = () => {
    const userStore = useUserStore();
    const headers = {
        "Access-Control-Allow-Origin": "*",
        "wl-access": userStore.access,
        "wl-secret": userStore.secret,
    };
    return headers;
}
export interface Entry{
    isIncome: boolean;
    amount: number;
    givenTime: string;
    type: string;
    category: string;
    description: string;
}
export interface SelectOption{
    dateRange:[string,string],
    startTime: string,
    endTime: string,
    direction: boolean,//收入为true，支出为false
    category: string
}

export const getCategories = () => {
  const headers = getHeaders();
  return axios.get("/api/ledger/category", { headers: headers });
};

export const addEntry = (record: Entry) => {
    const headers = getHeaders();
    return axios.post("/api/ledger/entry", record, { headers: headers });
}

export const selectEntries = (option: SelectOption) => {
    const headers = getHeaders();
    return axios.post("/api/ledger/select", option, { headers: headers });
}
