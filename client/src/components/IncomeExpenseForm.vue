<template>
  <div class="form-container">
    <div class="form-grid">
      <div class="form-item">
        <label>条目名称</label>
        <el-input v-model="form.name" placeholder="请输入名称" />
      </div>
      <div class="form-item">
        <label>金额</label>
        <el-input-number
          v-model="form.amount"
          :min="0"
          :max="999999"
          :precision="2"
          :step="0.01"
          placeholder="请输入金额"
        />
      </div>
      <div class="form-item">
        <label>类别</label>
        <el-select
          v-model="form.category"
          multiple
          placeholder="请选择类别"
          style="width: 100%"
        >
          <el-option
            v-for="category in categories"
            :key="category"
            :label="category"
            :value="category"
          />
        </el-select>
      </div>
      <div class="form-item">
        <button class="submit-button" @click="addRecord">添加记录</button>
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.form-container {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 25px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.form-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  align-items: end;
}

.form-item {
  label {
    display: block;
    color: white;
    margin-bottom: 8px;
    font-size: 14px;
    font-weight: 500;
  }
}

.submit-button {
  width: 100%;
  padding: 12px;
  background: linear-gradient(
    135deg,
    rgb(0, 225, 225) 0%,
    rgb(0, 195, 195) 100%
  );
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;

  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 225, 225, 0.3);
  }
}

:deep(.el-input__inner) {
  background: rgba(255, 255, 255, 0.1) !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
  color: white !important;
  height: 40px;
}

:deep(.el-input__wrapper) {
  background: rgba(255, 255, 255, 0.1) !important;
  box-shadow: none !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
}

:deep(.el-input-number__decrease),
:deep(.el-input-number__increase) {
  background: rgba(255, 255, 255, 0.1) !important;
  border-color: rgba(255, 255, 255, 0.2) !important;
  color: white !important;
}

:deep(.el-select__tags) {
  background: transparent !important;
}

// 响应式设计
@media (max-width: 1200px) {
  .form-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}
</style>

<script setup lang="ts">
import { ref } from "vue";
import { ElMessage } from "element-plus";

interface FormData {
  name: string;
  amount: number;
  category: string[];
}

const form = ref<FormData>({
  name: "",
  amount: 0,
  category: [],
});

const categories = ref(["食品", "交通", "娱乐", "其他"]);
const emit = defineEmits(["record-added"]);

const addRecord = () => {
  if (!form.value.name) {
    ElMessage.error("请输入条目名称");
    return;
  }
  if (form.value.amount <= 0) {
    ElMessage.error("金额必须大于0");
    return;
  }
  if (form.value.category.length === 0) {
    ElMessage.error("请选择至少一个类别");
    return;
  }

  const storedRecords = localStorage.getItem("records");
  const records = storedRecords ? JSON.parse(storedRecords) : [];
  records.push({
    ...form.value,
    date: new Date(),
    id: Date.now(), // 添加唯一ID
  });
  localStorage.setItem("records", JSON.stringify(records));
  emit("record-added");
  resetForm();
};

const resetForm = () => {
  form.value.name = "";
  form.value.amount = 0;
  form.value.category = [];
};
</script>
