<template>
  <el-menu
    default-active="/testManagement"
    class="el-menu-vertical-demo"
    router
  >
    <el-menu-item index="/index/shouru"
      ><i class="el-icon-message"></i>收入和支出记录</el-menu-item
    >
    <el-menu-item index="/index/fenlei"
      ><i class="el-icon-menu"></i>分类记录</el-menu-item
    >
    <el-menu-item index="/index/zidong"
      ><i class="el-icon-setting"></i>自动分类</el-menu-item
    >
    <el-menu-item index="/index/baobiao"
      ><i class="el-icon-search"></i>报表</el-menu-item
    >
  </el-menu>
</template>
