import React from 'react';
import './App.css';
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
      <header className="App-header">
      {/* <CalendarOutlined />
      <FundProjectionScreenOutlined /> */}
        </header>
    </div>
  );
}

export default App;
