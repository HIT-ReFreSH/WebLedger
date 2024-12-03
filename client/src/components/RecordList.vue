<script lang="ts" setup>
import { ref, defineProps, computed, onMounted } from 'vue'
import { type Entry, type SelectOption, selectEntries,getCategories } from '@/api/record'
const entries = ref<Entry[]>([])
const filters = ref<SelectOption>({
  dateRange: ['', ''],
  startTime: '',
  endTime: '',
  direction: false,
  category: ''
})
const categories=ref<string[]>([])

const formatDate = (dateString: string) => {
  let date=dateString.split('T')[0];
  let parts=date.split('-');
  return parts[0]+'年'+parts[1]+'月'+parts[2]+'日';
}

const searchEntries = async () => {
  if(filters.value.dateRange[0]===''||filters.value.dateRange[1]===''){
    ElMessage.error('请选择日期范围')
    return
  }
  if(filters.value.category===''){
    ElMessage.error('请选择分类')
    return
  }
  if(filters.value.direction===null){
    ElMessage.error('请选择类型')
    return
  } 
  
  filters.value.startTime=new Date(filters.value.dateRange[0]).toISOString();
  filters.value.endTime=new Date(filters.value.dateRange[1]).toISOString();
  const res = await selectEntries(filters.value);
  console.log(filters.value);
  if(res.status===200){
    entries.value = res.data
    console.log(entries.value);
  }else{
    ElMessage.error('获取记录失败')
  }
}
onMounted(async () => {
  const res = await getCategories()
  if(res.status===200){
    res.data.forEach((item: any) => {
        categories.value.push(item.name);
    })
  }else{
    ElMessage.error('获取分类失败')
  }
})
</script>

<template>
    <div class="transaction-table">
  <!-- 筛选区域 -->
  <div style="width: 700px;">
      <el-form :inline="true" label-width="auto">
        <el-form-item>
          <label slot="label" style="color:#fff;">日期范围：</label>
          <el-date-picker
            v-model="filters.dateRange"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            format="YYYY-MM-DD"
          />
        </el-form-item>
        <el-form-item>
          <label slot="label" style="color:#fff;">类型：</label>
          <el-radio-group v-model="filters.direction" placeholder="选择类型">
            <el-radio :label="true" style="color:#fff;font-size: 18px;">收入</el-radio>
            <el-radio :label="false" style="color:#fff;font-size: 18px;">支出</el-radio>
          </el-radio-group>
        </el-form-item>

        
        <div>
            <el-form-item style="width:200px">
                
                <el-select v-model="filters.category" placeholder="选择分类" clearable>
                    <el-option
                    v-for="category in categories"
                    :key="category"
                    :label="category"
                    :value="category"
                    />
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="searchEntries" class="search-btn">查询</el-button>
            </el-form-item>
        </div>
      </el-form>
    </div>
      <el-table :data="entries" stripe class="table">
        <el-table-column prop="givenTime" label="日期" width="150">
          <template #default="{ row }">
            {{ formatDate(row.givenTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="category" label="分类" width="120" />
        <el-table-column prop="type" label="具体类别" width="120" />
        <el-table-column prop="description" label="描述" />
        <el-table-column prop="amount" label="金额" width="150"/>
      </el-table>
    </div>
  </template>

<style scoped>
.transaction-table {
  margin: 20px 0;
}

.table {
    border-radius: 12px;
    height:450px;
}
.search-btn{
    background-color: rgb(0, 225, 225);
    color: rgba(0, 0, 0, 0.699);
    text-transform: uppercase;
    transition: all 0.3s;
}
</style>
