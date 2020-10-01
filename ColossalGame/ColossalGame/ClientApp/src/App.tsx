import * as React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import FetchData from './components/FetchData';
import GlobalChat from './components/GlobalChat';
import Login from './components/Login';
import Signup from './components/Signup';

import './custom.css'
import MainMenu from './components/MainMenu';
import View from './components/GameMainMenuToggler';

export default () => (
    <body>
        <Route exact path='/' component={View} />
        <Route exact path='/globalchat' component={GlobalChat} />
        <Route path='/login' component={Login} />
        <Route path='/signup' component={Signup} />
    </body>
);