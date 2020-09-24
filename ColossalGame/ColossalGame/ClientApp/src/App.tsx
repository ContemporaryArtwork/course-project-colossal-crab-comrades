import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import GlobalChat from './components/GlobalChat';

import './custom.css'
import MainMenu from './components/MainMenu';

export default () => (
    <body>
        <Route exact path='/' component={MainMenu} />
        <Route exact path='/globalchat' component={GlobalChat} />
    </body>
);