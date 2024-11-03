<script setup lang="ts">
import { ref, onMounted } from 'vue'
import IncomeExpenseForm from '../components/IncomeExpenseForm.vue';
import RecordList from '../components/RecordList.vue';
import Report from '../components/Report.vue';


onMounted(() => {
  fetchRecords()
})

const records = ref([])
const fetchRecords = () => {
  // Fetch records from API
  const storedRecords = JSON.parse(localStorage.getItem('records')) || [];
  records.value = storedRecords;
}
</script>

<template>
  <!--Hero section-->
  <section>
    <div class="hero-section-container">

      <div class="hero-content"> <!--Hero Content-->

        <div class="left-section">
          <div class="left-content-container">
            <div class="container">
              <h1>账本系统</h1>
              <IncomeExpenseForm @record-added="fetchRecords" />
            </div>
            <div class="container">
              <RecordList :records="records" />
              <Report :records="records" />
            </div>
          </div>
        </div>

        <div class="right-section">
          <div class="image-wrap">
            <img src="../assets/img/planet.png" alt="planet image">
          </div>
        </div>

      </div>

      <div class="social-icons">
        <i class="fa-brands fa-facebook"></i>
        <i class="fa-brands fa-youtube"></i>
        <i class="fa-brands fa-twitter"></i>
        <i class="fa-brands fa-tiktok"></i>
      </div>

    </div>
  </section>
</template>

<style lang="scss" scoped>
section {
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
}

.hero-section-container {
  background-color: rgba(255, 255, 255, 0.1);
  height: 85vh;
  width: 85%;
  border-radius: 30px;
  border: 2px solid rgba(211, 211, 211, 0.2);
  backdrop-filter: blur(8px);

  /* Hero Content */
  .hero-content {
    padding: 15px 25px 0 25px;
    display: grid;
    grid-template-columns: 4fr 1fr;

    img {
      width: 90%;
      filter: drop-shadow(0 0 10px rgb(0, 225, 225)) drop-shadow(0 0 20px rgb(0, 225, 225)) drop-shadow(0 0 40px rgb(0, 225, 225)) drop-shadow(0 0 100px rgb(0, 225, 225));
    }

    .right-section {
      display: flex;
      justify-content: center;
      align-items: center;

      /* Right Section Animation */
      @keyframes rotatePlanet {
        from {
          transform: rotate(0deg);
        }

        to {
          transform: rotate(360deg);
        }
      }

      .image-wrap {
        display: flex;
        justify-content: center;
        align-items: center;
        animation: rotatePlanet 120s linear infinite;
      }
    }

    /* left Section
        h3 {
          font-size: 4rem;
          letter-spacing: 20px;
          color: white;
        }
    
         
    
    */
    .left-section {
      display: flex;
      align-items: center;
      z-index: 1;
      height: 70vh;

      h1 {
        font-size: 3rem;
        font-weight: 800;
        color: white;
        margin: -20px 0 0 0;
      }

      .left-content-container {
        display: flex;
        align-items: center;
        gap: 100px;
        padding: 30px;


        .container {
          display: flex;
          flex-direction: column;
          align-items: center;
          gap: 30px;
        }
      }

    }
  }
}

.social-icons {
  color: white;
  display: flex;
  justify-content: center;
  gap: 20px;
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
