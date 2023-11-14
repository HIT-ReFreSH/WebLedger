import React from 'react';
import './App.css';
import CommonHeader from './components/CommonHeader';
declare module 'react' {
  interface HTMLAttributes<T> extends AriaAttributes, DOMAttributes<T> {//拓展HTMLAttributes
    // extends React's HTMLAttributes
    theme?: 'default' | 'dark';
  }
}
function App() {
  return (
    <div className="App" theme={
      window.localStorage.getItem('theme') === 'dark' ? 'dark' : 'default'
    }>
      <CommonHeader/>
    </div>
  );
}

export default App;
