<script setup lang="ts">
import { ref, onMounted } from 'vue'

import { ElMessage } from 'element-plus'

const open = () => {
    ElMessage({
        message: '啥也没有',
        type: 'success',
    })
};
const nav_list = ref([
    { text: 'Home', active: true, id: 0, src: '/' },
    { text: 'About', active: false, id: 1, src: '/about' },
]);

const onSwitch = (index: number) => {
    nav_list.value.forEach(item => {
        item.active = false;
        if (item.id === index) item.active = true;
    });
};
</script>

<template>
    <header>
        <a class="logo" href="#hero">Report<span>Query.</span></a> <!--Left Section-->
        <nav> <!--Middle Section-->
            <RouterLink v-for="item in nav_list" :key="item.id" :to="item.src"
                :class="{ 'nav-item': true, 'active': item.active }" @click="onSwitch(item.id)">{{
                    item.text }}</RouterLink>
        </nav>

        <button class="cta-btn" @click="open">Click</button> <!--Right Section-->
    </header>
</template>

<style lang="scss" scoped>
/* Header */
header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 5px 15px;
    margin: 10px;
    //z-index: 1;
    /* Appling Animation */

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
            content: '';
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
        font-size: 18px;
        font-weight: bold;
        padding: 7px 25px;
        background-color: transparent;
        border: 1px solid white;
        border-radius: 20px;
        color: white;
        transition: 0.3s;
        cursor: pointer;

        &:hover {
            background-color: rgb(0, 225, 225);
            color: rgba(0, 0, 0, 0.6);
            border-color: rgb(0, 225, 225);
        }
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

.active {
    color: rgb(0, 225, 225);
}

.active::after {
    background-color: rgb(0, 225, 225);
}
</style>
