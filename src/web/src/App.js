import React, { Component } from 'react';
import { Route, Link, Switch } from "react-router-dom";
import './App.css';
import OrderForm from './Components/OrderForm';
import OrderTracker from './Components/OrderTracker';
import axios from 'axios';
import { ROOTURL } from './constants';

const ORDERIDKEY = 'serverlss-pizza-order-id';
axios.defaults.baseURL = ROOTURL;
const api = axios.create();

class App extends Component {
    state = { 
        orderId: undefined,
        order: { }
    };

    componentDidMount = () => {
        setInterval(this.refresh, 5000);
    };

    refresh = () => {
        this.setState({ ...this.state, orderId: sessionStorage.getItem(ORDERIDKEY)}, () => {
            if (this.state.orderId) {
                this.getOrder(this.state.orderId)
                .then(order => this.setState({ order: order}));
            }
        });
    };

    forgetOrder = () => {
        sessionStorage.removeItem(ORDERIDKEY);
    };

    placeOrder = (order) => {
        axios.post('/orders', order)
        .then(response => {
            sessionStorage.setItem(ORDERIDKEY, order.id)
        })
        .catch(err => console.log(err));
    };

    placeFakeOrder = () => {
        let guid = Math.random().toString(36).substring(2) + (new Date()).getTime().toString(36);
        let order = {
            details: {
                cookTime: 5,
                size: "small",
                toppings: [
                    "cheese",
                    "pepperoni"
                ],
                type: "regular"
            },
            events: [
                {
                    start: "1-1-2009",
                    type: "Prep",
                }
            ],
            id: guid,
            name: "jp",
            placed: "1-1-2009"
        }

        this.placeOrder(order);
    }

    getOrder = (id) => {
        return new Promise((resolve, reject) => {
            axios.get(`/orders/${id}`)
            .then(response => { 
                console.log('return', response.data);
                resolve(response.data); 
            })
            .catch(err => reject(err));
        });
    }

    render = () => {
        console.log(this.state);

        return (
            <div className="App">
                {!this.state.orderId && <button onClick={this.placeFakeOrder}>Fake order</button>}
                {/* <button onClick={this.forgetOrder}>Forget order</button> */}
                {this.state.orderId ? <OrderTracker order={this.state.order}/> : <OrderForm/> }
            </div>
        );
    }
}

export default App;
