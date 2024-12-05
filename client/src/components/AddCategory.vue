<script lang="ts" setup>
import { getCategories, updateCategory, deleteCategory, type Category } from "@/api/setting";
import { ref, onMounted, h} from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
const categories = ref<Category[]>([]);
const categoryForm = ref<Category>({
    name:"",
    superCategory:""
});
const editCategoryVisible = ref(false);
const handleEditCategoryVisible = async (category?: Category) => {
    if(category){
        categoryForm.value.name = category.name;
        categoryForm.value.superCategory = category.superCategory;
    }
    else{
        categoryForm.value.name = "";
        categoryForm.value.superCategory = "";
    }
    editCategoryVisible.value = true;
}
const handleSubmitCategory = async () => {
    if(categoryForm.value.name===""){
        ElMessage.error("分类名称不能为空");
        return;
    }
    if(categoryForm.value.superCategory===categoryForm.value.name){
        ElMessage.error("上级分类不能与分类名称相同");
        return;
    }
    const res = await updateCategory(categoryForm.value);
    console.log(res);
    if(res?.status===204){
        ElMessage.success("设置成功");
        editCategoryVisible.value = false;
        fetchCategories();
    }
    else{
        ElMessage.error("设置失败，可能存在重复分类/循环分类");
    }
}
const handleDelete = (category: Category) => {
    ElMessageBox(
       {
            showCancelButton:true,
            title:"警告",
            message: h('p',null,[
                h('span',null,"确定删除分类吗,这会删除所有"),
                h('span',{style:{color:"red"}},`${category.name}`),
                h('span',null,"分类的收支")
            ]),
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type:"warning",
            
       }
    ).then(async () => {
        const res = await deleteCategory(category.name);
        console.log(res);
        if(res?.status===204){
            ElMessage.success("删除成功");
            fetchCategories();
        }
        else{
            ElMessage.error("删除失败，可能存在子分类，请先删除子分类");
        }
    })
}
const fetchCategories = async () => {
    const res = await getCategories();
    if(res?.status===200){
        categories.value = res.data;
    }
}
onMounted(async () => {
    fetchCategories();
})
</script>
<template>
    <div style="width:100%;height:100%;display:flex;flex-direction:column;justify-content:center;align-items:center;">
        <!-- 分类设置 -->
        <el-button class="big-cta-btn" @click="handleEditCategoryVisible()">添加分类</el-button>
        <el-table :data="categories" style="width: 100%;border-radius: 20px;height: 100%;" stripe>
            <el-table-column prop="name" label="分类名称" />
            <el-table-column prop="superCategory" label="上级分类" />
            <el-table-column label="操作" fixed="right" width="150px">
            <template #default="scope">
                <el-button size="small" style="background-color:rgb(0, 225, 225);color:rgba(0, 0, 0, 0.699)" @click="handleEditCategoryVisible(scope.row)">编辑</el-button>
                <el-button size="small" style="background-color:#EE4E4E;color:black" @click="handleDelete(scope.row)">删除</el-button>
            </template>
            </el-table-column>
        </el-table>
    </div>
    <el-dialog 
        v-model="editCategoryVisible"
        title="设置分类" width="300px" >
        <el-form :model="categoryForm">
            <el-form-item label="分类名称" required>
                <el-input v-model="categoryForm.name" placeholder="请输入分类名称"/>
            </el-form-item>
            <el-form-item label="上级分类">
                <el-select v-model="categoryForm.superCategory" placeholder="请选择上级分类">
                    <el-option v-for="item in categories" :key="item.name" :label="item.name" :value="item.name"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <div style="width:100%;display:flex;justify-content:flex-end;">
                    <el-button type="text" @click="editCategoryVisible=false">取消</el-button>
                    <el-button type="primary" @click="handleSubmitCategory">提交</el-button>
                </div>
            </el-form-item>
        </el-form>
    </el-dialog>
</template>
<style scoped>
.big-cta-btn {
    width: 50%;
    border-radius: 20px;
    margin-bottom: 10px;
    margin-top: 10px;
    font-weight: bold;
    background-color: rgb(0, 225, 225);
    color: rgba(0, 0, 0, 0.699);
}
</style>
