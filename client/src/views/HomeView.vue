<script setup lang="ts">
import { ref, onMounted } from "vue";
import Report from "../components/Report.vue";
import IncomeExpenseForm from "../components/IncomeExpenseForm.vue";
import RecordList from "../components/RecordList.vue";

interface Record {
  name: string;
  amount: number;
  category: string[];
  date: string;
}

const records = ref<Record[]>([]);

onMounted(() => {
  fetchRecords();
});

const fetchRecords = () => {
  const storedRecords = localStorage.getItem("records");
  records.value = storedRecords ? JSON.parse(storedRecords) : [];
};
</script>

<template>
  <section>
    <div class="hero-section-container">
      <div class="content-grid">
        <div class="form-section">
          <div class="header-content">
            <h1>个人记账本</h1>
            <p class="subtitle">轻松记录每一笔收支</p>
          </div>
          <IncomeExpenseForm @record-added="fetchRecords" />
        </div>

        <div class="records-section">
          <div class="records-wrapper">
            <RecordList :records="records" @record-deleted="fetchRecords" />
          </div>
          <div class="report-wrapper">
            <Report :records="records" />
          </div>
        </div>
      </div>

      <div class="social-icons">
        <a href="https://github.com" target="_blank" title="GitHub">
          <i class="fa-brands fa-github"></i>
        </a>
        <a href="https://www.linkedin.com" target="_blank" title="LinkedIn">
          <i class="fa-brands fa-linkedin"></i>
        </a>
        <a href="https://www.instagram.com" target="_blank" title="Instagram">
          <i class="fa-brands fa-instagram"></i>
        </a>
        <a href="https://discord.com" target="_blank" title="Discord">
          <i class="fa-brands fa-discord"></i>
        </a>
      </div>
    </div>
  </section>
</template>

<style lang="scss" scoped>
section {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 20px;
  background: linear-gradient(
    135deg,
    rgba(0, 0, 0, 0.4) 0%,
    rgba(0, 0, 0, 0.7) 100%
  );
}

.hero-section-container {
  background: rgba(255, 255, 255, 0.1);
  min-height: 85vh;
  width: 90%;
  max-width: 1400px;
  border-radius: 30px;
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(10px);
  padding: 30px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.content-grid {
  display: grid;
  grid-template-rows: auto 1fr;
  gap: 30px;
  height: 100%;
  padding: 80px 0 20px 0;
}

.form-section {
  padding: 0 30px;
}

.records-section {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
  padding: 0 30px;
}

.records-wrapper,
.report-wrapper {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 20px;
  backdrop-filter: blur(5px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.header-content {
  text-align: center;
  margin-bottom: 30px;
}

h1 {
  font-size: 36px;
  color: white;
  margin-bottom: 10px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.subtitle {
  color: rgba(255, 255, 255, 0.8);
  font-size: 18px;
}

.right-section {
  display: flex;
  justify-content: center;
  align-items: center;

  .image-wrap {
    animation: float 6s ease-in-out infinite;
    img {
      width: 100%;
      max-width: 300px;
      filter: drop-shadow(0 0 10px rgba(0, 225, 225, 0.5));
    }
  }
}

@keyframes float {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
}

.social-icons {
  margin-top: 30px;
  display: flex;
  justify-content: center;
  gap: 20px;

  a {
    color: white;
    font-size: 24px;
    transition: all 0.3s ease;
    opacity: 0.7;
    padding: 10px;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.1);
    display: flex;
    align-items: center;
    justify-content: center;
    width: 50px;
    height: 50px;
    border: 1px solid rgba(255, 255, 255, 0.2);

    &:hover {
      transform: translateY(-3px);
      opacity: 1;
      background: rgba(255, 255, 255, 0.2);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    }
  }
}

// 响应式设计
@media (max-width: 1024px) {
  .hero-content {
    grid-template-columns: 1fr;
  }

  .right-section {
    display: none;
  }
}

@media (max-width: 768px) {
  .hero-section-container {
    width: 95%;
    padding: 20px;
  }

  .left-content-container {
    flex-direction: column;
    gap: 30px !important;

    .container {
      width: 100% !important;
    }
  }
}
</style>
