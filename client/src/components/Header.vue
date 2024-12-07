<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'

import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router';

const props = defineProps<{
    activeItem: number
}>()
const router = useRouter()

// 用于计数点击次数
const clickCount = ref(0)

// 点击按钮的处理函数
const open = () => {
    // 输出消息
    ElMessage({
        message: '版本号3.1.1',
        type: 'success',
    })

    // 增加点击次数
    clickCount.value++

    // 如果点击次数达到 5 次，跳转到 /account
    if (clickCount.value >= 5) {
        router.push('/account')
    }
    };
const nav_list = ref([
    { text: '记录收支', active: props.activeItem === 0, id: 0, src: '/' },
    { text: '查看报表', active: props.activeItem === 1, id: 1, src: '/about' },
    { text: '分类管理', active: props.activeItem === 2, id: 2, src: '/setting' },
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
            <RouterLink 
                v-for="item in nav_list" :key="item.id" :to="item.src"
                :class="{ 'nav-item': true, 'active': item.active }" 
                @click="onSwitch(item.id)">
                {{item.text }}
            </RouterLink>
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
