import React, { Component } from 'react';

import {
    Icon,
    Segment,
    Header,
    List
} from 'semantic-ui-react';

import Pizza from './Pizza';

class StepSegment extends Component {
    state = { expanded: false }

    render() {
        let { name, iconName, orders } = this.props;

        return (
            <Segment className='step'>
                <Header as='h2'>
                    <Icon name={iconName} />
                    <Header.Content>{name}</Header.Content>
                </Header>
                {orders && <List className='pizzaList' divided relaxed>
                    {orders.map(o => <Pizza name={o.name} id={o.id} size={o.details.size} crust={o.details.type} toppings={o.details.toppings}/>)}
                </List>}
            </Segment>
        );
    }
}

export default StepSegment;