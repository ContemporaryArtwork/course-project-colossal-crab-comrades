import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as CounterStore from '../store/Counter';




class Signup extends React.Component{



    private handleSubmit = async (
        e: React.FormEvent<HTMLFormElement>
    ): Promise<void> => {
        e.preventDefault();

        if (this.validateForm()) {
            const submitSuccess: boolean = await this.submitForm();
            this.setState({ submitSuccess });
        }
    };


    private validateForm(): boolean {
        // TODO - validate form
        return true;
    }

    private async submitForm(): Promise<boolean> {
        // TODO - submit the form
        return true;
    }

    public render() {
        return (
            <React.Fragment>
                <h1>This is the Signup page</h1>

                <div>
                    <form method="post" action="/api/login" encType = "multipart/form-data">
                    <label>Username:</label>
                    <input type="text" placeholder="Enter Username"></input>
                    <br></br>
                    <label>Password:</label>
                    <input type="text" placeholder="Enter Password"></input>
                    <br></br>
                    <button type="submit">Submit</button>
                    </form>
                </div>
                
                    
            </React.Fragment>
        );
    }



};


export default connect()(Signup);