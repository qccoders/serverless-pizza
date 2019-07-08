import React, { Component } from 'react';
import './App.css';
import OrderForm from './Components/OrderForm';
import OrderTracker from './Components/OrderTracker';
import axios from 'axios';
import { ROOTURL } from './constants';

const ORDERIDKEY = 'serverlss-pizza-order-id';
axios.defaults.baseURL = ROOTURL;

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

    getOrder = (id) => {
        return new Promise((resolve, reject) => {
            axios.get(`/orders/${id}`)
            .then(response => { resolve(response.data); })
            .catch(err => reject(err));
        });
    }

    render = () => {
        return (
            <div className="App">
                {this.state.orderId ? 
                    <OrderTracker 
                        order={this.state.order} 
                        forgetOrder={this.forgetOrder}
                    /> : 
                    <OrderForm 
                        placeOrder={this.placeOrder}
                    /> 
                }
            </div>
        );
    }
}

export default App;
