<script setup lang="ts">
import Header from '../components/Header.vue';
import { onMounted } from 'vue';
import { useUserStore } from '@/stores/user';
import { useRouter } from 'vue-router';

const router = useRouter();

onMounted(()=>{
    if(useUserStore().access==null||useUserStore().secret==null){
        router.push('/');
    }
})
</script>

<template>
    <Header />
    <div class="title">
        请期待新功能！！
    </div>

</template>

<style lang="scss" scoped>
.title {
    font-size: 11rem;
    font-weight: 800;
    color: #fff;
    width: 100%;
    text-align: center;
}
</style>
