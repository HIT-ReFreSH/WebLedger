<script lang="ts" setup>
import { ref, onMounted, defineEmits } from 'vue'
import { getCategories, type Entry,addEntry } from '@/api/record';
import { ElMessage } from 'element-plus';

const form = ref<Entry>({
    isIncome: false,
    amount: 0,
    givenTime: '',
    type: '',
    category: '',
    description: ''
})
const categories = ref<any[]>([])
const emit = defineEmits(['record-added'])

const addRecord = async () => {
    if(!checkForm()){
        ElMessage.error('请完整填写表单');
        return;
    }   
    form.value.givenTime = new Date(form.value.givenTime).toISOString();
    if(!form.value.isIncome){
        form.value.amount=-Math.abs(form.value.amount);
    }
    const res = await addEntry(form.value);
    if(res?.status===200){
        window.location.reload();
    }
}
const checkForm = (): boolean => {
    return form.value.amount !== 0 && form.value.type !== '' && form.value.category !== '' && form.value.description !== '' && form.value.givenTime!=='';
}
onMounted(async () => {
    const res = await getCategories();
    if(res?.status===200){
        res.data.forEach((item: any) => {
            categories.value.push(item.name);
        })
    }
})
</script>

<template>
    <el-form :model="form" @submit.prevent="addRecord">
        <el-form-item required>
            <el-radio-group v-model="form.isIncome">
                <el-radio :label="true" style="color:#fff;font-size: 18px;">收入</el-radio>
                <el-radio :label="false" style="color:#fff;font-size: 18px;">支出</el-radio>
            </el-radio-group>
        </el-form-item>
        <el-form-item required>
            <el-date-picker v-model="form.givenTime" type="datetime" placeholder="入账时间" />
        </el-form-item>
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;">分类</label>
            <el-select v-model="form.category" placeholder="请选择分类">
                <el-option v-for="category in categories" :key="category" :label="category" :value="category" />
            </el-select>
        </el-form-item>
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;">具体类别</label>
            <el-input v-model="form.type"  placeholder="如饮料，水果"/>
        </el-form-item>
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;">描述</label>
            <el-input v-model="form.description" placeholder="对这笔账目的描述" />
        </el-form-item>
        <el-form-item required>
            <label slot="label" style="color:#fff;font-size: 18px;margin-right: 20px;">金额</label>
            <el-input-number v-model="form.amount" />
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
