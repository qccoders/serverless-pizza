import React, { Component } from 'react';

import {
    Icon,
    Step
} from 'semantic-ui-react';

const initialState = {};

class OrderTracker extends Component {
    state = initialState;

    stepState = (order, type) => {
        const def = { completed: false, disabled: true, active: false };

        if (!order) return def;

        let events = order.events;

        if (!events || events.length === 0) return def;

        let step = events.find(e => e.type === type);

        if (!step) return def;

        let started = step.start !== undefined;
        let ended = !!step.end;

        let retval =  {
            completed: ended,
            disabled: !started,
            active: started && !ended,
        }

        return retval;
    }
   
    render() {
        let order = this.props.order;

        if (!order) return (<br></br>);

        let prep = this.stepState(order, 'Prep');
        let cook = this.stepState(order, 'Cook');
        let finish = this.stepState(order, 'Finish');
        let delivery = this.stepState(order, 'Delivery');

        return (
            <Step.Group vertical size='huge' className='tracker'>
                <Step completed={order.placed !== undefined}>
                    <Icon name='edit' />
                    <Step.Content>
                        <Step.Title>Placed</Step.Title>
                        <Step.Description>Your order has been placed.</Step.Description>
                    </Step.Content>
                </Step>
            
                <Step {...prep}>
                    <Icon name='signing'/>
                    <Step.Content>
                        <Step.Title>Prep</Step.Title>
                        <Step.Description>{`Your pizza ${!prep.completed ? 'is being' : 'has been'} made.`}</Step.Description>
                    </Step.Content>
                </Step>
            
                <Step {...cook}>
                    <Icon name='time' />
                    <Step.Content>
                        <Step.Title>Cook</Step.Title>
                        <Step.Description>{`Your pizza ${!cook.completed ? 'is in the oven' : 'is cooked'}.`}</Step.Description>
                    </Step.Content>
                </Step>

                <Step {...finish}>
                    <Icon name='chart pie' />
                    <Step.Content>
                        <Step.Title>Finish</Step.Title>
                        <Step.Description>{`Your pizza ${!finish.completed ? 'is being prepared' : 'is ready'} for delivery.`}</Step.Description>
                    </Step.Content>
                </Step>

                <Step {...delivery}>
                    <Icon name='shipping fast' />
                    <Step.Content>
                        <Step.Title>Delivery</Step.Title>
                        <Step.Description>{`Your pizza ${!delivery.completed ? 'is out for delivery' : 'has been delivered'}.`}</Step.Description>
                    </Step.Content>
                </Step>
            </Step.Group>
        );
    }
}

export default OrderTracker;