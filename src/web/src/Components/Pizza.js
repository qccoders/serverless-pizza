import React, { Component } from 'react';

import {
    List,
    Popup
} from 'semantic-ui-react';

class Pizza extends Component {
    state = { expanded: false }

    render() {
        let { name, id, size, crust, toppings } = this.props;

        return (
            <List.Item>
                <Popup content={`Order ID: ${id}`} trigger={<List.Icon name='circle' size='large' verticalAlign='middle' />} />
                
                <List.Content>
                    <List.Header>{size} {crust} for {name}</List.Header>
                    <List.Description>Toppings: {toppings.join(', ')}</List.Description>
                </List.Content>
            </List.Item>
        );
    }
}

export default Pizza;