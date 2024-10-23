// the config of app router

import Zidong from './components/zidong/Zidong.vue';
import Fenlei from './components/fenlei/Fenlei.vue';
import Shouru from './components/shouru/Shouru.vue';
import Baobiao from './components/baobiao/Baobiao.vue';
import App from './App.vue'
export default [{
        path: '/index',
        component: App,
        children: [
          
          {
            name: '收入和支出记录',
            path: 'shouru',
            component: Shouru
        },
            {
              name: '分类记录',
              path: 'fenlei',
              component:Fenlei
            },
           
            
            {
              name: '自动分类',
              path: 'zidong',
              component:Zidong
            },
            
            {
              name: '报表',
              path: 'baobiao',
              component:Baobiao
            },
        ]
    },
    {
        path: '*',
        redirect: '/index/shouru'
    }
]
