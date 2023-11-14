import { useState } from "react";

export default function EditableLedgder(props){
    const {defaultVaule}=props;
    const [value,setValue]=useState(defaultVaule);
    return (
        <div>edit</div>
    )
}