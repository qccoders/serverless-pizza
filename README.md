<p align="center">
  <img src="docs/architecture.png">
</p>

# 🍕 Serverless Pizza
A fully automated, serverless pizzeria operating on AWS.

## Order Model

```js
{
    "id": "guid",
    "name": "customer name",
    "placed": "<timestamp>"
    "details": {
        "size": "small|medium|large",
        "toppings": [
            "cheese|pepperoni|sausage|whatever"
        ]
    },
    "events": []
}
```

## Event Model

```js
{
    "type": "prep|cook|finish",
    "start": "<timestamp>",
    "end": "<timestamp>",
}
```