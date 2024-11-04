<script lang="ts" setup>
import { ref, computed, defineProps } from 'vue'
const props = defineProps({
    records: {
        type: Array,
        required: true
    }
})
const summary = computed(
    () => {
        const summaryMap = props.records.reduce((acc, record) => {
            record.category.forEach(category => {
                acc[category] = (acc[category] || 0) + record.amount;
            });
            return acc;
        }, {});

        return Object.keys(summaryMap).map(key => ({
            category: key,
            total: summaryMap[key]
        }));
    }
)
</script>

<template>
    <el-card shadow="always" style="width: 100%;">
        <h2>报表</h2>
        <el-table :data="summary" style="width: 100%" height="180">
            <el-table-column prop="category" label="类别" />
            <el-table-column prop="total" label="总金额" />
        </el-table>
    </el-card>
</template>

<style scoped lang="scss"></style>
