<template>
  <div class="accounts-management">
    <el-card class="accounts-card" shadow="hover">
      <h2 class="accounts-title">Access 管理</h2>

      <!-- Access 列表 -->
      <el-table :data="accessList" class="accounts-table" border>
        <el-table-column label="Access Name" prop="name"></el-table-column>
        <el-table-column label="Key" prop="key"></el-table-column>
        <el-table-column label="操作" width="120">
          <template #default="{ row }">
            <el-button
              type="text"
              size="small"
              @click="handleDeleteAccess(row.name)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 添加 Access -->
      <div class="accounts-actions">
        <el-input
          v-model="newAccessName"
          placeholder="请输入新的 Access 名称"
          style="margin-right: 10px;"
        />
        <el-button :loading="isLoading" type="primary" @click="handleAddAccess">添加</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { addAccess, removeAccess, getAllAccess } from "@/api/user";

// 数据状态
const accessList = ref<Access[]>([]);
const newAccessName = ref("");
const isLoading = ref(false);

// 错误处理
const handleError = (error: unknown) => {
  if (error instanceof Error) {
    ElMessage.error("请求失败：" + error.message);
  } else {
    ElMessage.error("请求失败，未知错误");
  }
};

// 获取所有 Access
const fetchAccessList = async () => {
  try {
    const res = await getAllAccess();
    if (res?.status === 200) {
      accessList.value = res.data.map((item: any) => ({
        name: item.name,
        key: item.key,
      }));
    } else {
      ElMessage.error("获取 Access 列表失败");
    }
  } catch (error) {
    handleError(error);
  }
};

// 验证 Access 名称
const validateAccessName = (name: string) => {
  const regex = /^[a-zA-Z0-9_-]{3,20}$/;
  return regex.test(name);
};

// 添加新的 Access
const handleAddAccess = async () => {
  if (isLoading.value) return; // 防止重复提交
  if (!newAccessName.value.trim() || !validateAccessName(newAccessName.value)) {
    ElMessage.error("请输入有效的 Access 名称（3-20个字符，仅允许字母、数字、短划线和下划线）");
    return;
  }
  isLoading.value = true;
  try {
    await addAccess(newAccessName.value);
    ElMessage.success("Access 添加成功");
    newAccessName.value = ""; // 清空输入框
    fetchAccessList(); // 刷新列表
  } catch (error) {
    handleError(error);
  } finally {
    isLoading.value = false;
  }
};

// 删除 Access
const handleDeleteAccess = async (name: string) => {
  try {
    await ElMessageBox.confirm(
      `确认删除 Access：${name} 吗？删除后无法恢复，请谨慎操作！`,
      "删除确认",
      { type: "warning" }
    );
    await removeAccess(name);
    ElMessage.success("Access 删除成功");
    fetchAccessList();
  } catch (error) {
    if (error !== "cancel") {
      handleError(error);
    }
  }
};

// 页面加载时获取 Access 列表
onMounted(() => {
  fetchAccessList();
});
</script>

<style scoped>
.accounts-management {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh; /* 使用视口高度，保证全屏显示 */
  min-height: 500px; /* 最小高度 */
  background-image: url('/client\src\assets\img\lake.jpg');
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-color: rgba(255, 255, 255, 0.2);
}

.accounts-card {
  width: 1000px;
  padding: 20px;
}

.accounts-title {
  text-align: center;
  margin-bottom: 30px;
}

.accounts-table {
  margin-top: 30px;
}

.accounts-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 30px;
}
</style>
