import React, { Component } from 'react';

import {
    Icon,
    Loader,
    Header,
    Segment
} from 'semantic-ui-react';

import QueueSegment from './QueueSegment';
import StepSegment from './StepSegment';

class Manager extends Component {
    getStateFromEvents = (events) => {
        let ev = events.reduce((dict, e) => {
            let { type, ...rest } = e;
            dict[type] = rest
            return dict
        }, {})

        if (!ev.Prep) return 'prepQueued';        
        if (!!ev.Prep && !!ev.Prep.start && !ev.Prep.end) return 'prep';
        if (!ev.Cook) return 'cookQueued';
        if (!!ev.Cook && !!ev.Cook.start && !ev.Cook.end) return 'cook';
        if (!ev.Finish) return 'finishQueued';
        if (!!ev.Finish && !!ev.Finish.start && !ev.Finish.end) return 'finish';
        if (!ev.Delivery) return 'deliveryQueued';
        if (!!ev.Delivery && !!ev.Delivery.start && !ev.Delivery.end) return 'delivery';

        return 'done';
    }

    render() {
        if (!this.props.orders) return (<Loader/>)
        var orders = this.props.orders
            .map(d => ({ ...d, state: this.getStateFromEvents(d.events) }))
            .reduce((dict, j) => {
                let { state } = j;
                dict[state] = dict[state] === undefined ? [ j ] : dict[state].concat(j)
                return dict
            }, {});

        return (
            <div className='status'>
                <QueueSegment name='Prep' orders={orders.prepQueued}/>
                <StepSegment name='Prep' iconName='signing' orders={orders.prep} />
                <QueueSegment name='Cook' orders={orders.cookQueued}/>
                <StepSegment name='Cook' iconName='time' orders={orders.cook} />
                <QueueSegment name='Finish' orders={orders.finishQueued}/>
                <StepSegment name='Finish' iconName='chart pie' orders={orders.finish} />
                <QueueSegment name='Delivery' orders={orders.deliveryQueued}/>
                <StepSegment name='Delivery' iconName='shipping fast' orders={orders.delivery} />
                <Segment>
                    <Header as='h2'>
                        <Icon name='checkmark' />
                        <Header.Content>Done</Header.Content>
                    </Header>
                    <span>Count: {orders.done && orders.done.length}</span>
                </Segment>
            </div>
        );
    }
}

export default Manager;