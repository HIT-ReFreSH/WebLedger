<script lang="ts" setup>
import { ref, defineProps } from 'vue'
const props = defineProps({
    records: {
        type: Array,
        required: true
    }
})

const formatDate = (row: any) => {
    return new Date(row.date).toLocaleDateString();
}
</script>

<template>
    <el-table :data="props.records" style="width:100%" height="200">
        <el-table-column prop="name" label="条目名称" />
        <el-table-column prop="amount" label="金额" />
        <el-table-column prop="category" label="类别" />
        <el-table-column prop="date" label="日期" :formatter="formatDate" />
    </el-table>
</template>

<style scoped>
.el-table {
    border: 3px solid rgba(0, 0, 0, .6);
    border-radius: 12px;
}
</style>
