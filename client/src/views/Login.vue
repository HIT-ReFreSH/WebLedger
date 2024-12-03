<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '@/api/user';
import { useUserStore } from '@/stores/user';

// 定义响应式数据
const loginForm = reactive({
  Name: '',
  Key: '',
  rememberMe: false,
});

interface MessageApi {
  error: (msg: string) => void
  success: (msg: string) => void;
}

const $message: MessageApi = {
  error: (msg) => {
    ElMessage.error(msg)
  },
  success: (msg) => {
    ElMessage.success(msg)
  },
};

// 使用 Vue Router
const router = useRouter();

// 方法定义
const handleLogin = async () => {
  if (!loginForm.Name || !loginForm.Key) {
    $message.error("请填写完整的用户名和密码");
    return;
  }

  if (loginForm.rememberMe) {
    localStorage.setItem(
      'loginInfo',
      JSON.stringify({
        username: loginForm.Name,
        password: loginForm.Key,
      })
    );
  } else {
    localStorage.removeItem('loginInfo');
  }
  const res = await login(loginForm.Name, loginForm.Key);
  if(res?.status === 200){
    if (res.data?.code === 200) {
      $message.success("登录成功");
      const userStore = useUserStore();
      userStore.access = loginForm.Name;
      userStore.secret = loginForm.Key;
      router.push('/home');
    }
    else {
      $message.error(res.data?.message);
    }
  }else{
    $message.error("登录失败，错误码：" + res?.status);
  }
};

const navigateToRegister = () => {
  router.push('/register');
};

const loadSavedCredentials = () => {
  const savedInfoString = localStorage.getItem('loginInfo');
  if (savedInfoString) {
    const savedInfo: { username: string; password: string } = JSON.parse(savedInfoString);
    loginForm.Name = savedInfo.username;
    loginForm.Key = savedInfo.password;
    loginForm.rememberMe = true;
  }
};

// 生命周期钩子
onMounted(() => {
  if(useUserStore().access!==""&&useUserStore().secret!==""){
    router.push('/home');
  }else{
    loadSavedCredentials();
  }
});
</script>
<template>
    <div class="login-container">
      <el-card class="login-card" shadow="hover">
        <h2 class="login-title">欢迎使用Report Query</h2>
        <el-form :model="loginForm" class="login-form" label-width="auto">
          <el-form-item label="用户名">
            <el-input v-model="loginForm.Name" placeholder="请输入用户名"></el-input>
          </el-form-item>
          <el-form-item label="密码">
            <el-input v-model="loginForm.Key" type="password" placeholder="请输入密码" show-password></el-input>
          </el-form-item>
          <el-form-item>
            <el-checkbox v-model="loginForm.rememberMe">记住用户名和密码</el-checkbox>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleLogin">登录</el-button>
            <el-button type="text" @click="navigateToRegister">注册</el-button>
          </el-form-item>
        </el-form>
      </el-card>
    </div>
  </template>
<style scoped>
  .login-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    background-color: #f5f5f5;
  }
  .login-card {
    width: 500px;
    padding: 20px;
  }
  .login-title {
    text-align: center;
    margin-bottom: 20px;
  }
  </style>
  