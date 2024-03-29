import React, { Component } from 'react';

import {
    Icon,
    Segment,
    Accordion,
    List
} from 'semantic-ui-react';
import Pizza from './Pizza';

class QueueSegment extends Component {
    state = { expanded: true }

    render() {
        return (
            <Segment className='queue'>
                <Accordion>
                    <Accordion.Title 
                        className='queueTitle'
                        active={this.state.expanded} 
                        index={0} 
                        onClick={() => this.setState({ expanded: !this.state.expanded })}
                    >
                        <Icon name='dropdown' />
                            <span>{this.props.name} Queue</span>
                            <span style={{ float: 'right', marginRight: '20px'}}>{(this.props.orders && this.props.orders.length) || 0}</span>
                    </Accordion.Title>
                    <Accordion.Content active={this.state.expanded}>
                        {this.props.orders && <List className='pizzaList' divided relaxed>
                            {this.props.orders.map(o => <Pizza name={o.name} id={o.id} size={o.details.size} crust={o.details.type} toppings={o.details.toppings}/>)}
                        </List>}
                    </Accordion.Content>
                </Accordion>
            </Segment>
        );
    }
}

export default QueueSegment;