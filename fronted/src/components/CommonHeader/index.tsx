import { Link } from "react-router-dom";

interface HeaderItemProps{
    title?:string;
    to?:string;
    icon?:any;
}
function HeaderItem(props:HeaderItemProps){
    const {title='title',to='./',icon='icon'} = props;
    return (
        <div>
            {icon}
            <Link to={to}>{title}</Link>
        </div>
    )
}
export default function CommonHeader(){
    return (
        <div>
            
        </div>
    );
}