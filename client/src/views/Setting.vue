<script setup lang="ts">
import AddAutomation from '@/components/AddAutomation.vue';
import AddCategory from '@/components/AddCategory.vue';
import AddTemplate from '@/components/AddTemplate.vue';

</script>
<template>  
  <Header :activeItem="2" /> 
  <section>
    <div class="hero-section-container">
        <div class="left-section">
            <AddCategory />
            <AddTemplate />
            <div style="grid-column: span 2;">
                <AddAutomation/>
            </div>
        </div>
        <div class="right-section">
            <div class="image-wrap">
                <img src="../assets/img/planet.png" alt="planet image">
            </div>
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

  display: flex;
  flex-direction: row;

img {
    width: 90%;
    filter: drop-shadow(0 0 10px rgb(0, 225, 225)) drop-shadow(0 0 20px rgb(0, 225, 225)) drop-shadow(0 0 40px rgb(0, 225, 225)) drop-shadow(0 0 100px rgb(0, 225, 225));
}

.right-section {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30%;
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

.left-section {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-template-rows: 1fr 1fr;
    z-index: 1;
    height: 100%;
    width: 100%;
    gap: 20px;

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
        width: 100%;

        .big-container {
            width: 50%;
        }
    }

}
}
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

</style>