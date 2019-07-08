import React, { Component } from 'react';

import { 
    Segment,
    Form,
    Button,
    Dropdown,
    Label,
    Icon,
    Divider
} from 'semantic-ui-react';

const initialState = {
    name: undefined,
    size: undefined,
    crust: undefined,
    toppings: []
};

const crustOptions = [ 'Regular', 'Thin', 'Stuffed' ];
const sizeOptions = [ 'Small', 'Medium', 'Large', 'Jumbo' ];
const toppingOptions = [ 'Cheese', 'Pepperoni', 'Sausage', 'Beef', 'Pork' ];

class OrderForm extends Component {
    state = initialState;

    getToppingOptions = () => {
        return toppingOptions.map(t => ({ key: t, text: t, value: t }))
    }

    getCrustOptions = () => {
        return crustOptions.map(t => ({ key: t, text: t, value: t }))
    }

    getSizeOptions = () => {
        return sizeOptions.map(t => ({ key: t, text: t, value: t }));
    }

    selectTopping = (e, { name, value }) => {
        let toppings = this.state.toppings;
        toppings.push(value);
        this.setState({ toppings: toppings });
    }

    selectSize = (e, { name, value }) => {
        this.setState({ size: value });
    }

    selectCrust = (e, {name, value}) => {
        this.setState({ crust: value })
    }

    removeTopping = (topping) => {
        let toppings = this.state.toppings;
        toppings.splice(toppings.indexOf(topping), 1);
        this.setState({ toppings: toppings });
    }

    placeOrder = () => {
        let { name, size, crust, toppings } = this.state;
        let guid = Math.random().toString(36).substring(2) + (new Date()).getTime().toString(36);

        let order = {
            details: {
                cookTime: 5,
                size: size,
                toppings: toppings,
                type: crust
            },
            events: [],
            id: guid,
            name: name,
            placed: new Date().toISOString()
        }

        this.props.placeOrder(order).then(r => {
            //this.setState({ ...initialState, toppings: [] })
        });
    }
   
    setName = (e, { name, value }) => {
        this.setState({ name: value });
    }

    render() {
        let { toppings } = this.state;
        console.log(this.state);

        return (
            <Segment className='form'>
                <Form size='huge'>
                    <Form.Field>
                        <label>Name</label>
                        <Form.Input placeholder='Enter your name' onChange={this.setName}/>
                    </Form.Field>
                    <Form.Field>
                        <label>Crust</label>
                        <Dropdown
                            placeholder='Select crust type'
                            fluid
                            selection
                            options={this.getCrustOptions()}
                            onChange={this.selectCrust}
                        />
                    </Form.Field>
                    <Form.Field>
                        <label>Size</label>
                        <Dropdown
                            placeholder='Select size'
                            fluid
                            selection
                            options={this.getSizeOptions()}
                            onChange={this.selectSize}
                        />
                    </Form.Field>
                    <Form.Field>
                        <label>Toppings</label>
                        {toppings.map((t, index) => 
                            <Form.Field key={index}>
                                <Button className='ingredientButton' size='huge' as='div' labelPosition='left'>
                                    <Label style={{ width: '100%'}} as='a' basic pointing='right'>
                                        {t}
                                    </Label>
                                    <Button onClick={() => this.removeTopping(t)} size='huge' icon color='red'>
                                        <Icon name='close' />
                                    </Button>
                                </Button>
                            </Form.Field>
                        )}

                        <Form.Field>
                            <Dropdown
                                placeholder='Select topping'
                                fluid
                                selection
                                value={null}
                                options={this.getToppingOptions()}
                                onChange={this.selectTopping}
                            />
                        </Form.Field>
                    </Form.Field>
                    <Divider/>
                    <Button 
                        onClick={this.placeOrder} 
                        primary 
                        fluid 
                        size='huge' 
                        type='submit'
                    >
                        Place Order
                    </Button>
                </Form>
            </Segment>
        );
    }
}

export default OrderForm; 