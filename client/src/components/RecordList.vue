<script lang="ts" setup>
import { ref, defineProps, defineEmits } from "vue";
import { ElMessageBox, ElMessage } from "element-plus";

interface Record {
  id: number;
  name: string;
  amount: number;
  category: string[];
  date: string;
}

const props = defineProps<{
  records: Record[];
}>();

const emit = defineEmits(["record-deleted"]);

const formatDate = (row: Record) => {
  return new Date(row.date).toLocaleDateString();
};

const handleDelete = async (record: Record) => {
  try {
    await ElMessageBox.confirm("确定要删除这条记录吗？", "警告", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    });

    const storedRecords = localStorage.getItem("records");
    if (storedRecords) {
      const records = JSON.parse(storedRecords);
      const newRecords = records.filter((r: Record) => r.id !== record.id);
      localStorage.setItem("records", JSON.stringify(newRecords));
      emit("record-deleted");
      ElMessage.success("删除成功");
    }
  } catch {
    // 用户取消删除
  }
};
</script>

<template>
  <el-table :data="records" style="width: 100%" height="200">
    <el-table-column prop="name" label="条目名称" />
    <el-table-column prop="amount" label="金额">
      <template #default="{ row }">
        <span class="amount">{{ row.amount.toFixed(2) }}</span>
      </template>
    </el-table-column>
    <el-table-column prop="category" label="类别">
      <template #default="{ row }">
        <div class="category-tags">
          <span v-for="cat in row.category" :key="cat" class="category-tag">
            {{ cat }}
          </span>
        </div>
      </template>
    </el-table-column>
    <el-table-column prop="date" label="日期" :formatter="formatDate" />
    <el-table-column label="操作" width="100">
      <template #default="{ row }">
        <button class="delete-btn" @click="handleDelete(row)">
          <i class="fas fa-trash"></i>
        </button>
      </template>
    </el-table-column>
  </el-table>
</template>

<style lang="scss" scoped>
.amount {
  color: #67c23a;
  font-weight: 600;
}

.category-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 4px;
}

.category-tag {
  background: rgba(64, 158, 255, 0.1);
  color: #409eff;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.delete-btn {
  background: transparent;
  border: none;
  color: #f56c6c;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.3s ease;

  &:hover {
    background: rgba(245, 108, 108, 0.1);
  }

  i {
    font-size: 16px;
  }
}

:deep(.el-table) {
  background: transparent !important;

  th {
    background: rgba(255, 255, 255, 0.1) !important;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1) !important;
    color: white !important;
  }

  td {
    background: transparent !important;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1) !important;
    color: white !important;
  }
}
</style>

export default {}
