import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Chat } from './containers';

const rootElement: HTMLElement = document.getElementById('root');

const render = (Component) => {
  ReactDOM.render(
      <Component />,
    rootElement,
  );
};

render(Chat);

if (module.hot) {
  module.hot.accept('./containers', () => { render(Chat); });
}