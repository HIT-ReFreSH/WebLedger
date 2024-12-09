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
        <el-button type="primary" @click="handleAddAccess">添加</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { addAccess, removeAccess, getAllAccess } from "@/api/user"; // 你之前提供的接口方法

// 数据状态
const accessList = ref<Array<{ name: string; key: string }>>([]);
const newAccessName = ref("");

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
  } catch (error: unknown) {
    // 错误类型为 unknown，进行类型检查
    if (error instanceof Error) {
      ElMessage.error("请求失败：" + error.message);
    } else {
      ElMessage.error("请求失败，未知错误");
    }
  }
};

// 添加新的 Access
const handleAddAccess = async () => {
  if (!newAccessName.value.trim()) {
    ElMessage.error("请输入 Access 名称");
    return;
  }
  try {
    const secret = await addAccess(newAccessName.value);
    ElMessage.success("Access 添加成功");
    newAccessName.value = ""; // 清空输入框
    fetchAccessList(); // 刷新列表
  } catch (error: unknown) {
    if (error instanceof Error) {
      ElMessage.error("请求失败：" + error.message);
    } else {
      ElMessage.error("请求失败，未知错误");
    }
  }
};

// 删除 Access
const handleDeleteAccess = async (name: string) => {
  try {
    await ElMessageBox.confirm(
      `确认删除 Access：${name} 吗？`,
      "删除确认",
      { type: "warning" }
    );
    await removeAccess(name);
    ElMessage.success("Access 删除成功");
    fetchAccessList(); // 刷新列表
  } catch (error: unknown) {
    if (error !== "cancel") {
      if (error instanceof Error) {
        ElMessage.error("请求失败：" + error.message);
      } else {
        ElMessage.error("请求失败，未知错误");
      }
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
  height: 960px;
  background-image: url('/client\src\assets\img\lake.jpg'); /* 背景图片路径 */
  background-size: cover; /* 图片覆盖整个背景 */
  background-position: center center; /* 确保图片居中显示 */
  background-repeat: no-repeat; /* 防止背景图片重复 */
  background-color: rgba(255, 255, 255, 0.2); /* 透明白色背景，透明度50% */
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
