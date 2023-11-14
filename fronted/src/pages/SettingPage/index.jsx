import { useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import SimpleContainer from "../../components/Container/SimpleContainer";
import {Switch} from "antd-mobile";
import { setTheme } from "../../actions/setting";
export default function SettingPage() {
    const selector=state=>({
        theme:state.setting?.theme
    })
    const {theme}=useSelector(selector)
    const dispacth=useDispatch();
    return (
        <div>
            <h1>SettingPage</h1>
            <p>应该包含鉴权管理和用户设置</p>
            <SimpleContainer>
                主题：
                <Switch 
                checked={
                    theme==="dark"
                }
                onChange={(checked) => {
                    dispacth(setTheme(checked?"dark":"default"))
                }}
                ></Switch>
            </SimpleContainer>
        </div>
    );
}
