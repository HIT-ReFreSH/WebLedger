<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const isLogin = ref(true);
const message = ref({ text: "", type: "" });

const loginForm = ref({
  username: "",
  password: "",
});

const showMessage = (text: string, type: "success" | "error") => {
  message.value = { text, type };
  setTimeout(() => {
    message.value.text = "";
  }, 3000);
};

const handleLogin = () => {
  if (!loginForm.value.username || !loginForm.value.password) {
    showMessage("用户名和密码不能为空", "error");
    return;
  }

  const storedUser = localStorage.getItem("registeredUser");
  if (!storedUser) {
    showMessage("用户不存在，请先注册", "error");
    return;
  }

  const user = JSON.parse(storedUser);
  if (
    user.username === loginForm.value.username &&
    user.password === loginForm.value.password
  ) {
    localStorage.setItem(
      "user",
      JSON.stringify({
        username: loginForm.value.username,
        isLoggedIn: true,
      })
    );
    showMessage("登录成功", "success");
    setTimeout(() => {
      router.push("/");
    }, 1000);
  } else {
    showMessage("用户名或密码错误", "error");
  }
};

const handleRegister = () => {
  if (!loginForm.value.username || !loginForm.value.password) {
    showMessage("用户名和密码不能为空", "error");
    return;
  }

  localStorage.setItem(
    "registeredUser",
    JSON.stringify({
      username: loginForm.value.username,
      password: loginForm.value.password,
    })
  );

  showMessage("注册成功，请登录", "success");
  isLogin.value = true;
  loginForm.value.password = "";
};

const switchMode = () => {
  isLogin.value = !isLogin.value;
  loginForm.value.username = "";
  loginForm.value.password = "";
  message.value.text = "";
};
</script>

<template>
  <div class="login-container">
    <div class="login-box">
      <div class="form-header">
        <h2>{{ isLogin ? "欢迎回来" : "创建账号" }}</h2>
        <p>{{ isLogin ? "很高兴再次见到你" : "开始你的记账之旅" }}</p>
      </div>
      <div class="message" v-if="message.text" :class="message.type">
        {{ message.text }}
      </div>
      <form>
        <div class="form-group">
          <label>用户名</label>
          <input
            type="text"
            v-model="loginForm.username"
            placeholder="请输入用户名"
            class="input-field"
          />
        </div>
        <div class="form-group">
          <label>密码</label>
          <input
            type="password"
            v-model="loginForm.password"
            placeholder="请输入密码"
            class="input-field"
          />
        </div>
        <button
          type="button"
          class="submit-button"
          @click="isLogin ? handleLogin() : handleRegister()"
        >
          {{ isLogin ? "登录" : "注册" }}
        </button>
        <div class="switch-mode">
          <p>{{ isLogin ? "还没有账号?" : "已有账号?" }}</p>
          <a href="#" @click.prevent="switchMode">
            {{ isLogin ? "立即注册" : "立即登录" }}
          </a>
        </div>
      </form>
      <div class="social-icons">
        <p class="social-text">社交账号登录</p>
        <div class="icons-wrapper">
          <a href="https://github.com" target="_blank" title="GitHub">
            <i class="fa-brands fa-github"></i>
          </a>
          <a href="https://www.google.com" target="_blank" title="Google">
            <i class="fa-brands fa-google"></i>
          </a>
          <a href="https://www.apple.com" target="_blank" title="Apple">
            <i class="fa-brands fa-apple"></i>
          </a>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(
    135deg,
    rgba(0, 0, 0, 0.4) 0%,
    rgba(0, 0, 0, 0.7) 100%
  );
}

.login-box {
  width: 400px;
  padding: 40px;
  border-radius: 20px;
  background: rgba(255, 255, 255, 0.9);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  backdrop-filter: blur(10px);
}

.form-header {
  text-align: center;
  margin-bottom: 30px;
}

.form-header h2 {
  color: #333;
  font-size: 28px;
  margin-bottom: 10px;
}

.form-header p {
  color: #666;
  font-size: 16px;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  color: #333;
  font-size: 14px;
  font-weight: 500;
}

.input-field {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #eee;
  border-radius: 12px;
  font-size: 14px;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.9);
}

.input-field:focus {
  outline: none;
  border-color: #409eff;
  box-shadow: 0 0 0 3px rgba(64, 158, 255, 0.1);
}

.submit-button {
  width: 100%;
  padding: 14px;
  background: linear-gradient(135deg, #409eff 0%, #66b1ff 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.submit-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(64, 158, 255, 0.3);
}

.switch-mode {
  text-align: center;
  margin-top: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
}

.switch-mode p {
  color: #666;
  font-size: 14px;
}

.switch-mode a {
  color: #409eff;
  text-decoration: none;
  font-size: 14px;
  font-weight: 600;
}

.switch-mode a:hover {
  text-decoration: underline;
}

.message {
  padding: 12px 16px;
  border-radius: 12px;
  margin-bottom: 20px;
  text-align: center;
  font-size: 14px;
  animation: slideDown 0.3s ease;
}

@keyframes slideDown {
  from {
    transform: translateY(-20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.message.success {
  background-color: #f0f9eb;
  color: #67c23a;
  border: 1px solid #c2e7b0;
}

.message.error {
  background-color: #fef0f0;
  color: #f56c6c;
  border: 1px solid #fbc4c4;
}

.social-icons {
  margin-top: 30px;
  text-align: center;
}

.social-text {
  color: #666;
  font-size: 14px;
  margin-bottom: 15px;
}

.icons-wrapper {
  display: flex;
  justify-content: center;
  gap: 20px;
}

.social-icons a {
  color: #666;
  font-size: 20px;
  transition: all 0.3s ease;
  padding: 10px;
  border-radius: 50%;
  background: #f5f5f5;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 45px;
  height: 45px;
}

.social-icons a:hover {
  transform: translateY(-3px);
  background: #409eff;
  color: white;
}
</style>
