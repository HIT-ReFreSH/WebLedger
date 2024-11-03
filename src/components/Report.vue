<template>
    <el-card>
        <h2>报表</h2>
        <el-table :data="summary" style="width: 100%">
            <el-table-column prop="category" label="类别" />
            <el-table-column prop="total" label="总金额" />
        </el-table>
    </el-card>
</template>

<script>
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

<style scoped>
.el-card {
    width: 100%;
}
</style>
