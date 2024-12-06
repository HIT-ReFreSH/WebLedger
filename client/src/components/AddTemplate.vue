<script setup lang="ts">
import { getCategories,type Category } from '../api/setting';
import { getTemplate,getTemplates,updateTemplate,deleteTemplate,type Template } from '../api/view';
import { h, ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
const categories = ref<Category[]>([]);
const editTempleteVisible = ref(false);
const templates = ref<string[]>([]);
const templateForm = ref<Template>({
    name: "",
    categories: [],
    isIncome: false,
})

const fetchCategories = async () => {
    const res = await getCategories();
    if(res?.status===200){
        categories.value = res.data;
    }
}
const fetchTemplates = async () => {
    const res = await getTemplates();
    if(res?.status===200){
        templates.value = res.data;
    }
}
const handleEditTempleteVisible = async (template?: string) => {
    if(template){
        templateForm.value.name = template;
        await getTemplate(template).then(res => {
            templateForm.value.categories = res.data.categories;
            templateForm.value.isIncome = res.data.isIncome;
        })
    }
    else{
        templateForm.value = {
            name: "",
            categories: [],
            isIncome: false,
        }
    }
    await fetchCategories();
    editTempleteVisible.value = true;
}
const handleSubmitTemplate = async () => {
    if(templateForm.value.name===""||templateForm.value.categories.length===0||templateForm.value.isIncome===null){
        ElMessage.error("请填写完整模板信息");
        return;
    }
    const res = await updateTemplate(templateForm.value);
    if(res?.status===204){  
        ElMessage.success("模板更新成功");
        editTempleteVisible.value = false;
        fetchTemplates();
    }else{
        ElMessage.error("模板更新失败，可能原因：模板名称已存在");
    }
}
const handleDeleteTemplate = async (template: Template) => {
    ElMessageBox(
       {
            showCancelButton:true,
            title:"警告",
            message: h('p',null,[
                h('span',null,"确定删除模板吗,这会删除所有"),
                h('span',{style:{color:"red"}},`${template.name}`),
                h('span',null,"模板的报表模板和自动化模板")
            ]),
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type:"warning",
            
       }
    ).then(async () => {
        const res = await deleteTemplate(template.name);
        if(res?.status===204){
            ElMessage.success("删除成功");
            fetchTemplates();
        }
        else{
            ElMessage.error("删除失败，请联系管理员");
        }
    })
}
onMounted(async () => {
    await fetchTemplates();
})
</script>
<template>
    <div style="width:100%;height:100%;display:flex;flex-direction:column;justify-content:center;align-items:center;">
        <!-- 模板设置 -->
        <el-button class="big-cta-btn" @click="handleEditTempleteVisible()">添加报表模板</el-button>
        <el-table :data="templates" style="width: 100%;border-radius: 20px;height: 100%;">
            <el-table-column label="模板名称">
                <template #default="scope">
                    {{scope.row}}
                </template>
            </el-table-column>
            <el-table-column label="操作" fixed="right" width="200px">
                <template #default="scope">
                    <el-button size="small" style="background-color:rgb(0, 225, 225);color:rgba(0, 0, 0, 0.699)" @click="handleEditTempleteVisible(scope.row)">查看/编辑</el-button>
                    <el-button size="small" style="background-color:#EE4E4E;color:black" @click="handleDeleteTemplate(scope.row)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <el-dialog 
        v-model="editTempleteVisible"
        title="设置模板" width="300px" >
        <el-form :model="templateForm">
            <el-form-item label="模板名称" required>
                <el-input v-model="templateForm.name" placeholder="请输入模板名称"/>
            </el-form-item>
            <el-form-item label="分类" required>
                <el-select v-model="templateForm.categories" placeholder="请选择分类名称" multiple>
                    <el-option v-for="item in categories" :key="item.name" :label="item.name" :value="item.name"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item required>
                <el-radio-group v-model="templateForm.isIncome">
                <el-radio :label="true">收入</el-radio>
                <el-radio :label="false">支出</el-radio>
            </el-radio-group>
            </el-form-item>
            <el-form-item>
                <div style="width:100%;display:flex;justify-content:flex-end;">
                    <el-button type="text" @click="editTempleteVisible=false">取消</el-button>
                    <el-button type="primary" @click="handleSubmitTemplate">提交</el-button>
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
