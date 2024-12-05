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
export interface Category {
    name: string;
    superCategory: string;
}
export const getCategories = () => {
    return axios.get<Category[]>("/api/ledger/category", { headers: getHeaders() });
}
export const updateCategory = (category: Category) => {
    if(category.superCategory===""){
        return axios.put("/api/ledger/category", { name:category.name }, { headers: getHeaders() });
    }
    return axios.put("/api/ledger/category", category, { headers: getHeaders() });
}
export const deleteCategory = (category: string) => {
    return axios.delete(`/api/ledger/category?category=${category}`, { headers: getHeaders() });
}



