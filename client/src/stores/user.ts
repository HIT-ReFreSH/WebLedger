import { defineStore } from 'pinia';

export const useUserStore = defineStore('user', {
    state: () => ({
        access: '',
        secret: ''
    }),
    persist: true
})