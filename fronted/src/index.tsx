import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { HashRouter } from 'react-router-dom';
import { createStore, applyMiddleware } from 'redux';
import RootRoute from './route';
import thunk from 'redux-thunk';
import { composeWithDevTools } from 'redux-devtools-extension';
import { Provider } from 'react-redux';
import RootReducer from './reducers';
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
const store = createStore(
  RootReducer,
  composeWithDevTools(applyMiddleware(thunk))
);
root.render(
  <React.StrictMode>
    <Provider store={store}>
      <HashRouter>
        <App />
      </HashRouter>
    </Provider>
  </React.StrictMode>
);