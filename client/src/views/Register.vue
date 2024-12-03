<script setup lang="ts">
import { reactive, ref } from "vue";
import { useRouter } from "vue-router";
import { register } from "@/api/user";

const registerForm = reactive({
  username: "",
});
const dialogVisible = ref(false);
const router = useRouter();
const key = ref("");

const handleRegister = async () => {
  if (!registerForm.username) {
    ElMessage.error("请填写完整的信息");
    return;
  }
  const res = await register(registerForm.username);
  if(res?.status === 200){
    if(res.data?.code === 200){
      key.value = res.data.key;
      dialogVisible.value = true;
    }else{
      ElMessage.error(res.data?.message);
    }
  }
};

const copyKey = (key: string) => {
  navigator.clipboard.writeText(key);
  console.log(key);
  ElMessage.success("密钥已复制到剪贴板");
};

const navigateToLogin = () => {
  router.push("/");
};
</script>
<template>
    <div class="register-container">
      <el-card class="register-card" shadow="hover">
        <h2 class="register-title">注册</h2>
        <el-form :model="registerForm" class="register-form" label-width="auto">
          <el-form-item label="用户名">
            <el-input v-model="registerForm.username" placeholder="请输入用户名"></el-input>
          </el-form-item>
          <el-form-item>
              <el-button type="primary" @click="handleRegister">注册</el-button>
              <el-button type="text" @click="navigateToLogin">返回登录</el-button>
          </el-form-item>
        </el-form>
      </el-card>
      <el-dialog v-model="dialogVisible" title="注册成功" width="400px">
        <p>请记住您的密钥，密钥用于登录和API请求。</p>
        <p>密钥：{{ key }}</p>
        <div class="dialog-footer">
          <el-button type="text" @click="copyKey(key)">复制密钥</el-button>
          <el-button type="primary" @click="navigateToLogin">返回登录</el-button>
        </div>
      </el-dialog>
    </div>
  </template>
<style scoped>
  .register-container {
    display: flex;
    justify-content: center;
    align-items: center;
    height: 100vh;
    background-color: #f5f5f5;
  }
  .register-card {
    width: 400px;
    padding: 20px;
  }
  .register-title {
    text-align: center;
    margin-bottom: 20px;
  }
  .dialog-footer {
    display: flex;
    justify-content: space-between;
  }
</style>
  