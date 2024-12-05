<script setup lang="ts">
import { getTemplates,getViewAutomation,addViewAutomation,deleteViewAutomation} from '../api/view';
import { type Template, type ViewAutomation, LedgerViewAutomationType } from '../api/view';
import { h, ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
const showType = ["每日","每周","每月","每季度","每年"];
const templates = ref<string[]>([]);
const automations = ref<ViewAutomation[]>([]);
const automationForm = ref<ViewAutomation>({
    type:0,
    templateName:"",
})
const editAutomationVisible = ref(false);

const fetchTemplates = async () => {
    const res = await getTemplates();
    if(res?.status===200){
        templates.value = res.data;
    }
    else{
        ElMessage.error("获取报表模板失败，请联系管理员");
    }
}
const fetchViewAutomation = async () => {
    const res = await getViewAutomation();
    if(res?.status===200){
        automations.value = res.data;
    }
}
const handleSubmitAutomation = async () => {
    console.log(automationForm.value);
    if(automationForm.value.templateName===""){
        ElMessage.error("请选择报表模板");
        return;
    }
    const res = await addViewAutomation(automationForm.value);
    if(res?.status===204){
        ElMessage.success("添加成功");
        fetchViewAutomation();
        editAutomationVisible.value = false;
        automationForm.value = {
            type:0,
            templateName:"",
        }
    }else{
        ElMessage.error("添加失败，请联系管理员");
    }
}
const handleDeleteViewAutomation = async (automation: ViewAutomation) => {
    ElMessageBox(
       {
            showCancelButton:true,
            title:"警告",
            message:"确定删除自动化设置吗？",
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type:"warning",
            
       }
    ).then(async () => {
        const res = await deleteViewAutomation(automation);
        if(res?.status===204){
            ElMessage.success("删除成功");
            fetchViewAutomation();
        }
        else{
            ElMessage.error("删除失败，请联系管理员");
        }
    })
}
onMounted(async () => {
    await fetchTemplates();
    await fetchViewAutomation();
})
</script>
<template>
    <div style="width:100%;height:100%;display:flex;flex-direction:column;justify-content:center;align-items:center;">
        <!-- 模板设置 -->
        <el-button class="big-cta-btn" @click="editAutomationVisible=true">添加自动生成报表设置</el-button>
        <el-table :data="automations" style="width: 100%;border-radius: 20px;height: 100%;">
            <el-table-column label="生成频次"
            :filters="showType.map(item=>({text:item,value:item}))">
                <template #default="scope">
                    {{showType[scope.row.type]}}
                </template>
            </el-table-column>
            <el-table-column label="报表模板" prop="templateName"></el-table-column>
            <el-table-column label="操作" fixed="right" width="100px">
                <template #default="scope">
                    <el-button size="small" style="background-color:#EE4E4E;color:black;" @click="handleDeleteViewAutomation(scope.row)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </div>
    <el-dialog 
        v-model="editAutomationVisible"
        title="设置自动化模板" width="300px" >
        <el-form :model="automationForm">
            <el-form-item label="生成频次" required>
                <el-select v-model="automationForm.type" placeholder="请选择生成频次">
                    <el-option v-for="(item,index) in showType" :key="index" :label="item" :value="index"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="报表模板" required>
                <el-select v-model="automationForm.templateName" placeholder="请选择报表模板">
                    <el-option v-for="item in templates" :key="item" :label="item" :value="item"></el-option>
                </el-select>
            </el-form-item>
            <el-form-item>
                <div style="width:100%;display:flex;justify-content:flex-end;">
                    <el-button type="text" @click="editAutomationVisible=false">取消</el-button>
                    <el-button type="primary" @click="handleSubmitAutomation">提交</el-button>
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
