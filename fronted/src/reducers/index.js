import { combineReducers } from 'redux';
import settingReducer from './setting';

const RootReducer= combineReducers({
    setting:settingReducer
});
export default RootReducer;