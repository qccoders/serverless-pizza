import React, { Component } from 'react';
import './App.css';
import logo from './logo.png';

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
        this.refresh();
        setInterval(this.refresh, 2500);
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
        return new Promise((resolve, reject) => {
            axios.post('/orders', order)
            .then(response => {
                sessionStorage.setItem(ORDERIDKEY, order.id);
                resolve(response.data);
            })
            .catch(err => reject(err));
        })
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
                <img className='logo' alt='logo' src={logo}/>
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
