import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';



class Login extends React.Component{
    public render() {
        return (
            <React.Fragment>
                <h1>This is the login page</h1>

                <div>
                    <label>Username:</label>
                    <input type="text" placeholder="Enter Username"></input>
                    <br></br>
                    <label>Password:</label>
                    <input type="text" placeholder="Enter Password"></input>
                    <br></br>
                    <button>Submit</button>
                </div>
                <br></br>
                <div>
                    <button>Sign up for free!</button>
                </div>
                    

            </React.Fragment>
        );
    }
};

export default connect()(Login);