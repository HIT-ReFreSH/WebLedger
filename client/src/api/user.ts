import axios from "axios";

const headers = {
  "Content-Type": "application/json",
  "Access-Control-Allow-Origin": "*",
  "wl-access": "admin",
  "wl-secret": "admin123",
};

// 登录方法，保持不变
export const login = (Name: string, Key: string) => {
  return axios.post("/api/config/login", { Name, Key }, { headers: headers });
};

// 注册方法，保持不变
export const register = (Name: string) => {
  return axios.post("/api/config/register", { Name }, { headers: headers });
};

// 以下方法对应后端的添加访问权限接口，请求路径为 "/api/config/grant"，接收要添加的权限名称作为参数
// 发送请求后根据后端返回的结果（如生成的权限相关信息等）进行后续处理
export const addAccess = (name: string) => {
  return axios.get(`/api/config/grant?name=${name}`, { headers: headers });
};

// 对应后端的移除访问权限接口，请求路径为 "/api/config/cancel"，传入要移除的权限名称
// 发送请求通知后端移除相应权限，通常后端会根据操作结果返回相应状态码等信息，前端可按需处理
export const removeAccess = (name: string) => {
  return axios.get(`/api/config/cancel?name=${name}`, { headers: headers });
};

// 对应后端获取所有访问权限的接口，请求路径为 "/api/config/access"
// 获取所有访问权限相关信息，例如权限列表等内容，以便在前端进行展示或其他相关操作
export const getAllAccess = () => {
  return axios.get("/api/config/access", { headers: headers });
};