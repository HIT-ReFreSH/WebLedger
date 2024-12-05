import axios from "axios";
const headers = {
  "Content-Type": "application/json",
  "Access-Control-Allow-Origin": "*",
  "wl-access": "admin",
  "wl-secret": "admin123",
};
export const login = (Name: string, Key: string) => {
  return axios.post("/api/config/login", { Name, Key }, { headers: headers });
};
export const register = (Name: string) => {
  return axios.post("/api/config/register", { Name }, { headers: headers });
};
