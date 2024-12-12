<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Report from '../components/Report.vue';
import IncomeExpenseForm from '../components/IncomeExpenseForm.vue';
import RecordList from '../components/RecordList.vue';
import Header from '../components/Header.vue';
import { useUserStore } from '@/stores/user';
import { useRouter } from 'vue-router';


const router = useRouter();

const records = ref([])

const navigateToGithub = () => {
  window.open('https://github.com/HIT-ReFreSH/WebLedger', '_blank');
}
onMounted(() => {
  if (useUserStore().access == "" || useUserStore().secret == "") {
    router.push('/');
  }
})
</script>

<template>
  <!--Hero section-->
  <Header :activeItem="0" />
  <section>
    <div class="hero-section-container">
      <div class="left-container">
        <!-- 左容器的内容 -->
        <h1>账本系统</h1>
        <IncomeExpenseForm />
      </div>
      <div class="right-container">
        <!-- 右容器的内容 -->
        <RecordList :records="records" />
      </div>
    </div>
  </section>


  <div class="social-icons">
    <i class="fa-brands fa-github" @click="navigateToGithub"></i>
  </div>
</template>

<style lang="scss" scoped>
section {
  display: flex;
  justify-content: center;
  align-items: center;
}

.hero-section-container {
  display: flex;
  // height: 100vh;
  margin: auto;
  border-radius: 30px;
  border: 2px solid rgba(211, 211, 211, 0.2);
  backdrop-filter: blur(8px);
}

.left-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px;
  flex: 0 0 30%;
  width: 100%;
  height: 100%;
}

.right-container {
  display: flex;
  flex: 0 0 70%;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding-right: 40px;
  height: 100%;
  margin: auto;
}

@media (max-width: 1080px) {
  section {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    height: 80vh;
    padding-bottom: 10px;
    /* 确保这个高度足够容纳所有子元素 */
    overflow: auto;
  }

  .hero-section-container {
    display: flex;
    flex-direction: column;
    /* 移除 overflow: scroll;，除非你需要滚动条 */
    height: 100%;
    border: 2px solid rgba(211, 211, 211, 0.2);

    backdrop-filter: blur(2px);
    /* 设置高度为100%以确保它填满父容器 */
  }

  .left-container,
  .right-container {
    margin-bottom: 20px;
    /* 例如，添加底部外边距 */
  }

  .right-container {
    position: relative;
    padding-left: 40px;
    border-radius: 30px;
    border: 2px solid rgba(211, 211, 211, 0.2);
    background-color: rgba(1, 105, 124, 0.527);

  }

  .transaction-table {
    width: 100%;
    max-width: 700px;
  }
}


h1 {
  font-size: 2.5rem;
  font-weight: 800;
  color: white;
  margin: 0;
}

.social-icons {
  color: white;
  display: flex;
  justify-content: center;
  margin-top: 20px;
}

.social-icons i {
  font-size: 18px;
  width: 10px;
  height: 10px;
  border: 1px solid white;
  padding: 15px;
  border-radius: 50px;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: all 0.3s;
}

.social-icons i:hover {
  color: rgb(0, 225, 225);
  border-color: rgb(0, 225, 225);
  transform: rotate(360deg) scale(1.1);
}

/* Hero Section Animation */
@keyframes sideInLeft {
  from {
    transform: translateX(-100%);
    opacity: 0;
  }

  to {
    transform: translateX(0%);
    opacity: 1;
  }
}

@keyframes sideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }

  to {
    transform: translateX(0%);
    opacity: 1;
  }
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

@keyframes bottomIn {
  from {
    transform: translateY(100%);
    opacity: 0;
  }

  to {
    transform: translateY(0%);
    opacity: 1;
  }
}

h3 {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
}

h1 {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.4s;
}

p {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.6s;
}

.social-icons {
  animation: bottomIn 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.2s;
}

.right-section img {
  animation: sideInRight 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.2s;
}
</style>
