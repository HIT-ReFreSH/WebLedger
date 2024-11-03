<script setup lang="ts">

</script>

<template>
  <!--Hero section-->
  <section>
    <div class="hero-section-container">

      <div class="hero-content"> <!--Hero Content-->

        <div class="left-section">
          <div class="left-content-container">
            <button class="big-cta-btn">Donate</button>
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
    grid-template-columns: 1.2fr 1fr;

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

    /* left Section */
    .left-section {
      display: flex;
      align-items: center;
      z-index: 1;

      h3 {
        font-size: 4rem;
        letter-spacing: 20px;
        color: white;
      }

      h1 {
        font-size: 11rem;
        font-weight: 800;
        color: white;
        margin: -20px 0 0 0;
      }

      .big-cta-btn {
        font-size: 20px;
        font-weight: bold;
        letter-spacing: 2px;
        width: 250px;
        height: 55px;
        border-radius: 50px;
        background-color: rgb(0, 225, 225);
        color: rgba(0, 0, 0, 0.699);
        text-transform: uppercase;
        transition: all 0.3s;
      }

      .big-cta-btn:hover {
        letter-spacing: 5px;
        background-color: white;
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

.big-cta-btn {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.8s;
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
