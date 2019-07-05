## Order Model

```js
{
    "id": "guid",
    "name": "customer name",
    "placed": "<timestamp>"
    "details": {
        "crust": { 
            "thin|regular|stuffed": {
                "small|medium|large|jumbo": "<cook time>"
            }
        },
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
    "type": "prep|cook|finish|deliver",
    "start": "<timestamp>",
    "end": "<timestamp>",
}
```

## Settings Model

```js
{
    "toppings": [
        "<topping>"
    ],
    "crusts": [{ 
        "<type>": {
            "<size>": "<cook time>"
        }
    ]
}
```