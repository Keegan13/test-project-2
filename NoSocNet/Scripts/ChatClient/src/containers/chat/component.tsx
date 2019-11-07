import * as React from 'react';
import { ChatsList } from '../../components/ChatsList';



export const Chat = ({ ...props }) => (
    <div>
        <h2>Chat component</h2>
        <ChatsList />
    </div>
);
