export const SET_THEME = 'SET_THEME';
export function setTheme(data){
    window.localStorage.setItem('theme', data);
    return {
        type: SET_THEME,
        data: data
    }
}