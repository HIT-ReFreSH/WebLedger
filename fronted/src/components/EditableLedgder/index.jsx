import { useState } from "react";
import { Form, Input } from 'antd-mobile'
import { Button, Space } from 'antd-mobile'

export default function EditableLedgder(props) {
    // 该组件用于账单的编辑
    // 传入默认值，同时传入回调函数，用于获取编辑后的值 
    const {
        defaultVaule = {},
        getValue = () => { }
    } = props;
    const [category, setCategory] = useState(defaultVaule.category || '')
    const [type, setType] = useState(defaultVaule.type || '');
    const [amount, setAmount] = useState(defaultVaule.amount || '');
    const [time, setTime] = useState(defaultVaule.time || '');
    return (
        <div>
            <Form layout='horizontal'>
                <Form.Item label='类别' name='category'>
                    <Input placeholder='请输入收支物品类别' clearable onChange={(value) => { setCategory(value) }} />
                </Form.Item>
                <Form.Item label='类型' name='type'>
                    <Input placeholder='请输入收支物品具体类型' clearable onChange={(value) => { setType(value) }} />
                </Form.Item>
                <Form.Item label='金额' name='amount'>
                    <Input placeholder='请输入金额，收入为正数，支出为负数' clearable onChange={(value) => { setAmount(value) }} />
                </Form.Item>
                <Form.Item label='时间' name='time'>
                    <Input placeholder='请按下回车键入当前时间' clearable onChange={(value) => { setTime(value) }} onEnterPress={(e) => {
                        const date = new Date()
                        e.target.value = date.toLocaleString()
                    }} />
                </Form.Item>
            </Form>
            <Button block color='success' size='large' onClick={() => { console.log(category, type, amount, time); }}>
                提交账单
            </Button>
        </div>
    )
}