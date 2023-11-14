import { useState } from "react";

export default function EditableLedgder(props) {
    // 该组件用于账单的编辑
    // 传入默认值，同时传入回调函数，用于获取编辑后的值 
    const {
        defaultVaule = {},
        getValue = () => { }
    } = props;
    const [title, setTitle] = useState(defaultVaule.title || '');
    const [type, setType] = useState(defaultVaule.type || '');
    const [num, setNum] = useState(defaultVaule.num || '');
    return (
        <div>{num}</div>
    )
}