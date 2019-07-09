import React, { Component } from 'react';

import {
    Icon,
    Loader,
    Header,
    Segment
} from 'semantic-ui-react';

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
                <Segment>
                    <Header as='h2'>
                        <Icon name='signing' />
                        <Header.Content>Prep</Header.Content>
                    </Header>
                    <span>Count: {orders.prep && orders.prep.length} Queued: {orders.prepQueued && orders.prepQueued.length}</span>
                </Segment>
                <Segment>
                    <Header as='h2'>
                        <Icon name='time' />
                        <Header.Content>Cook</Header.Content>
                    </Header>
                    <span>Count: {orders.cook && orders.cook.length} Queued: {orders.cookQueued && orders.cookQueued.length}</span>
                </Segment>
                <Segment>
                    <Header as='h2'>
                        <Icon name='chart pie' />
                        <Header.Content>Finish</Header.Content>
                    </Header>
                    <span>Count: {orders.finish && orders.finish.length} Queued: {orders.finishQueued && orders.finishQueued.length}</span>
                </Segment>
                <Segment>
                    <Header as='h2'>
                        <Icon name='shipping fast' />
                        <Header.Content>Delivery</Header.Content>
                    </Header>
                    <span>Count: {orders.delivery && orders.delivery.length} Queued: {orders.deliveryQueued && orders.deliveryQueued.length}</span>
                </Segment>
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