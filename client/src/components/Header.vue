<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { ElMessage } from "element-plus";

const router = useRouter();

const open = () => {
  ElMessage({
    message: "啥也没有",
    type: "success",
  });
};
const nav_list = ref([
  { text: "Home", active: true, id: 0, src: "/" },
  { text: "About", active: false, id: 1, src: "/about" },
]);

const onSwitch = (index: number) => {
  nav_list.value.forEach((item) => {
    item.active = false;
    if (item.id === index) item.active = true;
  });
};

const handleLogout = () => {
  localStorage.removeItem("user");
  ElMessage.success("已退出登录");
  router.push("/login");
};
</script>

<template>
  <header>
    <a class="logo" href="#hero">Report<span>Query.</span></a>
    <nav>
      <RouterLink
        v-for="item in nav_list"
        :key="item.id"
        :to="item.src"
        :class="{ 'nav-item': true, active: item.active }"
        @click="onSwitch(item.id)"
        >{{ item.text }}</RouterLink
      >
    </nav>

    <div class="right-buttons">
      <button class="cta-btn" @click="open">Click</button>
      <button class="logout-btn" @click="handleLogout">退出登录</button>
    </div>
  </header>
</template>

<style lang="scss" scoped>
header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 30px;
  margin: 20px 30px;
  border-radius: 15px;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  z-index: 1000;

  animation: topIn 1.2s ease-out forwards;
  opacity: 0;
  animation-delay: 0.2s;

  .logo {
    font-size: 24px;
    font-weight: bold;
    color: white;

    span {
      color: rgb(0, 225, 225);
    }
  }

  nav {
    display: flex;
    align-items: center;
    gap: 30px;

    a {
      color: white;
      font-size: 18px;
      font-weight: 500;
      position: relative;
      display: flex;
      justify-content: center;
    }

    a::after {
      content: "";
      position: absolute;
      bottom: -10px;
      height: 2px;
      width: 80%;
      box-shadow: 0 0 5px white;
      transform: scaleX(0) translateY(-100%);
      background-color: white;
      transition: 0.3s;
    }

    a:hover::after {
      transform: scaleX(0.5) translateY(0);
    }
  }

  button {
    font-size: 16px;
    font-weight: 600;
    padding: 8px 20px;
    background: transparent;
    border: 1px solid rgba(255, 255, 255, 0.5);
    border-radius: 12px;
    color: white;
    transition: all 0.3s ease;
    cursor: pointer;

    &:hover {
      background: rgba(0, 225, 225, 0.2);
      border-color: rgb(0, 225, 225);
      transform: translateY(-2px);
    }
  }
}

.right-buttons {
  display: flex;
  gap: 15px;
}

.logout-btn {
  border-color: rgba(255, 99, 99, 0.5) !important;
  color: rgb(255, 99, 99) !important;

  &:hover {
    background: rgba(255, 99, 99, 0.2) !important;
    border-color: rgb(255, 99, 99) !important;
  }
}

.active {
  color: rgb(0, 225, 225) !important;
}

.active::after {
  background-color: rgb(0, 225, 225) !important;
}

@keyframes topIn {
  from {
    transform: translateY(-100%);
    opacity: 0;
  }

  to {
    transform: translateY(0%);
    opacity: 1;
  }
}

/* 响应式设计 */
@media (max-width: 768px) {
  header {
    margin: 10px;
    padding: 10px 15px;

    .logo {
      font-size: 20px;
    }

    nav {
      gap: 15px;

      a {
        font-size: 16px;
      }
    }

    button {
      font-size: 14px;
      padding: 6px 15px;
    }
  }
}
</style>
