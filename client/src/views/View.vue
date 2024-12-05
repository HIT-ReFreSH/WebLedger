<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { LedgerViewAutomationType, getTemplates,getViews, deleteView, addView, getViewDetail, downloadView } from '../api/view';
import type { ViewAutomation, View,  ViewDetail} from '../api/view';
import { ElMessage,ElMessageBox } from 'element-plus';
const detailVisible = ref(false);
const editViewVisible = ref(false);
const views = ref<string[]>([]);
const viewsAutomation = ref<AutoView[]>([]);
const templates = ref<string[]>([]);
const router = useRouter();
const editViewForm = ref<View>({
    name:"",
    startTime:"",
    endTime:"",
    templateName:""
});
const detailData = ref<ViewDetail>({
    raw:[],
    byCategory:{},
    byTime:{},
    category:[],
    time:[],
    total:0
});

interface AutoView{
    name:string;
    auto:ViewAutomation,
    time:string
}
const filterType=[
    {value:LedgerViewAutomationType.Daily.toString(),text:"日报"},
    {value:LedgerViewAutomationType.Weekly.toString(),text:"周报"},
    {value:LedgerViewAutomationType.Monthly.toString(),text:"月报"},
    {value:LedgerViewAutomationType.Quarterly.toString(),text:"季度报"},
    {value:LedgerViewAutomationType.Yearly.toString(),text:"年报"},
]
const fetchViews = async()=>{
    const res = await getViews();
    if(res.status === 200){
        views.value = res.data;
        let temp=views.value.filter(view=>view.includes(":"));
        viewsAutomation.value = temp.map(view=>({name:view,auto:{type:0,templateName:view.split(":")[0]},time:""}));
        for(let i=0;i<viewsAutomation.value.length;i++){
            timeFormat(viewsAutomation.value[i]);
        }
        views.value = views.value.filter(view=>!view.includes(":"));
    }else{
        ElMessage.error("获取报表失败，请联系管理员");
    }
}
const fetchTemplates = async()=>{
    const res = await getTemplates();
    if(res.status === 200){
        templates.value = res.data;
    }else{
        ElMessage.error("获取报表模板失败，请联系管理员");
    }
}
const handleEditViewVisible = ()=>{
    editViewForm.value = {
        name:"",
        startTime:"",
        endTime:"",
        templateName:""
    };
    fetchTemplates();
    editViewVisible.value = true;
}
const handleSubmitView = async()=>{
    if(editViewForm.value.name === "" || editViewForm.value.startTime === "" || editViewForm.value.endTime === "" || editViewForm.value.templateName === ""){
        ElMessage.error("请填写完整信息");
        return;
    }
    if(editViewForm.value.startTime > editViewForm.value.endTime){
        ElMessage.error("开始时间不能大于结束时间");
        return;
    }
    editViewForm.value.startTime=new Date(editViewForm.value.startTime).toISOString();
    editViewForm.value.endTime=new Date(editViewForm.value.endTime).toISOString();
    const res = await addView(editViewForm.value);
    if(res.status === 204){
        ElMessage.success("添加报表成功");
        fetchViews();
        editViewVisible.value = false;
    }else{
        ElMessage.error("添加报表失败，请联系管理员");
    }
}
const handleDeleteView = async(view: string)=>{
    ElMessageBox(
       {
            showCancelButton:true,
            title:"警告",
            message:"确定删除报表吗？这无法回复",
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type:"warning",
            
       }
    ).then(async () => {
        const res = await deleteView(view);
        if(res.status === 204){
            ElMessage.success("删除报表成功");
            await fetchViews();
        }else{
            ElMessage.error("删除报表失败，请联系管理员");
        }
    })
}
const handleDetailView = async(view: string)=>{
    detailVisible.value = true;
    const res = await getViewDetail(view);
    if(res.status === 200){
        detailData.value = res.data;
        if(detailData.value){
            detailData.value.category = Object.entries(detailData.value.byCategory).map(([name, amount]) => ({ name, amount }));
            detailData.value.total = detailData.value.category[detailData.value.category.length-1].amount;
            detailData.value.category=detailData.value.category.splice(0,detailData.value.category.length-1);
            detailData.value.time = Object.entries(detailData.value.byTime).map(([time, amount]) => ({ time, amount }));
        }
    }else{
        ElMessage.error("获取报表详情失败，请联系管理员");
    }
}
const timeFormat = (view:AutoView)=>{
    const time = view.name.split(":")[1];
    if(time.length === 6)// Daily 输入格式: "240315" (yyMMdd)
    {
        view.auto.type = LedgerViewAutomationType.Daily;
        view.time = `20${time.slice(0,2)}年${parseInt(time.slice(2,4))}月${parseInt(time.slice(4,6))}日`;
    }
    else if(time.includes("Week"))// Weekly 输入格式: "(2024) Week #12"
    {
        const matches = time.match(/\((\d{4})\) Week #(\d+)/);
        if (matches) {
            view.auto.type = LedgerViewAutomationType.Weekly;
            view.time = `${matches[1]}年第${matches[2]}周`;
        }
    } 
    else if(time.length === 4 && time.slice(0,2) !== "20"){// Monthly 输入格式: "2403" (yyMM)
        view.auto.type = LedgerViewAutomationType.Monthly;
        view.time = `20${time.slice(0,2)}年${parseInt(time.slice(2,4))}月`;
    }
    else if(time.includes("Quarter")){// Quarterly 输入格式: "(2024) Quarter #0"
        const quarterMatches = time.match(/\((\d{4})\) Quarter #(\d+)/);
        if (quarterMatches) {
            view.auto.type = LedgerViewAutomationType.Quarterly;
            const quarterNum = parseInt(quarterMatches[2]) + 1; // 因为原始数据从0开始
            view.time = `${quarterMatches[1]}年第${quarterNum}季度`;
        }
    }
    else if(time.length === 4){// Yearly 输入格式: "2024"
        view.auto.type = LedgerViewAutomationType.Yearly;
        view.time = `${time}年`;
    }
    view.auto.templateName = view.name.split(":")[0]+"-"+filterType.find(type=>type.value === view.auto.type.toString())?.text;
}
const filterViewAutomation = (value:string,row:AutoView)=>{
    return row.auto.type.toString() === value;
}
const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}-${date.getDate().toString().padStart(2, '0')} ${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`;
};
onMounted(async()=>{
    await fetchViews();
})
</script>   
<template>  
  <Header :activeItem="1" /> 
  <section>
    <div class="hero-section-container">
        <div class="left-section">
            <div style="width:40%;display:flex;flex-direction:column;justify-content:center;align-items:center;">
                <el-button class="top-btn" @click="handleEditViewVisible">添加报表</el-button>
                <el-table :data="views" style="border-radius: 20px;height: 100%;">
                    <el-table-column label="报表名称">
                        <template #default="scope">
                            {{ (scope.row as string).split(":")[0] }}
                        </template>
                    </el-table-column>
                    <el-table-column label="操作" fixed="right" width="200">
                        <template #default="scope">
                            <el-button class="detail-btn" @click="handleDetailView(scope.row)">查看详情</el-button>
                            <el-button class="delete-btn" @click="handleDeleteView(scope.row)">删除</el-button>
                        </template>
                    </el-table-column>
            </el-table>
            </div>
            <div style="width:60%;display:flex;flex-direction:column;justify-content:center;align-items:center;">
                <el-button class="top-btn" @click="router.push('/setting')">自动生成报表设置</el-button>
                <el-table 
                :data="viewsAutomation" 
                style="border-radius: 20px;height: 100%;">
                <el-table-column label="报表名称">
                    <template #default="scope">
                        {{ scope.row.auto.templateName }}
                    </template>
                </el-table-column>
                <el-table-column label="自动创建时间" prop="time"
                :filters="filterType"
                :filter-method="filterViewAutomation"></el-table-column>
                <el-table-column label="操作" fixed="right" width="150">
                    <template #default="scope">
                        <el-button class="detail-btn" @click="handleDetailView(scope.row.name)">查看详情</el-button>
                    </template>
                </el-table-column>
                </el-table>
            </div>
        </div>
        <div class="right-section">
            <div class="image-wrap">
                <img src="../assets/img/planet.png" alt="planet image">
            </div>
        </div>
    </div>
    <el-dialog v-model="editViewVisible" title="添加报表" width="500px">
        <el-form :model="editViewForm">
            <el-form-item label="报表名称" required>
                <el-input v-model="editViewForm.name" />
            </el-form-item>
            <el-form-item label="开始时间" required>
                <el-date-picker v-model="editViewForm.startTime" type="date" placeholder="选择开始时间" />
            </el-form-item>
            <el-form-item label="结束时间" required>
                <el-date-picker v-model="editViewForm.endTime" type="date" placeholder="选择结束时间" />
            </el-form-item>
            <el-form-item label="报表模板" required>
                <el-select v-model="editViewForm.templateName" placeholder="请选择报表模板">
                    <el-option v-for="template in templates" :key="template" :label="template" :value="template" />
                </el-select>
            </el-form-item>
            <el-form-item>
                <div style="width:100%;display:flex;justify-content:flex-end;">
                    <el-button type="text" @click="editViewVisible=false">取消</el-button>
                    <el-button type="primary" @click="handleSubmitView">提交</el-button>
                </div>
            </el-form-item>
        </el-form>
    </el-dialog>
    <el-dialog v-model="detailVisible" title="报表详情" width="1000px">
        <el-row :gutter="20">
            <!-- Raw Data Table -->
            <el-col :span="24">
                <h3>详细记录</h3>
                <el-table :data="detailData?.raw" style="width: 100%;max-height: 300px;margin-bottom: 20px">
                    <el-table-column prop="givenTime" label="时间" :formatter="(row) => formatDate(row.givenTime)">
                    </el-table-column>
                        <el-table-column prop="amount" label="金额" :formatter="(row) => `¥${Math.abs(row.amount).toFixed(2)}`">
                        </el-table-column>
                    <el-table-column prop="amount" label="占比" :formatter="(row) => `${Math.abs(row.amount/detailData?.total*100).toFixed(2)}%`">
                    </el-table-column>
                    <el-table-column prop="type" label="类型"></el-table-column>
                    <el-table-column prop="category" label="分类"></el-table-column>
                    <el-table-column prop="description" label="描述"></el-table-column>
                </el-table>
            </el-col>

            <!-- Category Summary -->
            <el-col :span="12">
                <h3>分类统计</h3>
                <el-table :data="detailData?.category" style="width: 100%;">
                    <el-table-column prop="name" label="分类"></el-table-column>
                    <el-table-column prop="amount" label="金额" :formatter="(row) => `¥${Math.abs(row.amount).toFixed(2)}`">
                    </el-table-column>
                    <el-table-column prop="amount" label="占比" :formatter="(row) => `${Math.abs(row.amount/detailData?.total*100).toFixed(2)}%`"></el-table-column>
                </el-table>
            </el-col>

            <!-- Time Summary -->
            <el-col :span="12">
                <h3>时间统计</h3>
                <el-table :data="detailData?.time" style="width: 100%">
                    <el-table-column prop="time" label="时间"></el-table-column>
                    <el-table-column prop="amount" label="金额" :formatter="(row) => `¥${row.amount.toFixed(2)}`">
                    </el-table-column>
                    <el-table-column prop="amount" label="占比" :formatter="(row) => `${Math.abs(row.amount/detailData?.total*100).toFixed(2)}%`"></el-table-column>
                </el-table>
            </el-col>
        </el-row>
        <el-row>
            <el-col :span="24">
                <h3>总计: ¥{{ detailData?.total.toFixed(2) }}</h3>
            </el-col>
        </el-row>
    </el-dialog>
  </section> 
</template>

<style lang="scss" scoped>
section {
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
}

.hero-section-container {
  background-color: rgba(255, 255, 255, 0.1);
  height: 85vh;
  width: 85%;
  border-radius: 30px;
  border: 2px solid rgba(211, 211, 211, 0.2);
  backdrop-filter: blur(8px);

  display: flex;
  flex-direction: row;

img {
    width: 90%;
    filter: drop-shadow(0 0 10px rgb(0, 225, 225)) drop-shadow(0 0 20px rgb(0, 225, 225)) drop-shadow(0 0 40px rgb(0, 225, 225)) drop-shadow(0 0 100px rgb(0, 225, 225));
}

.right-section {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30%;
    /* Right Section Animation */
    @keyframes rotatePlanet {
    from {
        transform: rotate(0deg);
    }

    to {
        transform: rotate(360deg);
    }
    }

    .image-wrap {
    display: flex;
    justify-content: center;
    align-items: center;
    animation: rotatePlanet 120s linear infinite;
    }
}

.left-section {
    display: flex;
    flex-direction: row;
    z-index: 1;
    height: 100%;
    width: 100%;
    gap: 20px;

    h1 {
    font-size: 3rem;
    font-weight: 800;
    color: white;
    margin: -20px 0 0 0;
    }

    .left-content-container {
        display: flex;
        align-items: center;
        gap: 100px;
        padding: 30px;
        width: 100%;

        .big-container {
            width: 50%;
        }
    }

}
}
@keyframes sideInLeft {
  from {
    transform: translateX(-100%);
    opacity: 0;
  }

  to {
    transform: translateX(0%);
    opacity: 1;
  }
}

@keyframes sideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }

  to {
    transform: translateX(0%);
    opacity: 1;
  }
}

@keyframes topIn {
  from {
    transform: translateY(-100%);
    opacity: 0;
  }

  to {
    transform: translateY(0%);
    opacity: 1;
  }
}

@keyframes bottomIn {
  from {
    transform: translateY(100%);
    opacity: 0;
  }

  to {
    transform: translateY(0%);
    opacity: 1;
  }
}
.top-btn {
    border-radius: 20px;
    margin-bottom: 10px;
    margin-top: 10px;
    font-weight: bold;
    background-color: rgb(0, 225, 225);
    color: rgba(0, 0, 0, 0.699);
}
.detail-btn {
    border-radius: 20px;
    margin-bottom: 10px;
    margin-top: 10px;
    font-weight: bold;
    background-color: rgb(0, 225, 225);
    color: rgba(0, 0, 0, 0.699);
}
.download-btn {
    border-radius: 20px;
    margin-bottom: 10px;
    margin-top: 10px;
    font-weight: bold;
    background-color: rgb(0, 180, 225);
    color: rgba(0, 0, 0, 0.699);
}
.delete-btn {
    border-radius: 20px;
    margin-bottom: 10px;
    margin-top: 10px;
    font-weight: bold;
    background-color: #EE4E4E;
    color: rgba(0, 0, 0, 0.699);
}
h3 {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
}

h3 {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
}

h1 {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.4s;
}

p {
  animation: sideInLeft 1s ease-out forwards;
  opacity: 0;
  animation-delay: 0.6s;
}

</style>