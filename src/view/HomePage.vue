<template>
  <div class="home-page">
    <!-- 新增和编辑表单 -->
    <form @submit.prevent="handleSubmit" class="form-container">
      <input v-model="newItem.name" placeholder="名称" required />
      <input v-model="newItem.description" placeholder="描述" required />
      <button type="submit">{{ editingIndex === -1 ? "新增" : "保存" }}</button>
      <button type="button" @click="resetForm" v-if="editingIndex !== -1">
        取消
      </button>
    </form>

    <!-- 查询功能 -->
    <input
      v-model="query"
      placeholder="搜索名称或描述"
      @input="searchItems"
      class="search-input"
    />

    <!-- 数据报表 -->
    <table class="report-table">
      <thead>
        <tr>
          <th>名称</th>
          <th>描述</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(item, index) in filteredItems" :key="index">
          <td>{{ item.name }}</td>
          <td>{{ item.description }}</td>
          <td>
            <button @click="editItem(index)" class="action-button">编辑</button>
            <button @click="deleteItem(index)" class="action-button">
              删除
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script>
export default {
  name: "HomePage",
  data() {
    return {
      items: [
        { name: "项目1", description: "描述1" },
        { name: "项目2", description: "描述2" },
      ], // 初始数据
      newItem: { name: "", description: "" }, // 新增和编辑的表单数据
      editingIndex: -1, // 编辑中的项目索引，-1 表示不在编辑状态
      query: "", // 查询关键字
      filteredItems: [], // 过滤后的数据
    };
  },
  mounted() {
    // 初始化时，显示全部数据
    this.filteredItems = this.items;
  },
  methods: {
    handleSubmit() {
      if (this.editingIndex === -1) {
        // 新增数据
        this.items.push({ ...this.newItem });
      } else {
        // 更新数据
        this.items[this.editingIndex] = { ...this.newItem };
        this.editingIndex = -1; // 退出编辑状态
      }
      this.resetForm();
      this.searchItems(); // 提交后重新搜索
    },
    editItem(index) {
      // 进入编辑状态
      this.editingIndex = index;
      this.newItem = { ...this.items[index] };
    },
    deleteItem(index) {
      // 删除数据
      this.items.splice(index, 1);
      this.searchItems(); // 删除后重新搜索
    },
    resetForm() {
      // 重置表单
      this.newItem = { name: "", description: "" };
      this.editingIndex = -1;
    },
    searchItems() {
      // 根据查询关键字过滤数据
      this.filteredItems = this.items.filter((item) => {
        return (
          item.name.includes(this.query) ||
          item.description.includes(this.query)
        );
      });
    },
  },
};
</script>
<style scoped>
/* 报表样式 */
.home-page {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  margin-top: 60px;
}

.form-container {
  margin-bottom: 20px;
}

.form-container input {
  margin-right: 10px;
  padding: 5px;
}

.search-input {
  margin-bottom: 20px;
  padding: 8px;
  width: 300px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.report-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.report-table th,
.report-table td {
  border: 1px solid #ddd;
  padding: 12px;
  text-align: left;
}

.report-table th {
  background-color: #f2f2f2;
  font-weight: bold;
}

.report-table tr:nth-child(even) {
  background-color: #f9f9f9;
}

.action-button {
  margin: 5px;
  padding: 5px 10px;
  border: none;
  background-color: #007bff;
  color: white;
  border-radius: 4px;
  cursor: pointer;
}

.action-button:hover {
  background-color: #0056b3;
}
</style>
