import * as React from 'react';
import { connect } from 'react-redux';
import { RouteComponentProps } from 'react-router';
import { ApplicationState } from '../store';
import * as GlobalChatStore from "../store/GlobalChat";

type GlobalChatProps =
    GlobalChatStore.GlobalChatState &
    typeof GlobalChatStore.actionCreators &
    RouteComponentProps<{}>;

class GlobalChat extends React.PureComponent<GlobalChatProps> {

    public componentDidMount() {
        console.log("About to initialize")
        this.initialize();
    }

    private initialize() {
        this.props.initialize();
    }

    public render() {
        return (
            <React.Fragment>
                <h1>Global Chat</h1>

                <p>This fragment can be easily moved elsewhere since it's an independent component.</p>

                <input value={this.props.username} onChange={(
                    ev: React.ChangeEvent<HTMLInputElement>,
                ): void => { this.props.setUsername(ev.target.value) }}
                />
                <input value={this.props.inputMessage} onChange={(
                    ev: React.ChangeEvent<HTMLInputElement>,
                ): void => { this.props.setInputMessage(ev.target.value) }}
                />

                <button type="button"
                    className="btn btn-primary btn-lg"
                    onClick={() => { this.props.frontendSentMessage(this.props.inputMessage); }}>
                    Send Message
                </button>

                {this.props.messages && this.props.messages.map((m: GlobalChatStore.Message) => (
                    <p>{m.timestamp} &lt;{m.user}&gt;: {m.message}</p>
                    ))}
            </React.Fragment>
        );
    }
};

export default connect(
    (state: ApplicationState) => state.globalchat,
    GlobalChatStore.actionCreators
)(GlobalChat as any);