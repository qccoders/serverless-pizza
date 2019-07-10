import React from 'react';

import {
    Icon,
    Segment,
    Header,
} from 'semantic-ui-react';

export default (props) => (
    <Segment className='done'>
        <Header as='h2'>
            <Icon name='check' />
            <Header.Content style={{ width: '100%' }}>
                <span>Done</span>
                <span style={{ float: 'right', marginRight: 5 }}>{!props.orders ? 0 : props.orders.length}</span>
            </Header.Content>
        </Header>
    </Segment>
);