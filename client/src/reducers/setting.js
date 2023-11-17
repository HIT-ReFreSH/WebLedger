const initalState = {
    theme: window.localStorage.getItem('theme') || 'default',

}
export default function settingReducer(state = initalState, action){
    switch(action.type){
        case 'SET_THEME':
            return {
                ...state,
                theme: action.data
            }
        default:
            return state
    }
}