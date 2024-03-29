const AWS = require('aws-sdk');

AWS.config.update({ region: 'us-east-1' });
const dynamoDB = new AWS.DynamoDB.DocumentClient({ apiVersion: '2012-08-10', convertEmptyValues: true });

exports.handler = async (event) => {
    event = JSON.parse(event.Records[0].body);
    console.log('event', event);
    
    //event.events = event.events || [];
    
    let originalRecord = await getOrder(event.id, event.name);
    originalRecord = originalRecord.Item;
    
    event.events = originalRecord.events || [];
    
    if (event.events.find(e => e.type === 'Prep')) {
        console.log('retransmitted event detected, dying.');
        return;
    }
    
    let newEvent = { type: 'Prep', start: new Date().toISOString() };

    await updateEvents(event.id, event.name, [...event.events, newEvent]);
    
    await sleep(5000);
    
    newEvent.end = new Date().toISOString();
    await updateEvents(event.id, event.name, [...event.events, newEvent]);
};

const getOrder = (id, name) => {
    let params = {
        TableName: 'serverless-pizza-orders',
        ConsistentRead: true,
        Key: {
            "id": id,
            "name": name
        }
    };

    return dynamoDB.get(params).promise();
}

const updateEvents = (id, name, events) => {
    let params = {
        TableName: 'serverless-pizza-orders',
        Key: {
            "id": id,
            "name": name
        },
        UpdateExpression: 'set #a = :x',
        ExpressionAttributeNames: { '#a': 'events' },
        ExpressionAttributeValues: {
            ':x': events
        }
    };

    return dynamoDB.update(params).promise();
}

const sleep = (ms) => {
    return new Promise(resolve => {
        setTimeout(resolve, ms);
    });
}