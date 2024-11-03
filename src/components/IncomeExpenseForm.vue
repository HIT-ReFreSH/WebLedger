<script lang="ts" setup>
import { ref, onMounted, defineEmits } from 'vue'
const form = ref({
    name: '',
    amount: 0,
    category: []
})
const categories = ref(['食品', '交通', '娱乐', '其他'])
const emit = defineEmits(['record-added'])

const addRecord = () => {
    console.log('simon');

    const records = JSON.parse(localStorage.getItem('records')) || [];
    records.push({ ...form.value, date: new Date() });
    localStorage.setItem('records', JSON.stringify(records));
    emit('record-added');
    resetForm();
}
const resetForm = () => {
    form.value.name = '';
    form.value.amount = 0;
    form.value.category = [];
}
</script>

<template>
    <el-form :model="form" @submit.prevent="addRecord">
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;">条目名称</label>
            <el-input v-model="form.name" />
        </el-form-item>
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;margin-right: 20px;">金额</label>
            <el-input-number v-model="form.amount" />
        </el-form-item>
        <el-form-item>
            <label slot="label" style="color:#fff;font-size: 18px;">类别</label>
            <el-select v-model="form.category" multiple>
                <el-option v-for="category in categories" :key="category" :label="category" :value="category" />
            </el-select>
        </el-form-item>
        <button class="big-cta-btn" type="submit">添加记录</button>
    </el-form>
</template>

<style scoped>
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

.big-cta-btn {
    animation: sideInLeft 1s ease-out forwards;
    opacity: 0;
    animation-delay: 0.8s;
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
</style>
