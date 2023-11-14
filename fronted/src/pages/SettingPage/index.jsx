import { useState } from "react";
import SimpleContainer from "../../components/Container/SimpleContainer";
import {Switch} from "antd-mobile";
export default function SettingPage() {
    const [theme, setTheme] = useState(window.localStorage.getItem('theme') === 'dark');
    return (
        <div>
            <h1>SettingPage</h1>
            <p>应该包含鉴权管理和用户设置</p>
            <SimpleContainer>
                主题：
                <Switch 
                checked={theme}
                onChange={(checked) => {
                    setTheme(checked);
                    window.localStorage.setItem('theme', checked ? 'dark' : 'default');
                }}
                ></Switch>
            </SimpleContainer>
        </div>
    );
}
