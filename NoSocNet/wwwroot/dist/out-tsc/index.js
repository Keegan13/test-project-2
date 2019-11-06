import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Chat } from './containers/index';
const rootElement = document.getElementById('root');
const render = () => {
    ReactDOM.render(React.createElement(Chat, null), rootElement);
};
// if (module.hot) {
//     // Hot reloadable React components and translation json files
//     // modules.hot.accept does not accept dynamic dependencies,
//     // have to be constants at compile-time
//     module.hot.accept(() => {
//         ReactDOM.unmountComponentAtNode(rootElement);
//         render();
//     });
// }
render();
//# sourceMappingURL=index.js.map