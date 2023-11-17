import React from 'react';
import './App.css';
import CommonHeader from './components/CommonHeader';
import RootRoute from './route';
import { useSelector } from 'react-redux';
declare module 'react' {
  interface HTMLAttributes<T> extends AriaAttributes, DOMAttributes<T> {//拓展HTMLAttributes
    // extends React's HTMLAttributes
    theme?: 'default' | 'dark';
  }
}
function App() {
  const selector=(state:any)=>({
    theme:state.setting?.theme,
  });
  const {
    theme,
  }=useSelector(selector);
  return (
    <div className="App" theme={
      theme
    }>
      <CommonHeader/>
      <RootRoute/>
    </div>
  );
}

export default App;
